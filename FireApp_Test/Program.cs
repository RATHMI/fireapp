using FireApp.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace FireApp.Test {
    class Program
    {
        private static HttpClient httpClient = new HttpClient();

        internal string ServiceUrl { get; private set; }
        static string addr = "http://localhost:50862/events/";

        static void Main(string[] args)
        {
            httpClient = new HttpClient();
            string rv;

            /*
            #region testsFireEvents
            addr = "http://localhost:50862/events/";
            System.Console.WriteLine("\r\n\r\nTest UploadFireEvent");
            FireEvent fe = new FireEvent(new FireEventId(9, 9), new DateTime(2018,6,1,0,0,0), "test", "description", EventTypes.test);
            rv = FireEventTests.UploadFireEvent(addr, fe);
            System.Console.WriteLine(rv);
            System.Console.WriteLine("\r\n------------------------------------------------------------------------------\r\n");
            
            var byId = ServiceGetCall<FireEvent>(addr + "id/9/9");
            System.Console.WriteLine("\r\n\r\nTest GetFireEventById");
            rv = FireEventTests.GetFireEventById(addr, new FireEventId(9,9));
            System.Console.WriteLine(rv);
            System.Console.WriteLine("\r\n------------------------------------------------------------------------------\r\n");
            
            System.Console.WriteLine("\r\n\r\nTest GetFireEventsBySourceId");
            rv = FireEventTests.GetFireEventsBySourceId(addr, 9);
            System.Console.WriteLine(rv);
            System.Console.WriteLine("\r\n------------------------------------------------------------------------------\r\n");

            System.Console.WriteLine("\r\n\r\nTest GetAllFireEvents");
            rv = FireEventTests.GetAllFireEvents(addr);
            System.Console.WriteLine(rv);
            System.Console.WriteLine("\r\n------------------------------------------------------------------------------\r\n");

            System.Console.WriteLine("\r\n\r\nTest GetFireEventsBySourceIdTargetId");
            rv = FireEventTests.GetFireEventsBySourceIdTargetId(addr, 9, "test");
            System.Console.WriteLine(rv);
            System.Console.WriteLine("\r\n------------------------------------------------------------------------------\r\n");

            System.Console.WriteLine("\r\n\r\nTest GetFireEventsBySourceIdEventType");
            rv = FireEventTests.GetFireEventsBySourceIdEventType(addr, 9, EventTypes.test);
            System.Console.WriteLine(rv);
            System.Console.WriteLine("\r\n------------------------------------------------------------------------------\r\n");

            System.Console.WriteLine("\r\n\r\nTest GetFireEventsBySourceIdTimespan");
            DateTime startTime = new DateTime(100);
            DateTime endTime = DateTime.Now;
            rv = FireEventTests.GetFireEventsBySourceIdTimespan(addr, 9, startTime, endTime);
            System.Console.WriteLine("\r\nstartTime: " + startTime.ToString());
            System.Console.WriteLine("\r\nendTime: " + endTime.ToString());
            System.Console.WriteLine(rv);
            System.Console.WriteLine("\r\n------------------------------------------------------------------------------\r\n");
            #endregion
            */
            #region testsFireAlarmSystems
            addr = "http://localhost:50862/fas/";

            System.Console.WriteLine("\r\n\r\nTest UploadFireAlarmSystem");
            FireAlarmSystem fas = new FireAlarmSystem();
            fas.City = "Linz";
            fas.Address = "Wolfgang-Pauli-Straße 2";
            rv = FireAlarmSystemTests.UploadFireAlarmSystem(addr, fas);
            System.Console.WriteLine(rv);
            System.Console.WriteLine("\r\n------------------------------------------------------------------------------\r\n");

            System.Console.WriteLine("\r\n\r\nTest GetAllFireAlarmSystems");
            rv = FireAlarmSystemTests.GetAllFireAlarmSystems(addr);
            System.Console.WriteLine(rv);
            System.Console.WriteLine("\r\n------------------------------------------------------------------------------\r\n");
            #endregion


            System.Console.ReadKey();
        }

        #region Templates
        private static T ServiceGetCall<T>(string callAddress)
        {
            HttpResponseMessage resp = httpClient.GetAsync(callAddress).Result;
            resp.EnsureSuccessStatusCode();
            return resp.Content.ReadAsAsync<T>().Result;
        }

        //public static T ServiceDeleteCall<T>(string callAddress) {
        //    HttpResponseMessage resp = httpClient.DeleteAsync(ServiceUrl + callAddress).Result;
        //    resp.EnsureSuccessStatusCode();
        //    return resp.Content.ReadAsAsync<T>().Result;
        //}

        //public static R ServicePutCall<T, R>(string callAddress, T element) {
        //    HttpResponseMessage resp = httpClient.PutAsJsonAsync<T>(ServiceUrl + callAddress, element).Result;
        //    resp.EnsureSuccessStatusCode();
        //    return resp.Content.ReadAsAsync<R>().Result;
        //}

        private static R ServicePostCall<T, R>(string callAddress, T element)
        {
            HttpResponseMessage resp = httpClient.PostAsJsonAsync<T>(callAddress, element).Result;
            resp.EnsureSuccessStatusCode();
            return resp.Content.ReadAsAsync<R>().Result;
        }
        #endregion
    }
}
