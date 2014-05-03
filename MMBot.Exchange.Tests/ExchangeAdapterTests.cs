using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MMBot.Exchange.Tests
{
    [TestClass]
    public class ExchangeAdapterTests
    {
        [TestMethod]
        public void CanInitializeWithNullRobot()
        {
            var adapter = new MMBot.Exchange.ExchangeAdapter(null, "");
            adapter.Initialize(null);
        }
    }
}
