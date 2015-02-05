using System.Collections;
using System.Collections.Generic;
using System.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FluentShellUnit.Tests
{
    [TestClass]
    [DeploymentItem(@"Data\TestModule.psd1", "TestModule")]
    [DeploymentItem(@"Data\WebApplication.ps1")]
    [DeploymentItem(@"Data\Modules\SampleModule.psm1", @"TestModule\Modules")]
    [DeploymentItem(@"Data\Modules\TestModule.psm1", @"TestModule\Modules")]
    public class PsHostTests
    {
        [TestMethod]
        [TestCategory("Framework UnitTest")]
        public void Execute_should_load_and_invoke_shell_with_params()
        {
            var actual = PsFactory.Create(HostState.Core)
                .FailOnNonTerminatingError()
                .Load(@"TestModule\Modules\TestModule.psm1")
                .StubFromFile("WebApplication.ps1")
                .Execute
                (
                    "Get-WelcomeMessage",
                    new Dictionary<string, string>
                    {
                        {"context", "VSTest"}
                    }
                );
            Assert.IsTrue(actual.FirstResultItemAs<Hashtable>().Contains("Url"));
        }

        [TestMethod]
        [TestCategory("Framework UnitTest")]
        public void SetGlobalVariable_sets_a_global_variable_in_scope()
        {
            var actual = PsFactory.Create(HostState.Core).SetGlobalVariable("test", "test1");
            var result = actual.LoadScriptBlock("return $test")
                .Execute().FirstResultItemAs<string>();
            Assert.IsTrue(result == "test1");
        }

        [TestMethod]
        [TestCategory("Framework UnitTest")]
        [DeploymentItem(@"Data\GlobalVariableTest.ps1")]
        [DeploymentItem(@"Data\ScriptTest.ps1")]
        [DeploymentItem(@"Data\TestXml.xml")]
        public void LoadScriptFromFile_should_be_able_to_set_variable()
        {
            var actual = PsFactory.Create(HostState.Core)
                .LoadScriptFromFile("GlobalVariableTest.ps1")
                .Execute()
                .LoadScriptFromFile("ScriptTest.ps1")
                .Execute()
                .FirstResultItemAs<XmlDocument>();
            Assert.IsTrue(actual != null);
        }



        [TestMethod]
        [TestCategory("Framework UnitTest")]
        [DeploymentItem(@"Data\GlobalVariableTest.ps1")]
        [DeploymentItem(@"Data\ScriptTest.ps1")]
        [DeploymentItem(@"Data\TestXml.xml")]
        public void Execute_should_invoke_method_on_module_after_scriptblock_execution()
        {
            var actual = PsFactory.Create(HostState.Core)
                .Load(@"TestModule\Modules\TestModule.psm1")
                .StubFromFile("WebApplication.ps1")
                .LoadScriptFromFile("GlobalVariableTest.ps1")
                .Execute
                (
                    "Get-WelcomeMessage",
                    new Dictionary<string, string>
                    {
                        {"context", "VSTest"}
                    }
                )
                .FirstResultItemAs<Hashtable>();
            Assert.IsTrue(actual.Contains("Url"));
        }
    }
}
