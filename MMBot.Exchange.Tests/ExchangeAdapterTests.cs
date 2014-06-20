using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MMBot.Exchange.Tests
{
    [TestClass]
    public class ExchangeAdapterTests
    {
        MMBot.Exchange.ExchangeAdapter adapter = new MMBot.Exchange.ExchangeAdapter(null, "");

        public ExchangeAdapterTests()
        {
            adapter = new MMBot.Exchange.ExchangeAdapter(null, "");
        }

        //TODO: Tests
    }
}
