# MMBot.Exchange

Harness the power of the [MMBot](http://github.com/mmbot/mmbot), a .Net port of Github's [HuBot](http://github.com/github/hubot) via your Exchange email account.

# Installation

Install MMBot.Exchange as you would any other MMBot adapter. Visit the [MMBot Readme](https://github.com/mmbot/mmbot#getting-started) for more information.

# Configuration

You'll need at least the following configuration parameters in your mmbot.ini file:

```
[EXCHANGE]
EMAIL=emailaccount@tomonitor.com
PASSWORD=your;email#passw0rd!
```

You can also optionally specify a URL instead of relying on the slow and sometimes unreliable autodiscovery service. This will help improve boot times as well!
If you don't know your URL, run MMBot.Exchange once and check the logs for the autodiscovered URL.

```
[EXCHANGE]
URL=https://mycompany.com/EWS/Exchange.asmx
```

# Things to Know

MMBot.Exchange relies on the Exchange Web Service and uses streaming notifications (aka push) to monitor the account's inbox.
Since email is a slightly different medium than the traditional bot chatroom, some changes have been made.

* If the bot is the only recipient, responses do not have to be preceeded by the bot name, e.g. an email to the bot with a simple "Ping" message body will suffice instead of "MMBot, Ping"
* Signatures are to be avoided, thus the MMBot.Exchange adapter only reads until the first blank line of the message
* Subjects are (currently) ignored
* The bot "replies all". Add him to long group conversations for fun!
* The bot replies using HTML - feel free to take advantage of this in your scripts.