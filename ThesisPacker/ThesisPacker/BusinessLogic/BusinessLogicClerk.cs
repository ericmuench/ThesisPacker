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

        private const string ErrTargetDirectoryAlreadyExists =
            "The Target Directory already exists. Please check your config file!";

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
            try
            {
                if (!Directory.Exists(config.TargetFolder))
                {
                    Directory.CreateDirectory(config.TargetFolder);
                }

                var filesTask = _filesAssembleClerk.Start(config, onLog);
                var gitTask = _gitAssembleClerk.Start(config, onLog);

                await filesTask;
                await gitTask;
            }
            catch (Exception ex)
            {
                onLog(ex.StackTrace ?? ex.Message);
            }
        }
        #endregion
    }
}
