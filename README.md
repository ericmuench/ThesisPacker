# ThesisPacker
This Project is a console application that allows creating a zipped version of a Bachelor Thesis including the given files and all Git-Projects specified. You can use a YAML-File to specify the files and directories included, ignored files and the name for the config and the target directory. Additionally you have the ability to use GitProjects to clone several Repositories

## Installation (Windows)
1. Download the Application Files as a zip file [here](https://github.com/ericmuench/ThesisPacker/raw/main/App/ThesisPacker.zip)
2. Unzip the file in a Folder of your choice
3. Create a `Config.yaml` File for your Thesis Configuration. You can find a template for this [here](https://github.com/ericmuench/ThesisPacker/blob/main/Templates/Config.yaml).
4. Run ThesisPacker.exe and specify the Path of your Config file (e.g. `C:\configs\Config.yaml`).
5. Have fun :smile:

## Library Usages
This Project uses [LibGit2Sharp](https://github.com/libgit2/libgit2sharp) and [YamlDotNet](https://github.com/aaubry/YamlDotNet). The given copyright-notices are shown when starting the program.
