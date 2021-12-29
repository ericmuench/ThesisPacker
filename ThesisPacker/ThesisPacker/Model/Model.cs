using System.Collections.Generic;

namespace ThesisPacker.Model
{
    public class ThesisPackerConfig
    {
        #region Fields
        public List<string> Files { get; }
        public List<GitProject> GitProjects { get; }
        #endregion

        #region Constructors
        public ThesisPackerConfig(List<string> files, List<GitProject> gitProjects)
        {
            Files = files;
            GitProjects = gitProjects;
        }
        #endregion
    }

    public class GitProject
    {
        #region Fields
        public string Name { get; }
        public string Url { get; }
        public List<string> IgnoredBranches { get; }
        #endregion

        #region Constructors
        public GitProject(string name, string url, List<string> ignoredBranches)
        {
            Name = name;
            Url = url;
            IgnoredBranches = ignoredBranches;
        }

        public GitProject(string name, string url) : this(name, url, new List<string>()) { }
        #endregion
    }
}
