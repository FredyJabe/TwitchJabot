using System;
using System.IO;
using System.Xml;

class XmlProcess
{
    private static XmlDocument xFile = new XmlDocument();
    
    #region Read(string filePath, string element, string attribute)
    public static string Read(string filePath, string element, string user, string attribute)
    {
        xFile.Load(filePath);
        
        XmlNodeList nodes = xFile.SelectNodes("//"+element);
        
        foreach(XmlNode node in nodes)
        {
            if (node.Attributes["name"].Value == user)
            {
                if (node.Attributes[attribute] != null)
                {
                    return node.Attributes[attribute].Value;
                }
            }
        }
        
        return string.Empty;
    }
    #endregion
    
    #region Write(string filePath, string element, string attribute)
    public static void Write(string filePath, string element, params string[] args)
    {
        xFile.Load(filePath);

        XmlNodeList nodes = xFile.SelectNodes("//"+element);
        
        foreach(XmlNode node in nodes)
        {
            if (node.Attributes["name"].Value == args[0])
            {
                node.Attributes["admin"].Value = args[1];
                node.Attributes["currency"].Value = args[2];
                node.Attributes["watchtime"].Value = args[3];
                
                node.Attributes["hp"].Value = args[4];
                node.Attributes["def"].Value = args[5];
                node.Attributes["atk"].Value = args[6];
                node.Attributes["rep"].Value = args[7];
                
                xFile.Save(filePath);
                return;
            }

        }

        XmlElement newUser = xFile.CreateElement(element);
        xFile.DocumentElement.AppendChild(newUser);
        
        XmlAttribute a1 = xFile.CreateAttribute("name");
        XmlAttribute a2 = xFile.CreateAttribute("admin");
        XmlAttribute a3 = xFile.CreateAttribute("currency");
        XmlAttribute a4 = xFile.CreateAttribute("watchtime");
        XmlAttribute a5 = xFile.CreateAttribute("hp");
        XmlAttribute a6 = xFile.CreateAttribute("def");
        XmlAttribute a7 = xFile.CreateAttribute("atk");
        XmlAttribute a8 = xFile.CreateAttribute("rep");
        
        a1.Value = args[0];
        a2.Value = args[1];
        a3.Value = args[2];
        a4.Value = args[3];
        a5.Value = args[4];
        a6.Value = args[5];
        a7.Value = args[6];
        a8.Value = args[7];
        
        newUser.SetAttributeNode(a1);
        newUser.SetAttributeNode(a2);
        newUser.SetAttributeNode(a3);
        newUser.SetAttributeNode(a4);
        newUser.SetAttributeNode(a5);
        newUser.SetAttributeNode(a6);
        newUser.SetAttributeNode(a7);
        newUser.SetAttributeNode(a8);
        
        xFile.Save(filePath);
    }
    #endregion
}