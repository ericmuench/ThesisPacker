using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThesisPacker.Model;

namespace ThesisPacker.BusinessLogic
{
    public class GitAssembleClerk : IThesisPackerClerk
    {
        #region InterfaceFunctions

        public async Task Start(ThesisPackerConfig config, Action<string> onLog)
        {
            //TODO Implement this
            Console.WriteLine("TODO Git Task");
        }

        #endregion
    }
}
