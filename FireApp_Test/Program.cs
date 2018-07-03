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

            //var res1 = ServicePostCall<FireEvent, bool>(addr + "upload", newItem);
            System.Console.WriteLine("\r\n\r\nTest UploadFireEvent");
            FireEvent fe = new FireEvent(new FireEventId(9, 9), DateTime.Now, "test", "description", EventTypes.disfunction);
            rv = Tests.UploadFireEvent(addr, fe);
            System.Console.WriteLine(rv);
            System.Console.WriteLine("\r\n------------------------------------------------------------------------------\r\n");

            // did not work because of the wrong generic datatype
            // used ServiceGetCall<IEnumerable<FireEvent>>(addr + "id/0/0");
            // instead of ServiceGetCall<FireEvent>(addr + "id/0/0");
            var byId = ServiceGetCall<FireEvent>(addr + "id/9/9");
            System.Console.WriteLine("\r\n\r\nTest GetFireEventById");
            rv = Tests.GetFireEventById(addr, new FireEventId(0,0));
            System.Console.WriteLine(rv);
            System.Console.WriteLine("\r\n------------------------------------------------------------------------------\r\n");

            //var byId = ServiceGetCall<IEnumerable<FireEvent>>(addr + "sid/0");
            System.Console.WriteLine("\r\n\r\nTest GetFireEventsBySourceId");
            rv = Tests.GetFireEventsBySourceId(addr, 0);
            System.Console.WriteLine(rv);
            System.Console.WriteLine("\r\n------------------------------------------------------------------------------\r\n");

            // var all2 = ServiceGetCall<IEnumerable<FireEvent>>(addr + "all");
            System.Console.WriteLine("\r\n\r\nTest GetAllFireEvents");
            rv = Tests.GetAllFireEvents(addr);
            System.Console.WriteLine(rv);
            System.Console.WriteLine("\r\n------------------------------------------------------------------------------\r\n");


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
