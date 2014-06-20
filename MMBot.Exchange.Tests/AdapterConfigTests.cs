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
    }
}
