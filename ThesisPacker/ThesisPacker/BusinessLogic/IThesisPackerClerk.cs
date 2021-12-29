using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThesisPacker.Model;

namespace ThesisPacker.BusinessLogic
{ 
    interface IThesisPackerClerk
    {
        public Task Start(ThesisPackerConfig config, Action<string> onLog);
    }
}
