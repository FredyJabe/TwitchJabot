using System;
using System.Threading;

class userInput
{
	private Thread input;
    public userInput()
    {
        input = new Thread(new ThreadStart(this.run));
    }
    
    public void start()
    {
        input.Start();
    }
    
    public void run()
    {
        while(true)
        {
            string command = Console.ReadLine();
			
			IrcBot.writer.WriteLine("PRIVMSG " + IrcBot.CHANNEL + " :" + command);
			IrcBot.writer.Flush();
        }
    }
}