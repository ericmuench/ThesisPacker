using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThesisPacker.Extensions;
using ThesisPacker.Files;
using ThesisPacker.Model;
using ThesisPacker.BusinessLogic;

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
                //getting config
                string configPath = (args.Length > 0) ? args[0] : AskUserForConfigPath();
                ThesisPackerConfig config = await _configDeserializer.DeserializeConfig(configPath);
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
