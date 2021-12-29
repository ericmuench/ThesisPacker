using System.Collections.Generic;
using System.Linq;

namespace ThesisPacker.Model
{
    public class SerializedThesisPackerConfig
    {
        #region Fields
        public List<string> Files { get; set; }
        public string TargetFolder { get; set; }
        public List<SerializedGitProject> GitProjects { get; set; }
        #endregion

        #region Constructors
        public SerializedThesisPackerConfig(List<string> files, string targetFolder, List<SerializedGitProject> gitProjects)
        {
            TargetFolder = targetFolder;
            Files = files ?? new List<string>();
            GitProjects = gitProjects ?? new List<SerializedGitProject>();
        }

        public SerializedThesisPackerConfig() : this(new List<string>(), "", new List<SerializedGitProject>()) { }
        #endregion

        #region Functions
        public ThesisPackerConfig ToThesisPackerConfig() => new ThesisPackerConfig(
            Files, 
            TargetFolder, 
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
        #endregion

        #region Constructors
        public SerializedGitProject(string name, string url, List<string> ignoredBranches)
        {
            Name = name;
            Url = url;
            IgnoredBranches = ignoredBranches ?? new List<string>();
        }

        public SerializedGitProject(string name, string url) : this(name, url, new List<string>()) { }

        public SerializedGitProject() : this("", "", new List<string>()) { }
        #endregion

        #region Functions
        public GitProject ToGitProject() => new GitProject(this.Name, this.Url, this.IgnoredBranches);
        #endregion
    }
}
