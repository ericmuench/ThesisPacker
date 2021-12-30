using System.Collections.Generic;
using System.Linq;

namespace ThesisPacker.Model
{
    public class SerializedThesisPackerConfig
    {
        #region Fields
        public List<string> Files { get; set; }
        public string TargetDirectory { get; set; }
        public string CodeDirectoryName { get; set;  }
        public List<SerializedGitProject> GitProjects { get; set; }
        #endregion

        #region Constructors
        public SerializedThesisPackerConfig(List<string> files, string targetDirectory, string codeDirName, List<SerializedGitProject> gitProjects)
        {
            TargetDirectory = targetDirectory;
            CodeDirectoryName = codeDirName;
            Files = files ?? new List<string>();
            GitProjects = gitProjects ?? new List<SerializedGitProject>();
        }

        public SerializedThesisPackerConfig() : this(new List<string>(), "", "",new List<SerializedGitProject>()) { }
        #endregion

        #region Functions
        public ThesisPackerConfig ToThesisPackerConfig() => new ThesisPackerConfig(
            Files, 
            TargetDirectory, 
            CodeDirectoryName,
            GitProjects.Select(it => it.ToGitProject()).ToList()
        );
        #endregion
    }

    public class SerializedGitProject
    {
        #region Fields
        public string Name { get; set; }
        public string Url { get; set; }
        public List<string> IgnoredBranches { get; set; }
        public SerializedGitCredentials GitCredentials { get; set; }
        #endregion

        #region Constructors
        public SerializedGitProject(string name, string url, List<string> ignoredBranches, SerializedGitCredentials credentials)
        {
            Name = name;
            Url = url;
            GitCredentials = credentials;
            IgnoredBranches = ignoredBranches ?? new List<string>();
        }

        public SerializedGitProject(string name, string url, SerializedGitCredentials credentials) : this(name, url, new List<string>(), credentials) { }

        public SerializedGitProject() : this("", "", new List<string>(), new SerializedGitCredentials()) { }
        #endregion

        #region Functions
        public GitProject ToGitProject() => new GitProject(this.Name, this.Url, this.IgnoredBranches, this.GitCredentials.ToGitCredentials());
        #endregion
    }

    public class SerializedGitCredentials
    {
        #region Fields
        public string UserName { get; set; }
        public string Password { get; set; }
        #endregion

        #region Constructors
        public SerializedGitCredentials(string userName, string password)
        {
            UserName = userName;
            Password = password;
        }

        public SerializedGitCredentials() : this("", "") { }
        #endregion

        #region Functions
        public GitCredentials ToGitCredentials() => new GitCredentials(UserName, Password);
        #endregion
    }
}
