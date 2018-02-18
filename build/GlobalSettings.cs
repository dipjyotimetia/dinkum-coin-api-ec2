using System;
using System.Collections.Generic;
using System.Text;
using static Nuke.Core.IO.PathConstruction;

namespace Build
{
    public class GlobalSettings
    {

        public string JiraXrayUri { get; set; }
        public string NugetToolPath { get; set; }

        public GlobalSettings()
        {
            JiraXrayUri = "https://jira.beteasy.be/rest/raven/1.0/import/execution/nunit?projectKey=IGN";
            NugetToolPath = "C:/nuget/nuget.exe";
        }
    }
}
