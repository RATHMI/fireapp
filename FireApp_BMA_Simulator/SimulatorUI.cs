using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FireApp.Domain;
using System.Net.Http;

namespace FireApp_BMA_Simulator
{
    public partial class SimulatorUI : Form
    {
        int testSourceId = 1;
        int testEventId = 1;    // should be incremented every time
        private static HttpClient httpClient;

        public SimulatorUI()
        {
            InitializeComponent();
            httpClient = new HttpClient();
        }

        private void cmdGenerateEvent_Click(object sender, EventArgs e)
        {
            FireEvent fe = new FireEvent();

            fe.Id = new FireEventId { SourceId = testSourceId, EventId = testEventId };
            fe.EventType = EventTypes.disfunction;
            fe.TimeStamp = DateTime.Now;
            fe.TargetId = "testmelder";
            fe.TargetDescription = "test";
            
        }

        public static R ServicePostCall<T, R>(string callAddress, T element)
        {
            HttpResponseMessage resp = httpClient.PostAsJsonAsync<T>(callAddress, element).Result;
            resp.EnsureSuccessStatusCode();
            return resp.Content.ReadAsAsync<R>().Result;
        }
    }
}
