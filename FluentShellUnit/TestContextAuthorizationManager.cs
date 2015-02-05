using System;
using System.Management.Automation;
using System.Management.Automation.Host;

namespace FluentShellUnit
{
    //The authorization manager helps control the execution of commands for the runspace. 
    //The Windows PowerShell runtime requests permission from the authorization manager to run the command types defined by the CommandTypes enumeration. 
    //The default implementation of this class gives permission for the runspace to run all types of commands.
    //We use a custom implementation of the authorizationmanager in our tests and override the default behaviour and explicitly sets the ShouldRun value to true always
    internal class TestContextAuthorizationManager : AuthorizationManager
    {
        public TestContextAuthorizationManager(string shellId) : base(shellId)
        {
        }

        protected override bool ShouldRun(CommandInfo commandInfo, CommandOrigin origin, PSHost host,
            out Exception reason)
        {
            base.ShouldRun(commandInfo, origin, host, out reason);
            return true;
        }
    }
}