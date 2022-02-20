using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using LibGit2Sharp;
using ThesisPacker.Extensions;
using ThesisPacker.Model;

namespace ThesisPacker.BusinessLogic
{
    public class GitAssembleClerk : IThesisPackerSubClerk
    {
        #region Fields
        private const string ErrInvalidCodeDirName =
            "Code-Directory Name is invalid. Please use a name not only containning whitespaces";
        private readonly IFileCopyClerk _fileCopyClerk;
        #endregion

        #region Constructors
        public GitAssembleClerk(IFileCopyClerk fileCopyClerk)
        {
            _fileCopyClerk = fileCopyClerk;
        }
        #endregion

        #region InterfaceFunctions
        public async Task Start(ThesisPackerConfig config, string rootDir, Action<string> onLog)
        {
            //Do early return if there are no Projects specified
            if (config.GitProjects.IsEmpty())
            {
                onLog("There are no Git-Projects. Skipping Git Assemble!");
                return;
            }

            var fullCodeDirPath = Path.Combine(rootDir, config.CodeDirectoryName);
            if (!Directory.Exists(fullCodeDirPath))
            {
                Directory.CreateDirectory(fullCodeDirPath);
            }

            IEnumerable<Task> gitAssembleTasks = config.GitProjects.Select(proj => AssembleGitProject(proj,fullCodeDirPath, onLog));
            await Task.WhenAll(gitAssembleTasks);

        }
        #endregion

        #region Help Functions

        private async Task AssembleGitProject(GitProject project, string codeDirPath, Action<string> onLog)
        {
            try
            {
                //create directory for project
                var projectDir = Path.Combine(codeDirPath, project.Name);
                var workingDir = Path.Combine(projectDir, $"WorkingDir-{Guid.NewGuid()}");

                if (!Directory.Exists(projectDir))
                {
                    Directory.CreateDirectory(projectDir);
                }

                if (!Directory.Exists(workingDir))
                {
                    Directory.CreateDirectory(workingDir);
                }


                //clone project in just created directory
                onLog($"Cloning Project {project.Name}");
                if (project.GitCredentials != null)
                {
                    var cloneOptions = new CloneOptions
                    {
                        CredentialsProvider = (url, user, cred) => new UsernamePasswordCredentials
                        {
                            Username = project.GitCredentials.UserName,
                            Password = project.GitCredentials.Password
                        }
                    };
                    Repository.Clone(project.Url, workingDir, cloneOptions);
                }
                else
                {
                    Repository.Clone(project.Url, workingDir);
                }

                //checkout branches
                using (var repo = new Repository(workingDir))
                {
                    var checkoutBranches = repo
                        .Branches
                        .ToList()
                        .Where(br => br.IsRemote && !project.IgnoredBranches.Contains(GetSimpleBranchName(br)))
                        .ToList();

                    Console.WriteLine($"{project.Name}:Got Branches: {checkoutBranches.PrettyPrint()}");

                    var zipFiles = new List<string>();
                    if (checkoutBranches.Count > 1)
                    {
                        var branchPackTasks = new List<Task>();
                        foreach (var branch in checkoutBranches)
                        {
                            var branchName = GetSimpleBranchName(branch).Replace("/", "_");
                            onLog($"Checking out Branch {branchName} in Project {project.Name}");
                            Commands.Checkout(repo, branch);
                            branchPackTasks.Add(PackBranch(workingDir, $"{project.Name}-{branchName}",projectDir, project.KeepGitIntegration, onLog));
                        }

                        await Task.WhenAll(branchPackTasks);
                    }
                    else if (!checkoutBranches.IsEmpty())
                    {
                        await PackBranch(workingDir, project.Name, projectDir, project.KeepGitIntegration, onLog);
                    }
                }

                // Delete Working directory
                if (Directory.Exists(workingDir))
                {
                    DisableGitIntegration(workingDir);
                    Directory.Delete(workingDir, true);
                }
            }
            catch (Exception e)
            {
                onLog(e.ToString());
            }
        }

        private async Task PackBranch(string sourceDir,string projectBranchId, string projectDir, bool keepGitIntegration, Action<string> onLog)
        {
            var copyDir = Path.Combine(projectDir, $"CopyDir-{projectBranchId}-{Guid.NewGuid()}");
            if (!Directory.Exists(copyDir))
            {
                Directory.CreateDirectory(copyDir);
            }

            var ignoredFiles = new List<string>();
            await _fileCopyClerk.CopyData(sourceDir, copyDir, ignoredFiles, false, onLog);

            if (!keepGitIntegration)
            {
                DisableGitIntegration(copyDir);
                ZipBranch(copyDir, Path.Combine(projectDir, $"{projectBranchId}.zip"), onLog);
            }
            else
            {
                onLog($"Keeping Git Integration for {projectBranchId}");
                ZipBranch(copyDir, Path.Combine(projectDir, $"{projectBranchId}.zip"), onLog);
                DisableGitIntegration(copyDir);
            }

            Directory.Delete(copyDir, true);
        }

        private bool ZipBranch(string branchDir, string targetFile, Action<string> onLog)
        {
            try
            {
                ZipFile.CreateFromDirectory(branchDir, targetFile);
                onLog($"Successfully zipped {targetFile}");
                return true;
            }
            catch (Exception e)
            {
                onLog(e.ToString());
                return false;
            }
        }

        /// <summary>
        /// Disable Git Integration by deleting the .git-Folder
        /// </summary>
        /// <param name="repoPath">The path of the Repository</param>
        private void DisableGitIntegration(string repoPath)
        {
            var gitDirPath = Path.Combine(repoPath, ".git");
            var gitDir = new DirectoryInfo(gitDirPath) { Attributes = FileAttributes.Normal };

            foreach (var info in gitDir.GetFileSystemInfos("*", SearchOption.AllDirectories))
            {
                info.Attributes = FileAttributes.Normal;
            }

            gitDir.Delete(true);
        }

        private string GetSimpleBranchName(Branch branch)
        {
            return branch
                .FriendlyName
                .Replace($"{branch.RemoteName}/", "");
        }
        #endregion
    }
}
