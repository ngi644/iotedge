// Copyright (c) Microsoft. All rights reserved.

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
            //rule.AddConverter<string, Message>(StringToMessageConverter);
            rule.BindToTrigger<Message>(bindingProvider);

            var rule2 = context.AddBindingRule<EdgeHubAttribute>();
            rule2.BindToCollector<Message>(typeof(EdgeHubCollectorBuilder));
        }

        private string MessageToStringConverter(Message arg)
        {
            string body = System.Text.Encoding.UTF8.GetString(arg.GetBytes());

            JObject bodyObject = new JObject();
            var messageJson = JsonConvert.DeserializeObject(body);
            bodyObject["body"] = messageJson as JToken;

            var applicationProperties = JsonConvert.SerializeObject(arg.Properties);
            bodyObject["applicationProperties"] = applicationProperties;

            return JsonConvert.SerializeObject(bodyObject);
        }
    }
}
