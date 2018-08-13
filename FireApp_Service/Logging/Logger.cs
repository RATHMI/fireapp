using Microsoft.IdentityModel.Protocols;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace FireApp.Service.Logging
{
    /// <summary>
    /// This class provides methods to create a log for operations with domain objects.
    /// </summary>
    public static class Logger
    {
        private static string logPath() {
            return ConfigurationManager.AppSettings["loggingPath"].ToFullPath() + DateTime.Now.ToString("yyyyMMdd") + "_log.txt";
        }

        /// <summary>
        /// Writes a log message to the logPath.
        /// </summary>
        /// <param name="logMessage">The type of operation you perform with the oject.</param>
        /// <param name="changedObject">The domain object which you perform an action on.</param>
        public static void Log(string logMessage, string user, object changedObject)
        {
            try
            {
                if (!File.Exists(logPath()))
                {
                    File.Create(logPath());
                }
                using (StreamWriter w = File.AppendText(logPath()))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append(DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss"));
                    sb.Append(';');
                    sb.Append(user);
                    sb.Append(';');
                    sb.Append(logMessage);
                    sb.Append(';');
                    sb.Append(changedObject.GetType().ToString());
                    sb.Append(';');
                    sb.Append(Newtonsoft.Json.JsonConvert.SerializeObject(changedObject));

                    w.WriteLine(sb.ToString());
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