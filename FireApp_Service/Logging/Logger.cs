using Microsoft.IdentityModel.Protocols;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;

namespace FireApp.Service.Logging
{
    /// <summary>
    /// This class provides methods to create a log for operations with domain objects
    /// </summary>
    public static class Logger
    {
        private static string logPath = ConfigurationManager.AppSettings["loggingPath"].ToFullPath();

        /// <summary>
        /// Writes a log message to the logPath
        /// </summary>
        /// <param name="logMessage">The type of operation you perform with the oject</param>
        /// <param name="changedObject">The domain object which you perform an action on</param>
        public static void Log(string logMessage, string user, object changedObject)
        {
            try
            {
                if (!File.Exists(logPath))
                {
                    File.Create(logPath);
                }
                using (StreamWriter w = File.AppendText(logPath))
                {
                    w.Write("{0};{1}", Newtonsoft.Json.JsonConvert.SerializeObject(DateTime.Now), user);
                    w.WriteLine(";{0};{1};{2}\n", logMessage, changedObject.GetType().ToString(), Newtonsoft.Json.JsonConvert.SerializeObject(changedObject));
                    w.Close();
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