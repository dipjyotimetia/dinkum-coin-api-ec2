using Nuke.Core;
using Nuke.Core.Tooling;


namespace Build.Settings
{
    public class GatlingSettings : ToolSettings
    {
        public override string ToolPath { get {
                if (EnvironmentInfo.IsWin)
                    return "C:/test-tools/gatling-2.3.0/bin/gatling.bat";
                else
                    return "/opt/gatling/bin/gatling.sh";
            } }

    }
}
