using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Caching;

namespace FireApp.Service.Cache
{
    /// <summary>
    /// copied from: https://www.codeproject.com/Articles/756423/How-to-Cache-Objects-Simply-using-System-Runtime-C
    /// </summary>
    public abstract class CachingProviderBase
    {
        public CachingProviderBase()
        {
            DeleteLog();
        }

        protected MemoryCache cache = new MemoryCache("CachingProvider");

        static readonly object padlock = new object();

        protected virtual void AddItem(string key, object value)
        {
            lock (padlock)
            {
                cache.Add(key, value, DateTimeOffset.MaxValue);
            }
        }

        protected virtual void RemoveItem(string key)
        {
            lock (padlock)
            {
                cache.Remove(key);
            }
        }

        protected virtual object GetItem(string key, bool remove)
        {
            lock (padlock)
            {
                var res = cache[key];

                if (res != null)
                {
                    if (remove == true)
                        cache.Remove(key);
                }
                else
                {
                    WriteToLog("CachingProvider-GetItem: Don't contains key: " + key);
                }

                return res;
            }
        }

        #region Error Logs

        string LogPath = System.Environment.GetEnvironmentVariable("TEMP");

        protected void DeleteLog()
        {
            System.IO.File.Delete(string.Format("{0}\\CachingProvider_Errors.txt", LogPath));
        }

        protected void WriteToLog(string text)
        {
            using (System.IO.TextWriter tw = System.IO.File.AppendText(string.Format("{0}\\CachingProvider_Errors.txt", LogPath)))
            {
                tw.WriteLine(text);
                tw.Close();
            }
        }
        #endregion
    }
}