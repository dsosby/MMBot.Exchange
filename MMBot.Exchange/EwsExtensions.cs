using Microsoft.Exchange.WebServices.Data;

namespace MMBot.Exchange
{
    public static class EmailMessageExtensions
    {
        public static bool IsOnlyTo(this EmailMessage message, string address)
        {
            return message != null &&
                   message.ToRecipients.Count == 1 &&
                   message.ToRecipients[0].Address == address;
        }
    }
}
