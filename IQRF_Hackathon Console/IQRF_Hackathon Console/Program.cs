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

                /* connections strings for Microsoft Azure IoT Hub
                 * located in App.config in <appSettings> section
                 * fill with your connection string
                */
                string connectionString = ConfigurationManager.AppSettings["IoTHubConnString"];
                
                SendToAzure(DeviceClient.CreateFromConnectionString(connectionString, TransportType.Http1), _regex.Match(connectionString).Groups["device"].Value);
            }
        }


        private static void SendToAzure(DeviceClient client, string deviceName)
        {
            var data = new Data {
                Value = _random.Next(-10, DateTime.Now.Minute + 10), 
                Device = deviceName, Time = DateTime.Now
            };

            // Convert Data to JSON format
            var dataString = JsonConvert.SerializeObject(data);
            var message = new Message(Encoding.UTF8.GetBytes(dataString));

            // wait 1 second
            Thread.Sleep(TimeSpan.FromSeconds(1));

            /* send message to Azure, do not wait for response
             * If you want to wait for response, await this method
             */
            client.SendEventAsync(message);

            Console.WriteLine("Message sent...");
        }
    }
}
