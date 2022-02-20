using System;
using System.IO;
using System.IO.Compression;
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

        public BusinessLogicClerk() : this(new FilesAssembleClerk(), new GitAssembleClerk(new FilesAssembleClerk())) { }
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

                var workingDir = Path.Combine(config.TargetDirectory, config.ThesisPackName);
                if (!Directory.Exists(workingDir))
                {
                    Directory.CreateDirectory(workingDir);
                }

                onLog(MsgStartFileAssemble);
                var filesTask = _filesAssembleClerk.Start(config, workingDir,onLog);
                
                onLog(MsgStartGitAssemble);
                var gitTask = _gitAssembleClerk.Start(config, workingDir,onLog);

                await filesTask;
                await gitTask;

                var thesisPackFilePath = Path.Combine(
                    config.TargetDirectory,
                    $"{config.ThesisPackName}_{DateTime.Now:yyyy-MM-dd-hhmmss}.zip"
                );
                ZipFile.CreateFromDirectory(workingDir, thesisPackFilePath);
                onLog(MsgCreatedThesisZipSuccess);
                onLog(MsgPerformingCleanup);
                if (Directory.Exists(workingDir))
                {
                    Directory.Delete(workingDir, true);
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
