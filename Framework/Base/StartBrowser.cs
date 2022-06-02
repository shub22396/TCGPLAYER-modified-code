using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using AventStack.ExtentReports;
using Framework.Config;
using Framework.Helpers;
using NUnit.Framework;
using OpenQA.Selenium;
using System.Threading;
using System;
using System.Collections;
using System.Reflection;
using System.IO;

namespace Framework.Base
{
    [TestFixture]
    public class StartBrowser
    {
        public static ThreadLocal<IWebDriver> driver = new ThreadLocal<IWebDriver>();


        public string profile;
        public string environment;
        public static ExtentTest parentTest;
        public static ExtentTest childTest;

        public static string seleniumUri = "https://hub.lambdatest.com:443/wd/hub";




        public StartBrowser() { }



        public StartBrowser(string profile, string environment)
        {
            this.profile = profile;
            this.environment = environment;

        }


        [SetUp]
        public void Init()
        {
        
            extent = new AventStack.ExtentReports.ExtentReports();
            parentTest = extent.CreateTest(TestContext.CurrentContext.Test.Name);

            //Getting the test Name
            var testname = NUnit.Framework.TestContext.CurrentContext.Test.Name;


            Console.Out.WriteLine("Profile==" + profile + "En");



           

            NameValueCollection caps = ConfigurationManager.GetSection("capabilities/" + profile) as NameValueCollection;
            NameValueCollection settings = ConfigurationManager.GetSection("environments/" + environment) as NameValueCollection;




          







            //DesiredCapabilities capability = new DesiredCapabilities();
            RemoteSessionSettings capability = new RemoteSessionSettings();
            Dictionary<string, object> ltOptions = new Dictionary<string, object>();

            foreach (string key in caps.AllKeys)
            {
               // capability.SetCapability(key, caps[key]);
                ltOptions.Add(key, caps[key]);

            }

            foreach (string key in settings.AllKeys)
                {
                //capability.SetCapability(key, settings[key]);

                ltOptions.Add(key, settings[key]);
                }

            String username = Environment.GetEnvironmentVariable("LT_USERNAME");
            if (username == null)
            {
                username = ConfigurationManager.AppSettings.Get("user");
                //capability.SetCapability("user", username);
                ltOptions.Add("user", username);
                Console.Out.WriteLine("Username" + username);
            }

            String accesskey = Environment.GetEnvironmentVariable("LT_ACCESS_KEY");
            if (accesskey == null)
            {
                accesskey = ConfigurationManager.AppSettings.Get("key");
                //capability.SetCapability("accessKey", accesskey);
                ltOptions.Add("accessKey", accesskey);
                Console.Out.WriteLine("AccessKey" + accesskey);
            }

            ltOptions.Add("name", testname);
            capability.AddMetadataSetting("LT:Options", ltOptions);

            Console.WriteLine("LTOPTIONSMATRIX----" + ltOptions);


            Console.WriteLine("CAPABILTYMATRIX----" + capability);


       

            driver.Value = new RemoteWebDriver(new Uri("http://" + ConfigurationManager.AppSettings.Get("server") + "/wd/hub/"), capability, TimeSpan.FromSeconds(600));

            
        }



        public StartBrowser(AventStack.ExtentReports.ExtentReports tempExtent)
        {
            extent = tempExtent;
        }

        AventStack.ExtentReports.ExtentReports extent;
        public void Setup(AventStack.ExtentReports.ExtentReports tempExtent, string url)
        {
            extent = tempExtent;
            ConfigReaders.LoadConfig();
            //Init();
            Console.WriteLine("Starting Tests");
            var file = JsonHelpers.GetJsonDataEnv("Urls.json");
            string testCaseUrl = file[url];
            StartBrowser.driver.Value.Url = testCaseUrl;
        }

        [TearDown]
        public void Cleanup()
        {
            bool passed = TestContext.CurrentContext.Result.Outcome.Status == NUnit.Framework.Interfaces.TestStatus.Passed;
            try
            {
                // Logs the result to LambdaTest
                ((IJavaScriptExecutor)driver.Value).ExecuteScript("lambda-status=" + (passed ? "passed" : "failed"));
            }
            finally
            {

                // Terminates the remote webdriver session
                driver.Value.Quit();
            }

        }
    }
}
