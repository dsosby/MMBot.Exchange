using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMBot.Exchange
{
    public class ExchangeAdapter : Adapter
    {
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
            //TODO: Configuration
        }

        public override Task Close()
        {
            Logger.Info("Closing Exchange adapter connection");
            throw new NotImplementedException();
        }

        public override Task Run()
        {
            Logger.Info("Starting Exchange adapter connection");
            throw new NotImplementedException();
        }

        //TODO: Send, Reply, Emote
    }
}
