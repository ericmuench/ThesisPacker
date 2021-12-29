using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThesisPacker.Model;

namespace ThesisPacker.BusinessLogic
{
    public class FilesAssembleClerk : IThesisPackerClerk
    {
        #region Interface Functions
        public async Task Start(ThesisPackerConfig config, Action<string> onLog)
        {
            var copyTasks = config.Files.Select(it => CopyFile(it, config.TargetFolder, onLog));
            foreach (var copyTask in copyTasks)
            {
                await copyTask;
            }
        }
        #endregion

        #region Help Functions

        private async Task CopyFile(string originalFilePath, string destinationDirectory, Action<string> onLog)
        {
            FileAttributes attrs = File.GetAttributes(originalFilePath);
            if (attrs.HasFlag(FileAttributes.Directory))
            {
                string newDestinationDir = Path.Combine(destinationDirectory, Path.GetFileName(originalFilePath));
                if (!Directory.Exists(newDestinationDir))
                {
                    Directory.CreateDirectory(newDestinationDir);
                }
                var tasks = Directory
                    .GetFiles(originalFilePath)
                    .Select(dirPath => CopyFile(originalFilePath, newDestinationDir, onLog));

                foreach (var task in tasks.Reverse())
                {
                    await task;
                }
                onLog($"COPIED ALL FILES OF DIR {originalFilePath}");
                //TODO: What happens if directory exists?
            }
            else
            {
                var destFilePath = Path.Combine(destinationDirectory, Path.GetFileName(originalFilePath));
                File.Copy(originalFilePath, destFilePath);
                onLog($"COPIED {originalFilePath} TO {destFilePath}");
            }

        }
        #endregion
    }
}
