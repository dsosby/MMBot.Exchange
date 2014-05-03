using Common.Logging;
using Microsoft.Exchange.WebServices.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Async = System.Threading.Tasks;

namespace MMBot.Exchange
{
    public class ExchangeAdapter : Adapter
    {
        private ExchangeService _service;

        public ExchangeAdapter(ILog logger, string adapterId)
            : base(logger, adapterId)
        {
        }

        public override void Initialize(Robot robot)
        {
            base.Initialize(robot);
            Configure();
        }

        private void Configure()
        {
            string email = "dasosby@microsoft.com";
            string pass = "India;juliet";

            _service = new ExchangeService();
            _service.Credentials = new WebCredentials(email, pass);
            _service.AutodiscoverUrl(email, RedirectionUrlValidationCallback);
        }

        public override Async.Task Close()
        {
            Logger.Info("Closing Exchange adapter connection");
            //TODO: STop watching messages
        }

        public override Async.Task Run()
        {
            Logger.Info("Starting Exchange adapter connection");
            //TODO: Poll or setup push to watch for messages in inbox
        }

        //TODO: Send, Reply, Emote

        private static bool RedirectionUrlValidationCallback(string redirectionUrl)
        {
            return new Uri(redirectionUrl).Scheme == "https";
        }
    }
}
