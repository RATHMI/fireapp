using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FireApp.Domain;

namespace FireApp.Test
{
    public static class FireEventTests
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

        public static string GetFireEventsBySourceIdTargetId(string address, int sourceId, string targetId)
        {
            address += "stid/";
            address += sourceId.ToString();
            address += "/";
            address += targetId;

            IEnumerable<FireEvent> events = ServiceGetCall<IEnumerable<FireEvent>>(address);

            StringBuilder sb = new StringBuilder();
            foreach (FireEvent fe in events)
            {
                sb.Append(getStringFromFireEvent(fe));
            }

            return sb.ToString();
        }

        public static string GetFireEventsBySourceIdTargetIdTimeStamp(string address, int sourceId, string targetId, DateTime timeStamp)
        {
            address += "stidt/";
            address += sourceId.ToString();
            address += "/";
            address += targetId;
            address += "/";
            address += timeStamp.Ticks.ToString();

            IEnumerable<FireEvent> events = ServiceGetCall<IEnumerable<FireEvent>>(address);

            StringBuilder sb = new StringBuilder();
            foreach (FireEvent fe in events)
            {
                sb.Append(getStringFromFireEvent(fe));
            }

            return sb.ToString();
        }

        public static string GetFireEventsBySourceIdEventType(string address, int sourceId, EventTypes eventType)
        {
            address += "et/";
            address += sourceId.ToString();
            address += "/";
            address += eventType;

            IEnumerable<FireEvent> events = ServiceGetCall<IEnumerable<FireEvent>>(address);

            StringBuilder sb = new StringBuilder();
            foreach (FireEvent fe in events)
            {
                sb.Append(getStringFromFireEvent(fe));
            }

            return sb.ToString();
        }

        public static string GetFireEventsBySourceIdTimespan(string address, int sourceId, DateTime startTime, DateTime endTime)
        {
            address += "time/";           
            address += sourceId.ToString();
            address += "/";
            address += startTime.Ticks.ToString();
            address += "/";
            address += endTime.Ticks.ToString();

            IEnumerable<FireEvent> events = ServiceGetCall<IEnumerable<FireEvent>>(address);

            StringBuilder sb = new StringBuilder();
            foreach (FireEvent fe in events)
            {
                sb.Append(getStringFromFireEvent(fe));
            }

            return sb.ToString();
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

        public static string GetActiveFireEvents(string address)
        {
            address += "active";
            IEnumerable<FireEvent> events = ServiceGetCall<IEnumerable<FireEvent>>(address);

            StringBuilder sb = new StringBuilder();
            foreach (FireEvent fe in events)
            {
                sb.Append(getStringFromFireEvent(fe));
            }

            return sb.ToString();
        }

        private static string getStringFromFireEvent(FireEvent fe)
        {
            string rv;
            if (fe != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("\r\n\r\nsourceId: ");
                sb.Append(fe.Id.SourceId);
                sb.Append("\r\neventId: ");
                sb.Append(fe.Id.EventId);
                sb.Append("\r\ntargetId: ");
                sb.Append(fe.TargetId);
                sb.Append("\r\ntargetDescription: ");
                sb.Append(fe.TargetDescription);
                sb.Append("\r\nTimestamp: ");
                sb.Append(fe.TimeStamp.ToString());
                sb.Append("\r\neventType: ");
                sb.Append(fe.EventType);

                rv = sb.ToString();
            }else{
                rv = "FireEvent is null";
            }
            return rv;
        }


        #region Templates
        private static T ServiceGetCall<T>(string callAddress)
        {
            httpClient.DefaultRequestHeaders.Add("token", "1234");
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

