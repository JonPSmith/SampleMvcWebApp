using NUnit.Framework;
using SampleWebApp.ActionProgress;
using Tests.Helpers;

namespace Tests.UnitTests.Group08ActionRunner
{
    class Test02ActionConfig
    {

        [Test]
        public void Test01BaseConfigJsonEncodeParametersIn()
        {
            //SETUP
            var config = new ActionConfig(); 

            //ATTEMPT
            var json = config.SerialiseToJsonIndentedUsingJsonNet();

            //VERIFY
            json.AssertJsonPropertyPresentAndReturnValue(config, x => x.IndeterminateLength).ShouldEqual("false");
            json.AssertJsonPropertyPresentAndReturnValue(config, x => x.ActionEndsAt).ShouldEqual("\"CurrentState\"");
        }

        [Test]
        public void Test02BaseConfigJsonEncodeParametersOut()
        {
            //SETUP
            var config = new ActionConfig();

            //ATTEMPT
            var json = config.SerialiseToJsonIndentedUsingJsonNet();

            //VERIFY
            json.AssertJsonPropertyNotPresent(config, x => x.HeaderText);
            json.AssertJsonPropertyNotPresent(config, x => x.SuccessExitUrl);
        }

        [Test]
        public void Test05CheckNunNullStringIncludedIn()
        {
            //SETUP
            var config = new ActionConfig
            {
                HeaderText = "XXX",
                SuccessExitUrl = "YYY",
            };

            //ATTEMPT
            var json = config.SerialiseToJsonIndentedUsingJsonNet();

            //VERIFY
            json.AssertJsonPropertyPresentAndReturnValue(config, x => x.HeaderText).ShouldEqual("\"XXX\"");
            json.AssertJsonPropertyPresentAndReturnValue(config, x => x.SuccessExitUrl).ShouldEqual("\"YYY\"");
        }

        [Test]
        public void Test10CtorWithParam()
        {
            //SETUP
            var config = new ActionConfig("url goes here");


            //ATTEMPT
            var json = config.SerialiseToJsonIndentedUsingJsonNet();

            //VERIFY
            json.AssertJsonPropertyPresentAndReturnValue(config, x => x.SuccessExitUrl).ShouldEqual("\"url goes here\"");
        }

    }
}
