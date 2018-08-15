using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.EdgeHub;
using Newtonsoft.Json;

namespace Functions.Samples
{
    public static class EdgeHubSamples
    {
        public static async Task FilterAndSend(
            [EdgeHubTrigger("input1")] Message messageReceived,
            [EdgeHub(OutputName = "output1")] IAsyncCollector<Message> output)
        {
            const int defaultTemperatureThreshold = 25;
            byte[] messageBytes = messageReceived.GetBytes();
            var messageString = System.Text.Encoding.UTF8.GetString(messageBytes);

            // Get message body, containing the Temperature data         
            var messageBody = JsonConvert.DeserializeObject<MessageBody>(messageString);

            if (messageBody != null && messageBody.Machine.Temperature > defaultTemperatureThreshold)
            {
                var filteredMessage = new Message(messageBytes);
                foreach (KeyValuePair<string, string> prop in messageReceived.Properties)
                {
                    filteredMessage.Properties.Add(prop.Key, prop.Value);
                }
                filteredMessage.Properties.Add("MessageType", "Alert");
                await output.AddAsync(filteredMessage).ConfigureAwait(false);
            }
        }

        public static async Task FilterStringAndSend(
            [EdgeHubTrigger("input1")] string messageReceived,
            [EdgeHub(OutputName = "output1")] IAsyncCollector<Message> output)
        {
            string s = "";
        }

        /// <summary>
        ///Body:
        ///{
        ///  “machine”:{
        ///    “temperature”:,
        ///    “pressure”:
        ///  },
        ///  “ambient”:{
        ///    “temperature”: , 
        ///    “humidity”:
        ///  }
        ///  “timeCreated”:”UTC iso format”
        ///}
        ///Units and types:
        ///Temperature: double, C
        ///Humidity: int, %
        ///Pressure: double, psi
        /// </summary>
        class MessageBody
        {
            public Machine Machine { get; set; }

            public Ambient Ambient { get; set; }

            public DateTime TimeCreated { get; set; }
        }

        class Machine
        {
            public double Temperature { get; set; }

            public double Pressure { get; set; }
        }

        class Ambient
        {
            public double Temperature { get; set; }

            public int Humidity { get; set; }
        }
    }
}