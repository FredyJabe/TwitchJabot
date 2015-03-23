using System;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;

/*
* Class that sends PING to irc server every 15 seconds
*/
class PingSender
{
    static string PING = "PING :";
    private Thread pingSender;
    private Thread payGiver;
    // Empty constructor makes instance of Thread
    public PingSender()
    {
        pingSender = new Thread(new ThreadStart(this.Run));
        payGiver = new Thread(new ThreadStart(this.Pay));
    }

    // Starts the thread
    public void Start()
    {
        pingSender.Start();
        payGiver.Start();
    }

    // Send PING to irc server every minutes
    public void Run()
    {
        while (true)
        {
            foreach(User us in Users.list)
            {
                us.watchTime ++;
                
                XmlProcess.Write("xml\\users.xml","User",us.name,us.admin.ToString(),us.currency.ToString(),us.watchTime.ToString(),us.hp.ToString(),us.def.ToString(),us.atk.ToString(),us.rep.ToString());
            }
            
            IrcBot.Send(PING + IrcBot.SERVER);
            Thread.Sleep (60000);
        }
    }
    
    // Gives the payout at each intervals
    public void Pay()
    {
        while (true)
        {
            Thread.Sleep(IrcBot.currencyInterval * 60000);
            
            foreach(User us in Users.list)
            {
                us.currency += IrcBot.currencyPayout;
            }
            IrcBot.writeLine("Payout !! +"+IrcBot.currencyPayout+" to everyone", ConsoleColor.White);
            IrcBot.Send("PRIVMSG " + IrcBot.CHANNEL + " :/me +"+IrcBot.currencyPayout+" "+IrcBot.oneCurrency+" for everyone!");
        }
    }
}