using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FireApp.Domain;

namespace FireApp.Test
{
    public static class Tests
    {
        private static HttpClient httpClient = new HttpClient();

        public static string GetFireEventsBySourceId(string address, int sourceId)
        {
            address += "sid/";
            address += sourceId.ToString();
            IEnumerable<FireEvent> events = ServiceGetCall<IEnumerable<FireEvent>>(address);

            StringBuilder sb = new StringBuilder();
            foreach(FireEvent fe in events)
            {
                sb.Append(getStringFromFireEvent(fe));
            }

            return sb.ToString();
        }

        public static string GetFireEventById(string address, FireEventId id)
        {
            address += "id/";
            address += id.SourceId.ToString();
            address += "/";
            address += id.EventId.ToString();
            FireEvent fe = ServiceGetCall<FireEvent>(address);

            return getStringFromFireEvent(fe);
        }

        public static string GetAllFireEvents(string address)
        {
            address += "all";

            IEnumerable<FireEvent> events = ServiceGetCall<IEnumerable<FireEvent>>(address);

            StringBuilder sb = new StringBuilder();
            foreach (FireEvent fe in events)
            {
                sb.Append(getStringFromFireEvent(fe));
            }

            return sb.ToString();
        }

        public static string UploadFireEvent(string address, FireEvent fe)
        {
            address += "upload";
            return "Upload successful: " + ServicePostCall<FireEvent, bool>(address, fe).ToString(); ;
        }

        private static string getStringFromFireEvent(FireEvent fe)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("\r\nsourceId: ");
            sb.Append(fe.Id.SourceId);
            sb.Append("\r\neventId: ");
            sb.Append(fe.Id.EventId);
            sb.Append("\r\nTimestamp: ");
            sb.Append(fe.TimeStamp.ToLongTimeString());

            return sb.ToString();
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

