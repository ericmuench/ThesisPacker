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

        private const string ErrInvalidConfigPath =
            "The specified Path seems not to be valid or does not exist. Please enter the File-Path to your prefered Config.yaml File:";


        private readonly UserInputValidator _userInputValidator;
        private readonly IConfigDeserializer _configDeserializer;
        #endregion

        #region Constructors
        public MainMenu(
            UserInputValidator inputValidator,
            IConfigDeserializer configDeserializer
        )
        {
            _userInputValidator = inputValidator;
            _configDeserializer = configDeserializer;
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
                var thesisPacker = new BusinessLogicClerk();
                await thesisPacker.Start(config, Console.WriteLine);
                //TODO Delete this comment
                /*Console.WriteLine(config.Files.PrettyPrint());
                Console.WriteLine(config.TargetFolder);
                Console.WriteLine(config.GitProjects.Select(it => $"{it.Name} ({it.IgnoredBranches.Count} ignored branches)").PrettyPrint());*/
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
