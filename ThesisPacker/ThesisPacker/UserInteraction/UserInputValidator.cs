using System.IO;
using ThesisPacker.Extensions;

namespace ThesisPacker.UserInteraction
{
    #nullable enable
    public class UserInputValidator
    {
        #region Functions

        public bool IsValidConfigFilePath(string? configFilePath)
        {
            if (configFilePath == null)
            {
                return false;
            }

            return File.Exists(@configFilePath);
        }
        #endregion
    }
}
