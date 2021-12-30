using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ThesisPacker.Model;

namespace ThesisPacker.BusinessLogic
{
    #nullable enable
    public class FilesAssembleClerk : IThesisPackerClerk
    {
        #region Interface Functions
        public async Task Start(ThesisPackerConfig config, Action<string> onLog)
        {
            IEnumerable<string> allCopiedFiles = new List<string>(); 
            var copyTasks = config
                .Files
                .Distinct()
                .Select(filePath => CopyData(filePath, config.TargetFolder, onLog));
            await Task.WhenAll(copyTasks);
        }
        #endregion

        #region Help Functions
        private async Task<FileCopyOperationStatus> CopyData(string originalFilePath, string destinationDirectory, Action<string> onLog)
        {
            FileAttributes attrs = File.GetAttributes(originalFilePath);
            if (attrs.HasFlag(FileAttributes.Directory))
            {
                string newDestinationDir = Path.Combine(destinationDirectory, Path.GetFileName(originalFilePath));
                if (!Directory.Exists(newDestinationDir))
                {
                    Directory.CreateDirectory(newDestinationDir);
                }

                IEnumerable<Task<FileCopyOperationStatus>> statusTasks = Directory
                    .GetFiles(originalFilePath)
                    .Concat(Directory.GetDirectories(originalFilePath))
                    .Select(path => CopyData(path, newDestinationDir, onLog));

                FileCopyOperationStatus[] allStatus = await Task.WhenAll(statusTasks);

                if (allStatus.All(st => st == FileCopyOperationStatus.Success))
                {
                    onLog($"\n\nCOPIED ALL FILES OF DIR {originalFilePath}\n\n");
                    return FileCopyOperationStatus.Success;
                }
                
                if (allStatus.Any(st => st == FileCopyOperationStatus.Success || st == FileCopyOperationStatus.PartlySuccess))
                {
                    onLog($"\n\nAN ERROR OCCURRED WHILE COPYING FILES\nCOPIED SOME FILES OF DIR {originalFilePath}\n\n");
                    return FileCopyOperationStatus.PartlySuccess;
                }

                onLog($"\n\nCOPYING FILES OF DIR {originalFilePath} FAILED!!!\n\n");
                return FileCopyOperationStatus.Failed;
            }
            else
            {
                return CopyFile(originalFilePath, destinationDirectory, onLog);
            }

        }

        private FileCopyOperationStatus CopyFile(string originalFilePath, string destinationDirectory, Action<string> onLog)
        {
            try
            {
                var fileName = Path.GetFileName(originalFilePath);
                var destFilePath = Path.Combine(destinationDirectory, fileName);

                var cnt = 2;
                while (File.Exists(destFilePath))
                {
                    destFilePath = Path.Combine(destinationDirectory, $"{Path.GetFileNameWithoutExtension(originalFilePath)}-{cnt}{Path.GetExtension(originalFilePath)}");
                    cnt++;
                }

                File.Copy(originalFilePath, destFilePath);
                onLog($"COPIED {fileName} TO {destFilePath}");
                return FileCopyOperationStatus.Success;
            }
            catch (Exception ex)
            {
                onLog(ex.ToString());
                return FileCopyOperationStatus.Failed;
            }
        }
        #endregion

        #region Inner classes
        private enum FileCopyOperationStatus
        {
            Success, Failed, PartlySuccess
        }
        #endregion
    }
}
