using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FireApp.Service.Cache
{
    /// <summary>
    /// copied from: https://www.codeproject.com/Articles/756423/How-to-Cache-Objects-Simply-using-System-Runtime-C
    /// </summary>
    public interface IGlobalCachingProvider
    {
        void AddItem(string key, object value);
        object GetItem(string key);
    }
}