using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace FireApp.Service.Logging
{
    public static class Logger
    {
        private const string logPath = "../../_Logs/log.txt";
        public static void Log(string logMessage)
        {
            using (StreamWriter w = File.AppendText(logPath))
            {
                w.WriteLine("\r\n{0} {1}", DateTime.Now.ToLongTimeString(), DateTime.Now.ToLongDateString());
                w.WriteLine(" : {0}", logMessage);
            }        
        }       
    }
}