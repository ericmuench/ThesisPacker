using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ThesisPacker.Files;
using ThesisPacker.Model;
using ThesisPacker.BusinessLogic;
using ThesisPacker.Extensions;

namespace ThesisPacker.UserInteraction
{
    #nullable enable
    public class MainMenu
    {
        #region Fields
        private const string MsgPleaseEnterConfigPath =
            "There was no config path specified via Programm Arguments. Please enter the File-Path to your prefered Config.yaml File:";
        private const string MsgDeserializedConfig =
            "Successfully obtained Config...";

        private string MsgGreeting = $"Welcome to the ThesisPacker v{Assembly.GetExecutingAssembly()?.GetName()?.Version}";
        private const string MsgUsedLibraries =
            "This Project is using LibGit2Sharp and YamlDotNet. \n"+
            "Copyright (c) LibGit2Sharp contributors\nPermission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files(the \"Software\"), " +
            "to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit " +
            "persons to whom the Software is furnished to do so, subject to the following conditions:\nThe above copyright notice and this permission notice shall be included in all copies " +
            "or substantial portions of the Software.\nTHE SOFTWARE IS PROVIDED \"AS IS\", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE " +
            "WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, " +
            "DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH" +
            " THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.\n\n\n"+
            "Copyright (c) 2008, 2009, 2010, 2011, 2012, 2013, 2014 Antoine Aubry and contributors\nPermission is hereby granted, " +
            "free of charge, to any person obtaining a copy of this software and associated documentation files(the \"Software\"), to " +
            "deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, " +
            "distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, " +
            "subject to the following conditions: \nThe above copyright notice and this permission notice shall be included in all copies " +
            "or substantial portions of the Software.\nTHE SOFTWARE IS PROVIDED \"AS IS\", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, " +
            "INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT " +
            "SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, " +
            "TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.\n\n";

        private const string ErrInvalidConfigPath =
            "The specified Path seems not to be valid or does not exist. Please enter the File-Path to your prefered Config.yaml File:";
        private const string ErrInvalidConfigTargetDirectory =
            "The specified Config files contains an error: The specified target directory is invalid! Please change your Config and try again!";
        private const string ErrInvalidConfigCodeDirectory =
            "The specified Config files contains an error: There seems to be a usage of Code, but the Code Directory is invalid. Please change your Config and try again!";
        private const string ErrDuplicateFiles =
            "The specified Config files contains an error: There seem to be file duplications. Please change your Config and try again!";
        private const string ErrDuplicateGitProjects =
            "The specified Config files contains an error: There seem to git Project duplications. Each Project needs an unique name. Please change your Config and try again!";
        private const string ErrInvalidThesisPackName =
            "The specified Config files contains an error: The ThesisPackName is invalid. Please change your Config and try again!";

        

        private readonly UserInputValidator _userInputValidator;
        private readonly ConfigValidator _configValiator;
        private readonly IConfigDeserializer _configDeserializer;
        #endregion

        #region Constructors
        public MainMenu(
            UserInputValidator inputValidator,
            ConfigValidator configValidator,
            IConfigDeserializer configDeserializer
        )
        {
            _userInputValidator = inputValidator;
            _configDeserializer = configDeserializer;
            _configValiator = configValidator;
        }
        #endregion

        #region Functions
        public async Task Start(string[] args)
        {
            try
            {
                //greeting
                Console.WriteLine(MsgGreeting);
                Console.WriteLine(MsgUsedLibraries);

                //getting config
                string configPath = (args.Length > 0) ? args[0] : AskUserForConfigPath();
                ThesisPackerConfig config = await _configDeserializer.DeserializeConfig(configPath);
                Console.WriteLine(config.GitProjects.Select(it => it.KeepGitIntegration).PrettyPrint());
                Console.WriteLine(MsgDeserializedConfig);

                var configValidation = _configValiator.ValidateConfig(config);
                switch (configValidation)
                {
                    case ConfigValidator.ConfigValidationInfo.InvalidTargetDirectoryName:
                        Console.WriteLine(ErrInvalidConfigTargetDirectory);
                        break;
                    case ConfigValidator.ConfigValidationInfo.InvalidCodeDirectoryName:
                        Console.WriteLine(ErrInvalidConfigCodeDirectory);
                        break;
                    case ConfigValidator.ConfigValidationInfo.DuplicateFiles:
                        Console.WriteLine(ErrDuplicateFiles);
                        break;
                    case ConfigValidator.ConfigValidationInfo.DuplicateGitProjects:
                        Console.WriteLine(ErrDuplicateGitProjects);
                        break;
                    case ConfigValidator.ConfigValidationInfo.InvalidThesisPackName:
                        Console.WriteLine(ErrInvalidThesisPackName);
                        break;
                    case ConfigValidator.ConfigValidationInfo.Valid:
                        var thesisPacker = new BusinessLogicClerk();
                        await thesisPacker.Start(config, Console.WriteLine);
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            

        }
        #endregion

        #region Help Functions

        private string AskUserForConfigPath() => AskUserForInput(
            MsgPleaseEnterConfigPath, 
            ErrInvalidConfigPath,
            _userInputValidator.IsValidConfigFilePath
        );


        private string AskUserForInput(string inputMessage) => AskUserForInput(inputMessage, null, _ => true);
        private string AskUserForInput(string inputMessage, string? errorMessage, Func<string?,bool> onValidate)
        {
            Console.WriteLine(inputMessage);
            while (true)
            {
                string? userInput = Console.ReadLine();
                bool isValidInput = onValidate(userInput);
                if (isValidInput)
                {
                    return userInput ?? "";
                }
                Console.WriteLine(errorMessage);
            }
        }

        #endregion
    }
}
