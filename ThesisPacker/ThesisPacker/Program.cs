using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ThesisPacker.Files;
using ThesisPacker.Model;
using ThesisPacker.UserInteraction;

namespace ThesisPacker
{
    class Program
    {
        static async Task Main(string[] args)
        {
            MainMenu mainMenu = new MainMenu(new UserInputValidator(), new YamlConfigDeserializer());
            await mainMenu.Start(args);
        }
    }
}
