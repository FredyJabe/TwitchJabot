//css_import D:\Programmes utiles\Notepad++\Projects\IRC game bot\input.cs
//css_import D:\Programmes utiles\Notepad++\Projects\IRC game bot\pingSender.cs
//css_import D:\Programmes utiles\Notepad++\Projects\IRC game bot\commands.cs
//css_import D:\Programmes utiles\Notepad++\Projects\IRC game bot\users.cs
//css_import D:\Programmes utiles\Notepad++\Projects\IRC game bot\xmlProcess.cs
//css_import D:\Programmes utiles\Notepad++\Projects\IRC game bot\giveAways.cs

using System;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;

/* 
* This program establishes a connection to irc server, joins a channel and greets every nickname that
* joins the channel.
* 
* Coded by Pasi Havia 17.11.2001 http://koti.mbnet.fi/~curupted
*/ 
class IrcBot
{
    #region Bot Infos
    // Irc server to connect 
    public static string SERVER = "irc.twitch.tv";
    // Irc server's port (6667 is default port)
    private static int PORT = 443;
    // User information defined in RFC 2812 (Internet Relay Chat: Client Protocol) is sent to irc server
    private static string PASS = XmlProcess.Read("xml\\config.xml","Bot", "bot","oauth");
    private static string NICK = XmlProcess.Read("xml\\config.xml","Bot", "bot","botName");
    public static string CHANNEL = XmlProcess.Read("xml\\config.xml","Bot", "bot","channel");
    
    public static string oneCurrency = XmlProcess.Read("xml\\config.xml", "Currency", "Currency", "onename");
    public static string multiCurrency = XmlProcess.Read("xml\\config.xml", "Currency", "Currency", "multiname");
    public static int currencyInterval = int.Parse(XmlProcess.Read("xml\\config.xml", "Currency", "Currency", "interval"));
    public static int currencyPayout = int.Parse(XmlProcess.Read("xml\\config.xml", "Currency", "Currency", "payout"));
    
    public static bool RPG = false;
    public static bool activeGiveAway = false;
    
    #endregion
    // StreamWriter is declared here so that PingSender can access it
    private static TcpClient irc = new TcpClient(SERVER, PORT);
    private static NetworkStream stream = irc.GetStream();
    private static StreamReader reader = new StreamReader(stream);
    public static StreamWriter writer = new StreamWriter(stream);
    private static StreamWriter logging = new StreamWriter(@"Logs\log" + DateTime.Now.ToString(" yyyy M d"),true);
    
    static void Main()
    {
        string nickname, inputLine;
        
        try
        {
            // Start PingSender thread
            PingSender ping = new PingSender();
            ping.Start();
            userInput test = new userInput();
            test.start();
            
            writer.WriteLine("PASS " + PASS);
            writer.Flush();
            writer.WriteLine("NICK " + NICK);
            writer.Flush();
            writer.WriteLine("JOIN " + CHANNEL);
            writer.Flush();
            
            while (true)
            {
                while ((inputLine = reader.ReadLine ()) != null)
                {
                    System.Diagnostics.Debug.WriteLine(inputLine);
                    
                    string message = "";
                    string userName = "";
                    
                    string[] parts = inputLine.Split(' ');
                    
                    switch(parts[1])
                    {
                        case "JOIN":
                            userName = parts[0].Substring(1, parts[0].IndexOf('!')-1);
                            Users.onUserJoin(userName);
                            break;
                            
                        case "PART":
                            userName = parts[0].Substring(1, parts[0].IndexOf('!')-1);
                            Users.onUserLeft(userName);
                            break;
                            
                        case "MODE":
                            foreach(User us in Users.list)
                            {
                                if (us.name == parts[4])
                                {
                                    us.admin = true;
                                    break;
                                }
                            }
                            break;
                            
                        case "353":
                            for(int i=5;i<parts.Length;i++)
                            {
                                if (i == 5)
                                {
                                    Users.onUserJoin(parts[5].Substring(1));
                                }
                                else
                                {
                                    Users.onUserJoin(parts[i]);
                                }
                            }
                            break;
                            
                        case "PRIVMSG":
                            if (parts[0].Contains("jtv")) { break; }
                                
                            userName = parts[0].Substring(1, parts[0].IndexOf('!')-1);
                            Users.onUserJoin(userName);
                            
                            string tmp = "";
                            for(int i=3;i<parts.Length;i++)
                            {
                                tmp += parts[i] + " ";
                            }
                            
                            if (Users.isUserAdmin(userName))
                            {
                                if (userName == "monstercat")
                                {
                                    writeLine(UppercaseFirst(userName) + " " + tmp, ConsoleColor.Cyan);
                                }
                                else
                                {
                                    writeLine(UppercaseFirst(userName) + " " + tmp, ConsoleColor.Yellow);
                                }
                            }
                            else
                            {
                                writeLine(UppercaseFirst(userName) + " " + tmp);
                            }
                            
                            // Now check if the user just sent a command
                            if (parts[3].StartsWith(":!"))
                            {
                                switch(parts.Length)
                                {
                                    case 4:
                                        Command.Check(userName, parts[3].Substring(2).ToLower());
                                        break;

                                    case 5:
                                        Command.Check(userName, parts[3].Substring(2).ToLower(),parts[4].ToLower());
                                        break;
                                    
                                    case 6:
                                        Command.Check(userName, parts[3].Substring(2).ToLower(),parts[4].ToLower(),parts[5].ToLower());
                                        break;
                                    
                                    case 7:
                                        Command.Check(userName, parts[3].Substring(2).ToLower(),parts[4].ToLower(),parts[5].ToLower(),parts[6].ToLower());
                                        break;

                                    default:
                                        // Else do nothing
                                        break;
                                }
                            }
                            break;
                            
                        default:
                            // Unknown message? Don't care, don't show it, it's in debug window anyways!
                            break;
                    }
                    
                    #region Console Output/Send message
                    // Makes sure to not show/sends empty messages
                    if (message != "")
                    {
                        writer.WriteLine(message);
                        writer.Flush();
                    }
                    #endregion
                }

                // Close all streams
                writer.Close();
                reader.Close();
                irc.Close();
            }
        }
        catch (Exception e)
        {
            // Show the exception, sleep for a while and try to establish a new connection to irc server
            Console.WriteLine(e.ToString ());
            Thread.Sleep(5000);
            Main();
        }
    }
    
    static string UppercaseFirst(string s)
    {
        // Check for empty string.
        if (string.IsNullOrEmpty(s))
        {
            return string.Empty;
        }
        // Return char and concat substring.
        return char.ToUpper(s[0]) + s.Substring(1);
    }
    
    public static void writeLine(string line)
    {
        Console.WriteLine(DateTime.Now.ToString("HH:mm") + " " + line);
        logging.WriteLine(DateTime.Now.ToString("HH:mm") + " " + line);
        logging.Flush();
    }
    
    public static void writeLine(string line, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(DateTime.Now.ToString("HH:mm") + " " + line);
        Console.ResetColor();
        logging.WriteLine(DateTime.Now.ToString("HH:mm") + " " + line);
        logging.Flush();
    }
    
    public static void Send(string text)
    {
        writer.WriteLine(text);
        writer.Flush();
    }
}