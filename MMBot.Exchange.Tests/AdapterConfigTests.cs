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
        public void GetConfigSafelyConvertsNull()
        {
            Assert.AreEqual(false, AdapterConfig.GetConfig<bool>(null));
        }

        [TestMethod]
        public void GetConfigUsesDefault()
        {
            Assert.AreEqual("foobar", AdapterConfig.GetConfig<string>(null, "foobar"));
        }

        [TestMethod]
        public void GetConfigUsesTypeDefault()
        {
            Assert.AreEqual(0, AdapterConfig.GetConfig<int>(null));
        }

        [TestMethod]
        public void GetConfigHandlesEmptyString()
        {
            Assert.AreEqual(0, AdapterConfig.GetConfig<int>(""));
        }

        [TestMethod]
        public void GetConfigHandlesBadConversions()
        {
            Assert.AreEqual(5, AdapterConfig.GetConfig<int>("not an int", 5));
        }
    }
}
