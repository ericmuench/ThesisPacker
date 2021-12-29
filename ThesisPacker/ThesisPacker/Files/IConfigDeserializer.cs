using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThesisPacker.Model;

namespace ThesisPacker.Files
{
    public interface IConfigDeserializer
    {
        public Task<ThesisPackerConfig> DeserializeConfig(string configFilePath);
    }
}
