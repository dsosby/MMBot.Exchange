using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MMBot.Exchange.Tests
{
    [TestClass]
    public class AdapterConfigTests
    {
        class FakeConfigurer : IRobotConfigurer
        {
            public Dictionary<string, string> Data { get; set; }

            public string GetConfigVariable(string configName)
            {
                if (Data == null) return null;
                if (!Data.ContainsKey(configName)) return null;
                return Data[configName];
            }
        }

        [TestMethod]
        public void DefaultValuesShouldBeSet()
        {
            var config = new AdapterConfig(new FakeConfigurer());

            Assert.AreEqual(true, config.UseOutlookStyle);
            Assert.AreEqual(true, config.TrimSignature);
            Assert.AreEqual(true, config.AllowImplicitCommand);
            Assert.AreEqual(5, config.MaxRetry);
            Assert.AreEqual(5, config.SubscriptionLifetime);
        }

        [TestMethod]
        public void ValuesCanBeConfigured()
        {
            var fake = new FakeConfigurer()
            {
                Data = new Dictionary<string, string>() {
                    {"MMBOT_EXCHANGE_EMAIL", "foobar@bizbaz.com"},
                    {"MMBOT_EXCHANGE_SUBSCRIPTIONLIFETIME", "30"},
                    {"MMBOT_EXCHANGE_USEOUTLOOKSTYLE", "false"}
                }
            };
            var config = new AdapterConfig(fake);

            Assert.AreEqual("foobar@bizbaz.com", config.Email);
            Assert.AreEqual(30, config.SubscriptionLifetime);
            Assert.AreEqual(false, config.UseOutlookStyle);
        }

        [TestMethod]
        public void ParseConfigSafelyConvertsNull()
        {
            Assert.AreEqual(false, AdapterConfig.ParseConfig<bool>(null));
        }

        [TestMethod]
        public void ParseConfigUsesDefault()
        {
            Assert.AreEqual("foobar", AdapterConfig.ParseConfig<string>(null, "foobar"));
        }

        [TestMethod]
        public void ParseConfigUsesTypeDefault()
        {
            Assert.AreEqual(0, AdapterConfig.ParseConfig<int>(null));
        }

        [TestMethod]
        public void ParseConfigHandlesEmptyString()
        {
            Assert.AreEqual(0, AdapterConfig.ParseConfig<int>(""));
        }

        [TestMethod]
        public void ParseConfigHandlesBadConversions()
        {
            Assert.AreEqual(5, AdapterConfig.ParseConfig<int>("not an int", 5));
        }
    }
}
