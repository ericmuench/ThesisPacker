using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace ThesisPacker.Model
{
    public class SerializedThesisPackerConfig
    {
        #region Fields
        public List<string> Files { get; set; }
        public List<string> IgnoredFiles { get; set; }
        public string TargetDirectory { get; set; }
        public string ThesisPackName { get; set; }
        public string CodeDirectoryName { get; set;  }
        public List<SerializedGitProject> GitProjects { get; set; }
        #endregion

        #region Constructors
        public SerializedThesisPackerConfig(List<string> files, List<string> ignoredFiles, string targetDirectory, string codeDirName, string thesisPackName, List<SerializedGitProject> gitProjects)
        {
            TargetDirectory = targetDirectory;
            CodeDirectoryName = codeDirName;
            IgnoredFiles = ignoredFiles;
            Files = files;
            GitProjects = gitProjects;
            ThesisPackName = thesisPackName;
        }

        public SerializedThesisPackerConfig() : this(new List<string>(), new List<string>(), "", "", "",new List<SerializedGitProject>()) { }
        #endregion

        #region Functions
        public ThesisPackerConfig ToThesisPackerConfig() => new ThesisPackerConfig(
            Files, 
            IgnoredFiles,
            TargetDirectory, 
            CodeDirectoryName,
            ThesisPackName,
            GitProjects.Select(it => it.ToGitProject()).ToList()
        );
        #endregion
    }

    public class SerializedGitProject
    {
        #region Fields
        public string Name { get; set; }
        public string Url { get; set; }
        public List<string> IgnoredBranches { get; set; }
        public SerializedGitCredentials? GitCredentials { get; set; }
        #endregion

        #region Constructors
        public SerializedGitProject(string name, string url, List<string> ignoredBranches, SerializedGitCredentials? credentials)
        {
            Name = name;
            Url = url;
            GitCredentials = credentials;
            IgnoredBranches = ignoredBranches;
        }

        public SerializedGitProject(string name, string url, SerializedGitCredentials? credentials) : this(name, url, new List<string>(), credentials) { }

        public SerializedGitProject() : this("", "", new List<string>(), null) { }
        #endregion

        #region Functions
        public GitProject ToGitProject() => new GitProject(this.Name, this.Url, this.IgnoredBranches, this.GitCredentials?.ToGitCredentials());
        #endregion
    }

    public class SerializedGitCredentials
    {
        #region Fields
        public string UserName { get; set; }
        public string Password { get; set; }
        #endregion

        #region Constructors
        public SerializedGitCredentials(string userName, string password)
        {
            UserName = userName;
            Password = password;
        }

        public SerializedGitCredentials() : this("", "") { }
        #endregion

        #region Functions
        public GitCredentials ToGitCredentials() => new GitCredentials(UserName, Password);
        #endregion
    }
}
