using System;
using System.Collections.Generic;
using System.Linq;
using ThesisPacker.Model;

namespace ThesisPacker
{
    class Program
    {
        static void Main(string[] args)
        {
            var testProj = new GitProject("Test", "https://gitlab.hof-university.de/Eric/sta_csharp_ws2020_21/-/tree/master/DJSets/DJSets");
            var testConfig = new ThesisPackerConfig(
                new List<string> { @"C:\Users\Eric\Documents\Hochschule Hof\WS 2021_22 (7)\projektarbeit_ws21_22\Praxisarbeit.pdf" }, 
                new List<GitProject> { testProj }
            );

            Console.WriteLine($"TestConfig: Files:{String.Join(",",testConfig.Files)}, Projects: {String.Join(",", testConfig.GitProjects.Select(it => it.Name))}");
            Console.ReadLine();
        }
    }
}
