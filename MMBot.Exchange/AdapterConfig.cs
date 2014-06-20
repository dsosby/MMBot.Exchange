using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMBot.Exchange
{
    public class AdapterConfig
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
            TrimSignature = GetConfig<bool>("MMBOT_EXCHANGE_TRIMSIGNATURE", true);
            AllowImplicitCommand = GetConfig<bool>("MMBOT_EXCHANGE_ALLOWIMPLICITCOMMAND", true);
            UseOutlookStyle = GetConfig<bool>("MMBOT_EXCHANGE_USEOUTLOOKSTYLE", true);
            MaxRetry = GetConfig<int>("MMBOT_EXCHANGE_MAXRETRY", 5);

            //TODO: Folder? Subject filter? From domain filter? Subscription timeout?
        }

        public static T GetConfig<T>(string configValue, T defaultValue = default(T))
        {
            if (configValue == null) return defaultValue;

            try
            {
                var converter = TypeDescriptor.GetConverter(typeof(T));
                if (converter != null)
                {
                    return (T)converter.ConvertFromString(configValue);
                }
            }
            catch (Exception) { }

            return defaultValue;
        }
    }
}
