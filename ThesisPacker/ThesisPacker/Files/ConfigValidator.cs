using System.IO;
using System.Linq;
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

            //validate thesis pack name
            if (!IsValidFileName(config.ThesisPackName))
            {
                return ConfigValidationInfo.InvalidThesisPackName;
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

        private bool IsValidDirectoryName(string dirName)
        {
            if (dirName.IsNullOrWhiteSpace())
            {
                return false;
            }

            return dirName.All(ch => !Path.GetInvalidPathChars().Contains(ch));
        }

        private bool IsValidFileName(string filename)
        {
            if (filename.IsNullOrWhiteSpace())
            {
                return false;
            }

            return filename.All(ch => !Path.GetInvalidFileNameChars().Contains(ch));
        }

        #endregion

        #region ConfigValidationInfo
        public enum ConfigValidationInfo
        {
            Valid,
            DuplicateFiles,
            DuplicateGitProjects,
            InvalidThesisPackName,
            InvalidCodeDirectoryName,
            InvalidTargetDirectoryName
        }
        #endregion
    }
}
