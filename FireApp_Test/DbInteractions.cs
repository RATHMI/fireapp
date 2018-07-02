using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using FireApp.Domain;

namespace FireApp.Test
{
    public class DbInteractions
    {
        private static HttpClient httpClient;
        private string address;

        public DbInteractions(string address)
        {
            httpClient = new HttpClient();
            this.address = address;
        }

        public IEnumerable<FireEvent> GetAllFireEvents()
        {
            return ServiceGetCall<IEnumerable<FireEvent>>(address + "all");
        }

        public IEnumerable<FireEvent> GetFireEventById(FireApp.Domain.FireEventId id)
        {
            return ServiceGetCall<IEnumerable<FireEvent>>(address + "id"); // todo: change address
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
