using System;
using System.Collections.Generic;
using System.Linq;
using SystemTask = System.Threading.Tasks;
using Common.Logging;
using Microsoft.Exchange.WebServices.Data;

namespace MMBot.Exchange
{
    public class ExchangeAdapter : Adapter
    {
        private string Email { get; set; }
        private string Password { get; set; }
        private string ExchangeUrl { get; set; }
        private bool TrimSignature { get; set; }
        private bool AllowImplicitCommand { get; set; }
        private bool UseOutlookStyle { get; set; }
        private int MaxRetry { get; set; }

        private PropertySet EmailProperties { get; set; }

        private ExchangeService Service { get; set; }

        private StreamingSubscriptionConnection ExchangeConnection { get; set; }

        private bool IsRunning { get; set; }

        private List<EmailMessage> Messages { get; set; }

        private int retryCount;


        public ExchangeAdapter(ILog logger, string adapterId)
            : base(logger, adapterId)
        {
            Messages = new List<EmailMessage>();

            EmailProperties = new PropertySet(
                ItemSchema.Id,
                ItemSchema.Subject,
                ItemSchema.UniqueBody,
                ItemSchema.IsFromMe,
                ItemSchema.DateTimeReceived,
                EmailMessageSchema.From,
                EmailMessageSchema.ToRecipients
            );
            EmailProperties.RequestedBodyType = BodyType.Text;
        }

        public override void Initialize(Robot robot)
        {
            base.Initialize(robot);
            Configure();

            if (string.IsNullOrEmpty(Email) ||
                string.IsNullOrEmpty(Password))
            {
                Logger.Warn("Exchange Adapter requires MMBOT_EXCHANGE_EMAIL and MMBOT_EXCHANGE_PASSWORD");
                return;
            }

            Service = new ExchangeService
            {
                Credentials = new WebCredentials(Email, Password)
            };

            InitializeExchangeUrl();

            var newMailSubscription = Service.SubscribeToStreamingNotifications(
                new FolderId[] { WellKnownFolderName.Inbox },
                EventType.NewMail);

            ExchangeConnection = new StreamingSubscriptionConnection(Service, 30);
            ExchangeConnection.AddSubscription(newMailSubscription);
            ExchangeConnection.OnNotificationEvent += OnExchangeNotification;
            ExchangeConnection.OnDisconnect += OnExchangeDisconnect;
        }

        private void InitializeExchangeUrl()
        {
            if (string.IsNullOrEmpty(ExchangeUrl))
            {
                Logger.Info("Autodiscovering Exchange service url...");
                Service.AutodiscoverUrl(Email);
            }
            else
            {
                Service.Url = new System.Uri(ExchangeUrl);
            }

            Logger.Info("Exchange service url is " + Service.Url);
        }

        private void Configure()
        {
            Email = Robot.GetConfigVariable("MMBOT_EXCHANGE_EMAIL");
            Password = Robot.GetConfigVariable("MMBOT_EXCHANGE_PASSWORD");
            ExchangeUrl = Robot.GetConfigVariable("MMBOT_EXCHANGE_URL");
            TrimSignature = GetBooleanConfig("MMBOT_EXCHANGE_TRIMSIGNATURE", true);
            AllowImplicitCommand = GetBooleanConfig("MMBOT_EXCHANGE_ALLOWIMPLICITCOMMAND", true);
            UseOutlookStyle = GetBooleanConfig("MMBOT_EXCHANGE_USEOUTLOOKSTYLE", true);
            MaxRetry = GetIntegerConfig("MMBOT_EXCHANGE_MAXRETRY", 5);

            //TODO: Folder? Subject filter? From domain filter? Subscription timeout?
        }

        private bool GetBooleanConfig(string name, bool defaultValue)
        {
            bool value;
            var success = Boolean.TryParse(Robot.GetConfigVariable(name) ?? "", out value);
            return success ? value : defaultValue;
        }

        private int GetIntegerConfig(string name, int defaultValue)
        {
            int value;
            var success = int.TryParse(Robot.GetConfigVariable(name) ?? "", out value);
            return success ? value : defaultValue;
        }

        private void OnExchangeDisconnect(object sender, SubscriptionErrorEventArgs args)
        {
            bool isRecoverable = args.Exception == null;

            if (IsRunning && isRecoverable)
            {
                Logger.Info("Restarting Exchange subscription");
                ExchangeConnection.Open();

                // We don't have a connected event, so we'll assume if we ended w/o
                // error we were in a good connection previously.
                retryCount = 0;
            }
            else
            {
                Logger.Info("Exchange service disconnected: " + IsRunning + " " + args.Exception);
                if (retryCount < MaxRetry)
                {
                    retryCount++;
                    Logger.Info("Attempting Exchange reconnect: " + retryCount + " of " + MaxRetry);
                    ExchangeConnection.Open();
                }
            }
        }

        private void OnExchangeNotification(object sender, NotificationEventArgs args)
        {
            var service = args.Subscription.Service;
            var emailIds = args.Events
                .OfType<ItemEvent>()
                .Select(i => i.ItemId);
            var emails = service.BindToItems(emailIds, EmailProperties)
                .Select(r => r.Item as EmailMessage)
                .ToList();

            SaveMessages(emails);
            foreach (var email in emails) ProcessMessage(email);
        }

        private void ProcessMessage(EmailMessage message)
        {
            var user = Robot.GetUser(
                message.From.Address,
                message.From.Name,
                message.Id.UniqueId,
                Id);

            var messageBody = message.UniqueBody.Text.Trim();
            if (TrimSignature) messageBody = TrimSignatureFromBody(messageBody);

            if (AllowImplicitCommand && message.IsOnlyTo(Email))
            {
                if (!messageBody.StartsWith(Robot.Name, StringComparison.InvariantCultureIgnoreCase))
                {
                    messageBody = Robot.Name + ", " + messageBody;
                }
            }

            //TODO: Try and add spoofed address detection

            Logger.Info(string.Format("Received message from {0}: {1}", user.Id, messageBody));
            if (string.IsNullOrEmpty(messageBody))
            {
                Logger.Info("Skipping empty message: " + message.Subject);
                return;
            }

            var robotMessage = new TextMessage(user, messageBody);
            SystemTask.Task.Run(() => Robot.Receive(robotMessage));
        }

        // Returns body up to the first blank line
        private string TrimSignatureFromBody(string body)
        {
            var firstLines = body
                .Split('\n')
                .TakeWhile(line => !string.IsNullOrWhiteSpace(line));
            return string.Join("\n", firstLines).Trim();
        }

        private void SaveMessages(IEnumerable<EmailMessage> incomingMessages)
        {
            //Only save messages for an hour or so
            Messages.AddRange(incomingMessages);
            Messages = Messages
                .Where(m => m.DateTimeReceived > DateTime.Now.AddHours(-1))
                .ToList();
        }

        public override async SystemTask.Task Send(Envelope envelope, params string[] messages)
        {
            if (messages == null || !messages.Any()) return;

            var replyToId = envelope.User.Room;
            var replyTo = Messages.FirstOrDefault(m => m.Id.UniqueId == replyToId);

            if (replyTo == null)
            {
                Logger.Info("Could not find parent message for " + replyToId);
                return;
            }

            var response = string.Join("<br>", messages);
            response = response.Replace(Environment.NewLine, "<br>");
            if (UseOutlookStyle) response = WrapInOutlookStyle(response);
            //TODO: Embed local images as inline attachments

            Logger.Info(string.Format("Replying to {0}: {1}", replyTo.From.Name, response));
            replyTo.Reply(response, replyAll: true);
        }

        private static string WrapInOutlookStyle(string message)
        {
            //TODO: Basic reply appends an <hr> which is ugly...need to style our own using border:0;border-top:1px solid #e1e1e1;
            return string.Format("<div style='font-family: Calibri,sans-serif; font-size: 11pt;'>{0}</div>", message);
        }

        public override async SystemTask.Task Run()
        {
            if (Service == null) return;

            IsRunning = true;
            ExchangeConnection.Open();
        }

        public override async SystemTask.Task Close()
        {
            if (Service == null) return;

            IsRunning = false;
            ExchangeConnection.Close();
        }
    }
}
