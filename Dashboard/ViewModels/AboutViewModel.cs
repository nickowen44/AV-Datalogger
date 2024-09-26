using System;

namespace Dashboard.ViewModels;

public class AboutViewModel : ViewModelBase
{
    public static string GitCommit => ThisAssembly.Git.Commit;
    public static string GitBranch => ThisAssembly.Git.Branch;

    public static string CurrentYear => DateTime.Now.Year.ToString();
}