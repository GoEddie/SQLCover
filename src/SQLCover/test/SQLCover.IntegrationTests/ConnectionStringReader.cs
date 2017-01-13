﻿using System;
using System.Xml;

namespace SQLCover.IntegrationTests
{
    class ConnectionStringReader
    {
        private static string Get(string name)
        {
            var xmldoc = new XmlDocument();
            xmldoc.Load(GetFilename());
            XmlNodeList nodes = xmldoc.GetElementsByTagName("add");
            foreach (XmlNode node in nodes)
                if (node.Attributes["name"].Value == name)
                    return node.Attributes["connectionString"].Value;
            throw new Exception();
        }


        public static string GetIntegration()
        {
            return Get("Integration");
        }
        
        private static string GetFilename()
        {
            //If available use the user file
            if (System.IO.File.Exists("ConnectionString.user.config"))
            {
                return "ConnectionString.user.config";
            }
            else if (System.IO.File.Exists("ConnectionString.config"))
            {
                return "ConnectionString.config";
            }
            return "";
        }
        
    }
}
