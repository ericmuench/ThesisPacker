using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ThesisPacker.Model;

namespace ThesisPacker.BusinessLogic
{
    #nullable enable
    public class FilesAssembleClerk : IThesisPackerSubClerk, IFileCopyClerk
    {
        #region Interface Functions
        public async Task Start(ThesisPackerConfig config, string rootDir, Action<string> onLog)
        {
            var copyTasks = config
                .Files
                .Distinct()
                .Select(filePath => CopyData(filePath, rootDir, config.IgnoredFiles , true,onLog));
            await Task.WhenAll(copyTasks);
        }
        #endregion

        #region Help Functions
        public async Task<FileCopyOperationStatus> CopyData(string originalFilePath, string destinationDirectory, List<string> ignoredFiles, bool copyBaseDir, Action<string> onLog)
        {

            if (ignoredFiles.Contains(originalFilePath))
            {
                return FileCopyOperationStatus.Skipped;
            }

            FileAttributes attrs = File.GetAttributes(originalFilePath);
            if (attrs.HasFlag(FileAttributes.Directory))
            {
                string newDestinationDir = (copyBaseDir) ? Path.Combine(destinationDirectory, Path.GetFileName(originalFilePath)) : destinationDirectory;
                if (!Directory.Exists(newDestinationDir))
                {
                    Directory.CreateDirectory(newDestinationDir);
                }

                IEnumerable<Task<FileCopyOperationStatus>> statusTasks = Directory
                    .GetFiles(originalFilePath)
                    .Concat(Directory.GetDirectories(originalFilePath))
                    .Select(path => CopyData(path, newDestinationDir, ignoredFiles, true, onLog));

                FileCopyOperationStatus[] allStatus = await Task.WhenAll(statusTasks);

                if (allStatus.All(st => st == FileCopyOperationStatus.Success))
                {
                    onLog($"\n\nCOPIED ALL FILES OF DIR {originalFilePath}\n\n");
                    return FileCopyOperationStatus.Success;
                }

                if (allStatus.All(st => st == FileCopyOperationStatus.Failed))
                {
                    onLog($"\n\nCOPYING FILES OF DIR {originalFilePath} FAILED!!!\n\n");
                    return FileCopyOperationStatus.Failed;
                }

                if (allStatus.All(st => st == FileCopyOperationStatus.Skipped))
                {
                    onLog($"\n\nCOPYING FILES OF DIR {originalFilePath} WAS COMPLETELY SKIPPED...\n\n");
                    return FileCopyOperationStatus.Skipped;
                }

                if (allStatus.Any(st => st == FileCopyOperationStatus.Skipped))
                {
                    onLog($"\n\nSOME FILES OF DIR {originalFilePath} WERE SKIPPED...\n\n");
                }

                if (allStatus.Any(st => st == FileCopyOperationStatus.Failed || st == FileCopyOperationStatus.PartlySuccess))
                {
                    onLog($"\n\nAN ERROR OCCURRED WHILE COPYING FILES\nCOPIED SOME FILES OF DIR {originalFilePath}\n\n");
                    return FileCopyOperationStatus.PartlySuccess;
                }

                //assume everything is ok
                return FileCopyOperationStatus.Success;

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

                //Eventhough this should never happen, the following loop creates a new file name to avoid unexpected results because of duplicate filenames
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
    }
}
