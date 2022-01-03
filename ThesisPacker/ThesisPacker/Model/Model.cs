﻿using System;
using System.Collections.Generic;

#nullable enable
namespace ThesisPacker.Model
{
    public class ThesisPackerConfig
    {
        #region Fields
        public List<string> Files { get; }
        public List<string> IgnoredFiles { get; set; }
        public string TargetDirectory { get; }
        public string CodeDirectoryName { get; }
        public List<GitProject> GitProjects { get; }
        #endregion

        #region Constructors
        public ThesisPackerConfig(List<string> files, List<string> ignoredFiles, string targetDirectory, string codeDirName, List<GitProject> gitProjects)
        {
            TargetDirectory = targetDirectory;
            CodeDirectoryName = codeDirName;
            Files = files;
            IgnoredFiles = ignoredFiles;
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
        public GitCredentials? GitCredentials { get; }
        #endregion

        #region Constructors
        public GitProject(string name, string url, List<string> ignoredBranches, GitCredentials? credentials)
        {
            Name = name;
            Url = url;
            IgnoredBranches = ignoredBranches;
            GitCredentials = credentials;
        }

        public GitProject(string name, string url, GitCredentials? credentials) : this(name, url, new List<string>(), credentials) { }
        #endregion
    }

    public class GitCredentials
    {
        #region Fields
        public string UserName { get; }
        public string Password { get; }
        #endregion

        #region Constructors
        public GitCredentials(string userName, string password)
        {
            UserName = userName;
            Password = password;
        }
        #endregion
    }
}
