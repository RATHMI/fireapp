using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace FireApp.Service.Logging
{
    public static class Logger
    {
        public static void Log(string logMessage, string logPath)
        {
            try
            {
                if (!File.Exists(logPath))
                {
                    File.Create(logPath);
                }
                using (StreamWriter w = File.AppendText(logPath))
                {
                    w.WriteLine("\r\n{0} {1}", DateTime.Now.ToLongTimeString(), DateTime.Now.ToLongDateString());
                    w.WriteLine(" : {0}", logMessage);
                }
            }
            catch(IOException ex)
            {
                Console.WriteLine("Problem with logger\n" + ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Problem with logger\n" + ex.Message);
            }
        }       
    }
}