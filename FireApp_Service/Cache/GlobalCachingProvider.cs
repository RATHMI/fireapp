using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FireApp.Service.Cache
{
    /// <summary>
    /// copied from: https://www.codeproject.com/Articles/756423/How-to-Cache-Objects-Simply-using-System-Runtime-C
    /// </summary>
    public class GlobalCachingProvider : CachingProviderBase, IGlobalCachingProvider
    {
        #region Singleton 

        protected GlobalCachingProvider()
        {
        }

        public static GlobalCachingProvider Instance
        {
            get
            {
                return Nested.instance;
            }
        }

        class Nested
        {
            // Explicit static constructor to tell C# compiler
            // not to mark type as beforefieldinit
            static Nested()
            {
            }
            internal static readonly GlobalCachingProvider instance = new GlobalCachingProvider();
        }

        #endregion

        #region ICachingProvider

        public virtual new void AddItem(string key, object value)
        {
            base.AddItem(key, value);
        }

        public virtual object GetItem(string key)
        {
            return base.GetItem(key, true);//Remove default is true because it's Global Cache!
        }

        public virtual new object GetItem(string key, bool remove)
        {
            return base.GetItem(key, remove);
        }

        public virtual new void RemoveItem(string key)
        {
            base.RemoveItem(key);
        }
        #endregion

    }
}