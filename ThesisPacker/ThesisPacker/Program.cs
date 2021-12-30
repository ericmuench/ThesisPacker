using System.Threading.Tasks;
using ThesisPacker.Files;
using ThesisPacker.UserInteraction;

namespace ThesisPacker
{
    class Program
    {
        static async Task Main(string[] args)
        {
            MainMenu mainMenu = new MainMenu(new UserInputValidator(), new ConfigValidator(), new YamlConfigDeserializer());
            await mainMenu.Start(args);
        }
    }
}
