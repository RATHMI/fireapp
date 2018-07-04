using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FireApp.Domain;

namespace FireApp.Test
{
    public static class FireAlarmSystemTests
    {
        private static HttpClient httpClient = new HttpClient();

        public static string UploadFireAlarmSystem(string address, FireAlarmSystem fas)
        {
            address += "upload";
            return "Upload successful: " + ServicePostCall<FireAlarmSystem, bool>(address, fas).ToString(); ;
        }

        public static string GetAllFireAlarmSystems(string address)
        {
            address += "all";

            IEnumerable<FireAlarmSystem> fireAlarmSystems = ServiceGetCall<IEnumerable<FireAlarmSystem>>(address);

            StringBuilder sb = new StringBuilder();
            foreach (FireAlarmSystem fas in fireAlarmSystems)
            {
                sb.Append(getStringFromFireAlarmSystem(fas));
            }

            return sb.ToString();
        }

        private static string getStringFromFireAlarmSystem(FireAlarmSystem fas)
        {
            string rv;
            if (fas != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("\r\n\r\nId: ");
                sb.Append(fas.Id);
                sb.Append("\r\nCity: ");
                sb.Append(fas.City);
                sb.Append("\r\nAddress: ");
                sb.Append(fas.Address);
                
                rv = sb.ToString();
            }
            else
            {
                rv = "FireEvent is null";
            }
            return rv;
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
