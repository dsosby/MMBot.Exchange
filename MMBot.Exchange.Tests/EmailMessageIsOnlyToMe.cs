using System;
using System.Linq;
using Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Exchange.WebServices.Data;
using System.Collections.Generic;

namespace MMBot.Exchange.Tests
{
    [TestClass]
    public class EmailMessageIsOnlyToMeTests
    {
        private static string[] emailsStrings = new string[] {
            "foobar@bizbaz.com",
            "biz-123@biz.baz.uk",
            "santa@claus.org"
        };

        private static EmailAddress[] emails = emailsStrings
            .Select(e => new EmailAddress(e))
            .ToArray();

        [TestMethod]
        public void IsOnlyToMeShouldBeFalseWhenOthersArePresent()
        {
            var service = new Mock<ExchangeService>();
            var email = new EmailMessage(service.Object);
            email.ToRecipients.AddRange(emails);
            Assert.IsFalse(email.IsOnlyTo("santa@claus.org"));
        }
    }
}
