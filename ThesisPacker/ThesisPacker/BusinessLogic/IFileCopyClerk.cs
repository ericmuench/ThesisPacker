using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThesisPacker.BusinessLogic
{
    public interface IFileCopyClerk
    {
        public Task<FileCopyOperationStatus> CopyData(string originalFilePath, string destinationDirectory,
            List<string> ignoredFiles,bool copyBaseDir,Action<string> onLog);

    }

    public enum FileCopyOperationStatus
    {
        Success, Failed, PartlySuccess, Skipped
    }
}
