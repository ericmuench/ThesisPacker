using System.IO;
using System.Threading.Tasks;
using ThesisPacker.Model;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace ThesisPacker.Files
{
    public class YamlConfigDeserializer : IConfigDeserializer
    {
        #region Interface Functions
        public async Task<ThesisPackerConfig> DeserializeConfig(string configFilePath)
        {
            var fileText = await File.ReadAllTextAsync(configFilePath);
            var deserialized = new DeserializerBuilder()
                .WithNamingConvention(new CamelCaseNamingConvention())
                .Build()
                .Deserialize<SerializedThesisPackerConfig>(fileText);
            return deserialized.ToThesisPackerConfig();
        }

        #endregion
    }
}
