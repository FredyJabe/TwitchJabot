using System;
using System.IO;

class Command
{
    #region Simple command without arguments
    public static void Check(string caller, string cmdName)
    {
        User us = Users.getUser(caller);
        
        switch(cmdName.ToLower())
        {
            #region !help // does nothing for now
            case "help":
                {
                    IrcBot.Send("PRIVMSG "+IrcBot.CHANNEL+" :/me For a list of commands, click here http://pastebin.com/nf15NubV");
                    break;
                }
            #endregion
            
            #region !money
            case "money":
                if (us.currency == 0)
                {
                    IrcBot.Send("PRIVMSG "+IrcBot.CHANNEL+" :/me Oh that's sad "+caller+", you don't have any "+IrcBot.oneCurrency);
                }
                if (us.currency > 1)
                {
                    IrcBot.Send("PRIVMSG " + IrcBot.CHANNEL + " :/me "+caller+" have "+us.currency+" "+IrcBot.multiCurrency);
                }
                else
                {
                    IrcBot.Send("PRIVMSG " + IrcBot.CHANNEL + " :/me "+caller+" have "+us.currency+" "+IrcBot.oneCurrency);
                }
                break;
            #endregion
            
            #region !watched
            case "watched":
                int hours = us.watchTime/60;
                int minutes = us.watchTime % 60;
                
                string singleHour = "hour";
                string multiHour = "hours";
                string singleMinute = "minute";
                string multiMinute = "minutes";
                
                if (us.watchTime > 60)
                {
                    if (hours > 1) 
                    {
                        if (minutes > 1)
                        {
                            IrcBot.Send("PRIVMSG "+IrcBot.CHANNEL+" :/me "+caller+" has been watching for "+hours+" "+multiHour+" and "+minutes+" "+multiMinute+".");
                        }
                        else
                        {
                            IrcBot.Send("PRIVMSG "+IrcBot.CHANNEL+" :/me "+caller+" has been watching for "+hours+" "+multiHour+" and "+minutes+" "+singleMinute+".");
                        }
                    }
                    else
                    {
                        if (minutes > 1)
                        {
                            IrcBot.Send("PRIVMSG "+IrcBot.CHANNEL+" :/me "+caller+" has been watching for "+hours+" "+singleHour+" and "+minutes+" "+multiMinute+".");
                        }
                        else
                        {
                            IrcBot.Send("PRIVMSG "+IrcBot.CHANNEL+" :/me "+caller+" has been watching for "+hours+" "+singleHour+" and "+minutes+" "+singleMinute+".");
                        }
                    }
                }
                else
                {
                    if (minutes > 1)
                    {
                        IrcBot.Send("PRIVMSG "+IrcBot.CHANNEL+" :/me "+caller+" has been watching for "+minutes+" "+multiMinute+".");
                    }
                    else
                    {
                        IrcBot.Send("PRIVMSG "+IrcBot.CHANNEL+" :/me "+caller+" has been watching for "+minutes+" "+singleMinute+".");
                    }
                }
                break;
            #endregion
                
            #region !rep
            case "rep":
                IrcBot.Send("PRIVMSG "+IrcBot.CHANNEL+" :/me "+caller+" has "+us.rep+" reputation");
                break;
            #endregion
            
            #region !hp
            case "hp":
                if (IrcBot.RPG)
                {
                    IrcBot.Send("PRIVMSG "+IrcBot.CHANNEL+" :/me "+caller+" has "+us.hp+" hp left");
                }
                break;
            #endregion
            
            #region !admin
            case "admin":
                if (Users.isUserAdmin(caller))
                {
                    IrcBot.Send("PRIVMSG " + IrcBot.CHANNEL + " :/me " + caller + " is an admin!");
                }
                else 
                {
                    IrcBot.Send("PRIVMSG " + IrcBot.CHANNEL + " :/me " + caller + " is not an admin!");
                }
                break;
            #endregion
            
            #region !rpg
            case "rpg":
                if (Users.isUserAdmin(caller))
                {
                    if (IrcBot.RPG)
                    {
                        IrcBot.RPG = false;
                        IrcBot.writeLine("RPG mode deactivated", ConsoleColor.Green);
                        IrcBot.Send("PRIVMSG " + IrcBot.CHANNEL + " :/me RPG mode deactivated");
                    }
                    else
                    {
                        IrcBot.RPG = true;
                        IrcBot.writeLine("RPG mode activated", ConsoleColor.Green);
                        IrcBot.Send("PRIVMSG " + IrcBot.CHANNEL + " :/me RPG mode activated");
                    }
                }
                else
                {
                    if (IrcBot.RPG)
                    {
                        IrcBot.Send("PRIVMSG "+IrcBot.CHANNEL+" :/me RPG mode is activated");
                    }
                    else
                    {
                        IrcBot.Send("PRIVMSG "+IrcBot.CHANNEL+" :/me RPG mode is not activated, ask a MOD to activate it");
                    }
                }
                break;
            #endregion
            
            #region !start
            case "start":
                if (Users.isUserAdmin(caller))
                {
                    giveAways.start();
                    IrcBot.Send("PRIVMSG "+IrcBot.CHANNEL+" :/me "+caller+" has started the give away, type !join [tickets] to buy tickets to participate!");
                }
                break;
            #endregion
            
            #region !pick
            case "pick":
                if (Users.isUserAdmin(caller))
                {
                    string winner = giveAways.pick();
                    
                    if (winner == null) break;
                    
                    IrcBot.Send("PRIVMSG "+IrcBot.CHANNEL+" :/me the giveaway winner is "+winner+" !!");
                }
                break;
            #endregion
            
            
        }
    }
    #endregion
    
    #region Command with 1 argument
    public static void Check(string caller, string cmdName, string arg1)
    {
        User us = Users.getUser(caller);
        
        switch(cmdName)
        {
            #region !attack [user]
            case "attack":
                if (IrcBot.RPG)
                {
                    if (arg1 == caller)
                    {
                        return;
                    }
                    
                    if (Users.isUserThere(arg1))
                    {
                        User target = Users.getUser(arg1);
                        
                        if (target.hp == 0)
                        {
                            IrcBot.Send("PRIVMSG "+IrcBot.CHANNEL+" :/me "+arg1+" is already dead, why would you hit a corpse..");
                            break;
                        }
                        
                        Random rdm = new Random();
                        int crit = rdm.Next(100);
                        int damage = rdm.Next(3,6) + us.atk;
                        
                        if (crit <= 2)
                        {
                            if ((target.hp-damage*2) > 0)
                            {
                                target.hp -= damage*2;
                                IrcBot.Send("PRIVMSG "+IrcBot.CHANNEL+" :/me "+caller+" did "+(damage*2)+" critical damage to "+arg1+"!");
                            }
                            else
                            {
                                target.hp = 0;
                                IrcBot.Send("PRIVMSG "+IrcBot.CHANNEL+" :/me "+caller+" just killed "+arg1+" with a critical strike!");
                            }
                        }
                        else
                        {
                            if ((target.hp-damage*2) > 0)
                            {
                                target.hp -= damage;
                                IrcBot.Send("PRIVMSG "+IrcBot.CHANNEL+" :/me "+caller+" did "+damage+" damage to "+arg1);
                            }
                            else
                            {
                                target.hp = 0;
                                IrcBot.Send("PRIVMSG "+IrcBot.CHANNEL+" :/me "+caller+" just killed "+arg1);
                            }
                        }
                        
                        
                    }
                }
                break;
            #endregion
            
            #region !like [user]
            case "like":
                if (Users.isUserThere(arg1))
                {
                    User target = Users.getUser(arg1);
                    
                    target.rep ++;
                    us.rep --;
                    
                    IrcBot.writeLine(caller+" gave rep to "+arg1, ConsoleColor.Magenta);
                    IrcBot.Send("PRIVMSG "+IrcBot.CHANNEL+" :/me "+caller+" gave rep to "+arg1);
                }
                break;
            #endregion
            
            #region !join [tickets]
            case "join":
                int tickets;
                int.TryParse(arg1, out tickets);
                
                if (us.currency >= tickets)
                {
                    us.currency -= tickets;
                    giveAways.participate(caller,tickets);
                    IrcBot.writeLine(caller+" bought "+tickets+" tickets", ConsoleColor.Red);
                }
                break;
            #endregion
            
            #region !revive [user]
            case "revive":
                if (Users.isUserThere(arg1))
                {
                    if (Users.isUserAdmin(caller))
                    {
                        User target = Users.getUser(arg1);
                    
                        if (target.hp == 0)
                        {
                            target.hp = 100;
                            IrcBot.Send("PRIVMSG "+IrcBot.CHANNEL+" :/me "+caller+" just revived "+arg1+" with a divine force");
                        }
                    }
                }
                break;
            #endregion
            
            
        }
    }
    #endregion
    
    #region Command with 2 argument
    public static void Check(string caller, string cmdName, string arg1, string arg2)
    {
        User us = Users.getUser(caller);
        
        switch(cmdName)
        {
            #region !money add [amount]
            case "money":
                if (arg1 == "add")
                {
                    if (Users.isUserAdmin(caller))
                    {
                        int amount;
                        int.TryParse(arg2, out amount);
                        
                        if (amount == 0) break;
                        
                        if (amount != 0)
                        {
                            if (amount > 1)
                            {
                                IrcBot.writeLine(caller+" just added "+amount+" "+IrcBot.multiCurrency+" to everyone", ConsoleColor.White);
                                IrcBot.Send("PRIVMSG "+IrcBot.CHANNEL+" :/me "+caller+" added "+amount+" "+IrcBot.multiCurrency+" to everyone");
                            }
                            else
                            {
                                IrcBot.writeLine(caller+" just added "+amount+" "+IrcBot.oneCurrency+" to everyone", ConsoleColor.White);
                                IrcBot.Send("PRIVMSG "+IrcBot.CHANNEL+" :/me "+caller+" added "+amount+" "+IrcBot.oneCurrency+" to everyone");
                            }
                        }
                        else
                        {
                            if (amount < -1)
                            {
                                IrcBot.writeLine(caller+" just removed "+amount+" "+IrcBot.multiCurrency+" from everyone", ConsoleColor.White);
                                IrcBot.Send("PRIVMSG "+IrcBot.CHANNEL+" :/me "+caller+" removed "+amount+" "+IrcBot.multiCurrency+" from everyone");
                            }
                            else
                            {
                                IrcBot.writeLine(caller+" just removed "+amount+" "+IrcBot.oneCurrency+" from everyone", ConsoleColor.White);
                                IrcBot.Send("PRIVMSG "+IrcBot.CHANNEL+" :/me "+caller+" removed "+amount+" "+IrcBot.oneCurrency+" from everyone");
                            }
                        }
                        
                        foreach(User uss in Users.list)
                        {
                            uss.currency += amount;
                        }
                        
                        
                    }
                }
                break;
            #endregion
            
            #region !hp check [user]
            case "hp":
                if (arg1 == "check")
                {
                    if (Users.isUserThere(arg2))
                    {
                        User target = Users.getUser(arg2);
                        
                        if (target.hp == 0)
                        {
                            IrcBot.Send("PRIVMSG "+IrcBot.CHANNEL+" :/me "+arg2+" is dead");
                        }
                        else
                        {
                            IrcBot.Send("PRIVMSG "+IrcBot.CHANNEL+" :/me "+arg2+" has "+target.hp+" hp left");
                        }
                    }
                }
                break;
            #endregion
            
            
        }
    }
    #endregion
    
    #region Command with 3 argument
    public static void Check(string caller, string cmdName, string arg1, string arg2, string arg3)
    {
        User us = Users.getUser(caller);
        
        switch(cmdName)
        {
            #region !money give [amount] [user] | !money add [amount] [user]
            case "money":
                if (arg1 == "give")
                {
                    if (Users.isUserThere(arg3))
                    {
                        User target = Users.getUser(arg3);
                        int amount;
                        
                        int.TryParse(arg2, out amount);
                        
                        if (amount <= 0)
                        {
                            return;
                        }
                        
                        if (us.currency >= amount)
                        {
                            us.currency -= amount;
                            target.currency += amount;
                            
                            if (amount > 1)
                            {
                                IrcBot.Send("PRIVMSG "+IrcBot.CHANNEL+" :/me "+caller+" just gave "+amount+" "+IrcBot.multiCurrency+" to "+target);
                            }
                            else
                            {
                                IrcBot.Send("PRIVMSG "+IrcBot.CHANNEL+" :/me "+caller+" just gave "+amount+" "+IrcBot.oneCurrency+" to "+target);
                            }
                        }
                    }
                }
                else if (arg1 == "add")
                {
                    if (Users.isUserAdmin(caller))
                    {
                        if (Users.isUserThere(arg3))
                        {
                            int amount;
                            int.TryParse(arg2, out amount);
                            
                            if (amount == 0) break;
                            
                            User target = Users.getUser(arg3);
                            target.currency += amount;
                            
                            if (amount != 0)
                            {
                                if (amount > 1)
                                {
                                    IrcBot.writeLine(caller+" added "+amount+" "+IrcBot.multiCurrency+" to "+arg3, ConsoleColor.White);
                                    IrcBot.Send("PRIVMSG "+IrcBot.CHANNEL+" :/me "+caller+" added "+amount+" "+IrcBot.multiCurrency+" to "+arg3);
                                }
                                else
                                {
                                    IrcBot.writeLine(caller+" added "+amount+" "+IrcBot.oneCurrency+" to "+arg3, ConsoleColor.White);
                                    IrcBot.Send("PRIVMSG "+IrcBot.CHANNEL+" :/me "+caller+" added "+amount+" "+IrcBot.oneCurrency+" to "+arg3);
                                }
                            }
                            else
                            {
                                if (amount < -1)
                                {
                                    IrcBot.writeLine(caller+" removed "+amount+" "+IrcBot.multiCurrency+" from "+arg3, ConsoleColor.White);
                                    IrcBot.Send("PRIVMSG "+IrcBot.CHANNEL+" :/me "+caller+" removed "+amount+" "+IrcBot.multiCurrency+" from "+arg3);
                                }
                                else
                                {
                                    IrcBot.writeLine(caller+" removed "+amount+" "+IrcBot.oneCurrency+" from "+arg3, ConsoleColor.White);
                                    IrcBot.Send("PRIVMSG "+IrcBot.CHANNEL+" :/me "+caller+" removed "+amount+" "+IrcBot.oneCurrency+" from "+arg3);
                                }
                            }
                        }
                    }
                }
                break;
            #endregion
        }
    }
    #endregion
}