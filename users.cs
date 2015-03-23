using System;
using System.IO;
using System.Collections.Generic;

class Users
{
	public static List<User> list = new List<User>();
    
    #region isUserThere(string name)
    public static bool isUserThere(string name)
    {
        foreach(User us in list)
        {
            if (us.name == name) 
            { 
                return true; 
            }
        }
        return false;
    }
    #endregion
    
    #region getUser(string name)
    public static User getUser(string name)
    {
        foreach(User us in list)
        {
            if (us.name == name)
            {
                return us;
            }
        }
        return null;
    }
    #endregion
    
    #region isUserAdmin(string name)
    public static bool isUserAdmin(string name)
    {
        foreach(User us in list)
        {
            if (us.name == name && us.admin) 
            { 
                return true; 
            }
        }
        return false;
    }
    #endregion
    
    #region onUserJoin(string name)
    public static void onUserJoin(string name)
    {
        if (!isUserThere(name))
        {
            list.Add(new User(name));
            IrcBot.writeLine("[" + list.Count + "] " + name + " has joined", ConsoleColor.White);
        }
    }
    #endregion
    
    #region onUserLeft(string name)
    public static void onUserLeft(string userName)
    {
        if (isUserThere(userName))
        {
            foreach(User us in list)
            {
                if (us.name == userName && list.Contains(us))
                {
                    list.Remove(us);
                    IrcBot.writeLine("[" + list.Count + "] " + us.name + " has left", ConsoleColor.White);
                    break;
                }
            }
        }
    }
    #endregion
}

public class User
{
	public string name;
	public int currency = 0,watchTime = 0;
    public bool admin = false;
    
    //RPG stats
    public int hp = 100, def = 0, atk = 0, rep = 10;
    
    public User(string name)
    {
        this.name = name;
        this.admin = XmlProcess.Read("xml\\users.xml","User", name,"admin").ToUpper() == "TRUE";
        int.TryParse(XmlProcess.Read("xml\\users.xml","User", name,"currency"), out this.currency);
        int.TryParse(XmlProcess.Read("xml\\users.xml","User", name,"watchtime"), out this.watchTime);
        
        if (!int.TryParse(XmlProcess.Read("xml\\users.xml","User", name,"hp"), out this.hp))
        {
            this.hp=100;
        }
        int.TryParse(XmlProcess.Read("xml\\users.xml","User", name,"def"), out this.def);
        int.TryParse(XmlProcess.Read("xml\\users.xml","User", name,"atk"), out this.atk);
        if (!int.TryParse(XmlProcess.Read("xml\\users.xml","User", name,"rep"), out this.rep))
        {
            this.rep = 10;
        }
    }
}