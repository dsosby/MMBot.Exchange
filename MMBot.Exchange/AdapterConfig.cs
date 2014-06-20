using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMBot.Exchange
{
    class AdapterConfig
    {
        /// <summary>
        /// Get or set the email address of the bot account
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Get or set the password of the bot account
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Get or set the URI to the Exchange Web Service
        /// </summary>
        public string ExchangeUrl { get; set; }

        /// <summary>
        /// Get or set a flag that email signatures should be clipped
        /// </summary>
        public bool TrimSignature { get; set; }

        /// <summary>
        /// Get or set a flag that allows received emails where
        /// MMBot is the only recipient to imply the "MMBot " prefix
        /// </summary>
        public bool AllowImplicitCommand { get; set; }

        /// <summary>
        /// Get or set flag to emulate Outlook message style
        /// </summary>
        public bool UseOutlookStyle { get; set; }

        /// <summary>
        /// Get or set the number of times to retry a broken EWS subscription
        /// </summary>
        public int MaxRetry { get; set; }

        public AdapterConfig(Robot robot)
        {
            Email = robot.GetConfigVariable("MMBOT_EXCHANGE_EMAIL");
            Password = robot.GetConfigVariable("MMBOT_EXCHANGE_PASSWORD");
            ExchangeUrl = robot.GetConfigVariable("MMBOT_EXCHANGE_URL");
            TrimSignature = GetBooleanConfig("MMBOT_EXCHANGE_TRIMSIGNATURE", true);
            AllowImplicitCommand = GetBooleanConfig("MMBOT_EXCHANGE_ALLOWIMPLICITCOMMAND", true);
            UseOutlookStyle = GetBooleanConfig("MMBOT_EXCHANGE_USEOUTLOOKSTYLE", true);
            MaxRetry = GetIntegerConfig("MMBOT_EXCHANGE_MAXRETRY", 5);

            //TODO: Folder? Subject filter? From domain filter? Subscription timeout?
        }

        private bool GetBooleanConfig(string configValue, bool defaultValue)
        {
            bool value;
            var success = Boolean.TryParse(configValue ?? "", out value);
            return success ? value : defaultValue;
        }

        private int GetIntegerConfig(string configValue, int defaultValue)
        {
            int value;
            var success = int.TryParse(configValue ?? "", out value);
            return success ? value : defaultValue;
        }
    }
}
