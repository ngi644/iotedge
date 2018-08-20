// Copyright (c) Microsoft. All rights reserved.

using Microsoft.Azure.WebJobs.Host.Bindings;
using Newtonsoft.Json.Linq;

namespace Microsoft.Azure.WebJobs.Extensions.EdgeHub.Config
{
    using System;
    using Microsoft.Azure.Devices.Client;
    using Microsoft.Azure.WebJobs.Description;
    using Microsoft.Azure.WebJobs.Host.Config;
    using Newtonsoft.Json;

    /// <summary>
    /// Extension configuration provider used to register EdgeHub triggers and binders
    /// </summary>
    [Extension("EdgeHub")]
    class EdgeHubExtensionConfigProvider : IExtensionConfigProvider
    {
        public void Initialize(ExtensionConfigContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var bindingProvider = new EdgeHubTriggerBindingProvider();
            var rule = context.AddBindingRule<EdgeHubTriggerAttribute>();
            rule.AddConverter<Message, string>(MessageToStringConverter);
            rule.AddConverter<Message, JObject>(MessageToJObjectConverter);
            
            rule.BindToTrigger<Message>(bindingProvider);

            var rule2 = context.AddBindingRule<EdgeHubAttribute>();
            rule2.BindToCollector<Message>(typeof(EdgeHubCollectorBuilder<>));
            rule2.AddConverter<string, Message>(StringToMessageConverter);
            //rule2.BindToCollector<Message>(typeof(EdgeHubCollectorBuilder));
        }

        private Message StringToMessageConverter(string arg)
        {
            return new Message();
        }

        private JObject MessageToJObjectConverter(Message msg)
        {
            string body = System.Text.Encoding.UTF8.GetString(msg.GetBytes());

            JObject messageObject = new JObject();
            var messageJson = JsonConvert.DeserializeObject(body);
            messageObject["body"] = messageJson as JToken;

            var applicationProperties = JsonConvert.SerializeObject(msg.Properties);
            messageObject["applicationProperties"] = applicationProperties;

            return messageObject;
        }

        private string MessageToStringConverter(Message msg)
        {
            JObject messageObject = MessageToJObjectConverter(msg);

            return JsonConvert.SerializeObject(messageObject);
        }
    }
}
