using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;

namespace FireApp.Service {
    public class AppResources {
        //public static SiteNetworkApps SiteNetworkApps { get { return SiteNetworkAppStore.Instance.Apps; } }
        //public static SiteNetworkPersons SiteNetworkPersons { get { return SiteNetworkPersonStore.Instance.Users; } set { SiteNetworkPersonStore.Instance.Upload(value); } }
        //public static HttpClient HttpClient { get { return ClientStore.Instance.Client; } }


        //public class SiteNetworkAppStore {
        //    #region Singleton
        //    // singleton: to keep the dependency injection way open 
        //    private static volatile SiteNetworkAppStore instance;
        //    private static object locker = new object();

        //    public static SiteNetworkAppStore Instance {
        //        get {
        //            if (instance == null) {
        //                lock (locker) {
        //                    instance = new SiteNetworkAppStore(AppSettings.ApplicationListPath);
        //                }
        //            }
        //            return instance;
        //        }
        //    }

        //    // singleton
        //    private SiteNetworkAppStore(string filePath) {
        //        if (!File.Exists(filePath))
        //            SiteNetworkApps.Write(new List<SiteNetworkApp>(), filePath);

        //        Apps = SiteNetworkApps.Load(filePath);
        //    }
        //    #endregion
        //    public SiteNetworkApps Apps { get; set; }
        //}
        //public class SiteNetworkPersonStore {
        //    #region Singleton
        //    // singleton: to keep the dependency injection way open 
        //    private static volatile SiteNetworkPersonStore instance;
        //    private static object locker = new object();

        //    public static SiteNetworkPersonStore Instance {
        //        get {
        //            if (instance == null) {
        //                lock (locker) {
        //                    instance = new SiteNetworkPersonStore(AppSettings.UsersListPath);
        //                }
        //            }
        //            return instance;
        //        }
        //    }

        //    // singleton
        //    private SiteNetworkPersonStore(string filePath) {
        //        if (!File.Exists(filePath))
        //            SiteNetworkPersons.Write(new List<SiteNetworkPerson>(), filePath);

        //        Users = SiteNetworkPersons.Load(filePath);
        //    }
        //    #endregion
        //    public SiteNetworkPersons Users { get; set; }

        //    public void Upload(SiteNetworkPersons users) {
        //        Users = users;
        //        SiteNetworkPersons.Write(users.Persons, AppSettings.UsersListPath);
        //    }
        //}
        //public class ClientStore {
        //    #region Singleton
        //    // singleton: to keep the dependency injection way open 
        //    private static volatile ClientStore instance;
        //    private static object locker = new object();

        //    public static ClientStore Instance {
        //        get {
        //            if (instance == null) {
        //                lock (locker) {
        //                    instance = new ClientStore();
        //                }
        //            }
        //            return instance;
        //        }
        //    }

        //    // singleton
        //    private ClientStore() {
        //        Client = new HttpClient();
        //        ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
        //    }
        //    #endregion
        //    public HttpClient Client { get; set; }
        //}
    }
}