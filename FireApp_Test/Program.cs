using FireApp.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace FireApp.Test {
    class Program {
        private static HttpClient httpClient;

        internal string ServiceUrl { get; private set; }
        static string addr = "http://localhost:50862/events/";

        static void Main(string[] args) {
            httpClient = new HttpClient();

            var newItem = new FireEvent {
                Id = new FireEventId { SourceId = 0, EventId = 0 },
                TimeStamp = DateTime.Now,
                TargetId = "testtarget",
                TargetDescription = "testdescription",
                EventType = EventTypes.disfunction
            };

            var res1 = ServicePostCall<FireEvent, bool>(addr + "upload", newItem);

            var byId = ServiceGetCall<IEnumerable<FireEvent>>(addr + "id/0");

            //var res2 = ServicePostCall<FireEvent, bool>(addr + "upload", newItem);

           // var all2 = ServiceGetCall<IEnumerable<FireEvent>>(addr + "all");

            //var name1 = ServiceGetCall<IEnumerable<FireEvent>>(addr + "getNameTest/test1234");
            

            //@philippollmann: just a short test
            foreach (var entry in byId)
            {
                System.Console.WriteLine("Id: " + entry.Id);
            }
            
            /*DbInteractions dbi = new DbInteractions(addr);
            var res1 = dbi.GetAllFireEvents();*/
            System.Console.ReadKey();
        }

        /*public T GetFireEvenByName(string name)
        {
            return ServiceGetCall<IEnumerable<FireEvent>>(addr + "getNameTest/test1234");
        }*/

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
