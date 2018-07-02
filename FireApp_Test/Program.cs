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

        static void Main(string[] args) {
            httpClient = new HttpClient();

            var addr = "http://192.168.1.105:50862/events/"; 

            var newItem = new FireEvent {
                At = DateTime.Now,
                By = "test1",
                Name = "name1"
            };

            var res1 = ServicePostCall<FireEvent, bool>(addr + "upload", newItem);

            var all1 = ServiceGetCall<IEnumerable<FireEvent>>(addr + "all");

            var res2 = ServicePostCall<FireEvent, bool>(addr + "upload", newItem);

            var all2 = ServiceGetCall<IEnumerable<FireEvent>>(addr + "all");

            var name1 = ServiceGetCall<IEnumerable<FireEvent>>(addr + "getNameTest/test1234");

            foreach (var entry in name1)
            {
                System.Console.WriteLine("Name: " + entry.Name + "\nDate: " + entry.Id);
            }

            System.Console.ReadKey();*/
        }

        /*public T GetFireEvenByName(string name)
        {
            return ServiceGetCall<IEnumerable<FireEvent>>(addr + "getNameTest/test1234");
        }*/

        #region Templates
        public static T ServiceGetCall<T>(string callAddress) {
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

        public static R ServicePostCall<T, R>(string callAddress, T element) {
            HttpResponseMessage resp = httpClient.PostAsJsonAsync<T>(callAddress, element).Result;
            resp.EnsureSuccessStatusCode();
            return resp.Content.ReadAsAsync<R>().Result;
        }
        #endregion
    }
}
