using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;

namespace FireApp.Service {
    public static class AppSettings {
        public static bool QualityMode { get; set; }
        public static string ServicePath { get; set; }


        public static string FireEventDBPath { get; set; }


        static AppSettings() {
            var fullSystemPath = System.Reflection.Assembly.GetAssembly(typeof(AppSettings)).CodeBase;
            ServicePath = new Uri(Path.GetDirectoryName(fullSystemPath)).LocalPath;
            QualityMode = fullSystemPath.ToLower().Contains("_q");

            FireEventDBPath = ConfigurationManager.AppSettings["fireEventDBPath"].ToFullPath();
        }

        public static string ToFullPath(this string path) {
            if (path.StartsWith("..")) {
                path = Path.Combine(ServicePath, path);
                path = Path.GetFullPath(path);
            }
            return path;
        }

        public static void SaveLastCrashData(Exception e) {
            try {
                Func<Exception, string> recExceptionTexts = null;
                recExceptionTexts = (ex) => { return ex == null ? string.Empty : string.Format("ex: {0}\r\nst: {1}\r\n", ex.Message, ex.StackTrace) + recExceptionTexts(ex.InnerException); };

                int i = 0;
                while (File.Exists(Path.Combine(ServicePath, "crash") + i + ".txt")) i++;
                File.AppendAllText(Path.Combine(ServicePath, "crash") + i + ".txt", recExceptionTexts(e));
            } catch (Exception) {
            }
        }

        public static void InitializeApp() {
            //method is called by iis before global appstart // therefore following entry is needed in the assemblyInfo.cs
            //[assembly: PreApplicationStartMethod(typeof(Ticket_Monitor.AppSettings), "InitializeApp")]
            //Ticket_Monitor.DAL.AppData.Initialize(ASCDB, string.Empty);
        }
    }
}