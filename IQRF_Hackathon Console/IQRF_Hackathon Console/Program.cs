using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace IQRF_Hackathon_Console
{
    class Program
    {
        private static readonly Random _random = new Random();
        private static CancellationTokenSource _tokenSource = new CancellationTokenSource();

        readonly static Regex _regex = new Regex(@"DeviceId=(?<device>[^;]*);");

        static void Main(string[] args)
        {
            while (true)
            {
                string connString = ConfigurationManager.AppSettings["IoTHubConnString"];
                DoWork(DeviceClient.CreateFromConnectionString(connString, TransportType.Http1), _regex.Match(connString).Groups["device"].Value);
            }
        }


        private static void DoWork(DeviceClient client, string deviceName)
        {
            var data = new Data { Value = _random.Next(-10, DateTime.Now.Minute + 10), Device = deviceName, Time = DateTime.Now };
            var dataString = JsonConvert.SerializeObject(data);
            var message = new Message(Encoding.UTF8.GetBytes(dataString));
            Thread.Sleep(TimeSpan.FromSeconds(1));
            client.SendEventAsync(message);
            Console.WriteLine("Message sent...");
        }
    }
}
