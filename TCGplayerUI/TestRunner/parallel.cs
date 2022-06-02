using NUnit.Framework;
using OpenQA.Selenium;
using TCGplayerUI.TestCases;
namespace lambdatest
{
    [TestFixture("parallel", "firefox")]
    [TestFixture("parallel", "chrome")]
    [TestFixture("parallel", "chrome")]
    [Parallelizable(ParallelScope.Fixtures)]
    public class ParallelTest : LoginTest
    {
        public ParallelTest(string profile, string environment) : base(profile, environment) { }
    }
}