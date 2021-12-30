using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThesisPacker.Extensions;
using ThesisPacker.Model;

namespace ThesisPacker.BusinessLogic
{
    public class GitAssembleClerk : IThesisPackerClerk
    {
        #region Fields

        private const string ErrInvalidCodeDirName =
            "Code-Directory Name is invalid. Please use a name not only containning whitespaces";
        #endregion

        #region InterfaceFunctions
        public async Task Start(ThesisPackerConfig config, Action<string> onLog)
        {
            //Do early return if there are no Projects specified
            if (config.GitProjects.IsEmpty())
            {
                return;
            }

            string fullCodeDirPath = Path.Combine(config.TargetDirectory, config.CodeDirectoryName);
            if (!Directory.Exists(fullCodeDirPath))
            {
                Directory.CreateDirectory(fullCodeDirPath);
            }

            IEnumerable<Task> gitAssembleTasks = config.GitProjects.Select(proj => AssembleGitProject(proj,fullCodeDirPath));
            await Task.WhenAll(gitAssembleTasks);

        }
        #endregion

        #region Help Functions
        private async Task AssembleGitProject(GitProject project, string codeDirPath)
        {
            //TODO Implement Logic for fetching git project and checking out all branches except ignored ones
            //create directory for project
            //checkout project in just created directory

        }
        #endregion
    }
}
