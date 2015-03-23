using System;
using System.Collections.Generic;

class giveAways
{
    private static List<string> giveAwayParticipation = new List<string>();
    
    public static void start()
    {
        IrcBot.activeGiveAway = true;
        IrcBot.writeLine("Giveaway started !!",ConsoleColor.Red);
    }
    
    public static void participate(string user, int times)
    {   
        for (int i=0;i<times;i++)
        {
            if (Users.isUserThere(user))
            {
                giveAwayParticipation.Add(user);
            }
        }
    }
    
    public static string pick()
    {
        if (giveAwayParticipation.Count == 0)
        {
            IrcBot.activeGiveAway = false;
            giveAwayParticipation.Clear();
            
            IrcBot.writeLine("Giveaway is done, no winner because nobody participated",ConsoleColor.Red);
            return null;
        }
        
        Random rdm = new Random();
        int position = rdm.Next(giveAwayParticipation.Count);
        string result = giveAwayParticipation[position];
        
        IrcBot.writeLine("Giveaway is done, winner is "+result,ConsoleColor.Red);
        
        IrcBot.activeGiveAway = false;
        giveAwayParticipation.Clear();
        
        return result;
    }
}