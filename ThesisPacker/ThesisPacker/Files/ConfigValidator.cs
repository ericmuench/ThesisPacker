using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThesisPacker.Extensions;
using ThesisPacker.Model;

namespace ThesisPacker.Files
{
    public class ConfigValidator
    {
        #region Functions
        public ConfigValidationInfo ValidateConfig(ThesisPackerConfig config)
        {
            //validate target directory
            if (!IsValidDirectoryName(config.TargetDirectory))
            {
                return ConfigValidationInfo.InvalidTargetDirectoryName;
            }

            // validating files --> files is invalid if there are duplicates in files
            if (config.Files.ContainsDuplicates())
            {
                return ConfigValidationInfo.DuplicateFiles;
            }

            //validate git folder--> invalid if there are git Projects specified and dir name is invalid
            if (!IsValidDirectoryName(config.CodeDirectoryName) && !config.GitProjects.IsEmpty())
            {
                return ConfigValidationInfo.InvalidCodeDirectoryName;
            }

            //validate git projects --> invalid if there are duplicate projects (multiple projects with same name)
            if (config.GitProjects.Select(p => p.Name).ContainsDuplicates())
            {
                return ConfigValidationInfo.DuplicateGitProjects;
            }

            return ConfigValidationInfo.Valid;

        }

        private bool IsValidDirectoryName(string dirName) => !dirName.IsNullOrWhiteSpace();
        #endregion

        #region ConfigValidationInfo
        public enum ConfigValidationInfo
        {
            Valid,
            DuplicateFiles,
            DuplicateGitProjects,
            InvalidCodeDirectoryName,
            InvalidTargetDirectoryName
        }
        #endregion
    }
}
