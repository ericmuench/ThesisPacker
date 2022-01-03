using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThesisPacker.Model;

#nullable enable
namespace ThesisPacker.BusinessLogic
{
    public class BusinessLogicClerk : IThesisPackerClerk
    {
        #region Fields

        private const string MsgStartThesisPacking = "Start Thesis Packing...";
        private const string MsgPrepareTargetDir = "Preparing Target Directory...";
        private const string MsgStartFileAssemble = "Starting File Assemble...";
        private const string MsgStartGitAssemble = "Starting Git Assemble...";
        private const string MsgCreatedThesisZipSuccess = "Successfully created zip file for thesis";
        private const string MsgPerformingCleanup = "Performing Cleanup...";
        private const string ErrOccurred = "An Error Occurred!";

        private readonly FilesAssembleClerk _filesAssembleClerk;
        private readonly GitAssembleClerk _gitAssembleClerk;
        #endregion

        #region Constructors
        public BusinessLogicClerk(
            FilesAssembleClerk filesAssembleClerk,
            GitAssembleClerk gitAssembleClerk
        )
        {
            _filesAssembleClerk = filesAssembleClerk;
            _gitAssembleClerk = gitAssembleClerk;
        }

        public BusinessLogicClerk() : this(new FilesAssembleClerk(), new GitAssembleClerk()) { }
        #endregion

        #region Interface Functions
        public async Task Start(ThesisPackerConfig config, Action<string> onLog)
        {
            onLog(MsgStartThesisPacking);
            try
            {
                onLog(MsgPrepareTargetDir);
                if (!Directory.Exists(config.TargetDirectory))
                {
                    Directory.CreateDirectory(config.TargetDirectory);
                }

                onLog(MsgStartFileAssemble);
                var filesTask = _filesAssembleClerk.Start(config, onLog);
                
                onLog(MsgStartGitAssemble);
                var gitTask = _gitAssembleClerk.Start(config, onLog);

                await filesTask;
                await gitTask;

                var thesisPackFilePath = Path.Combine(
                    Path.GetDirectoryName(config.TargetDirectory) ?? "C:\\",
                    $"{Path.GetFileName(config.TargetDirectory)}_{DateTime.Now:yyyy-MM-dd-hhmmss}.zip"
                );
                ZipFile.CreateFromDirectory(config.TargetDirectory, thesisPackFilePath);
                onLog(MsgCreatedThesisZipSuccess);
                onLog(MsgPerformingCleanup);
                if (Directory.Exists(config.TargetDirectory))
                {
                    Directory.Delete(config.TargetDirectory, true);
                }
            }
            catch (Exception ex)
            {
                onLog(ErrOccurred);
                onLog(ex.ToString());
            }
        }
        #endregion
    }
}
