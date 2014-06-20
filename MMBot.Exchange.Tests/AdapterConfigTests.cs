using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace MMBot.Exchange.Tests
{
    [TestClass]
    public class AdapterConfigTests
    {
        [TestMethod]
        public void DefaultValuesShouldBeSet()
        {
            var robot = new Mock<Robot>();
            robot.Setup(r => r.GetConfigVariable(It.IsAny<string>()))
                .Returns((string)null);

            var config = new AdapterConfig(robot.Object);
            Assert.AreEqual(config.Email, string.Empty);
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
