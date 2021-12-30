using System;
using System.Collections.Generic;
using System.IO;
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
