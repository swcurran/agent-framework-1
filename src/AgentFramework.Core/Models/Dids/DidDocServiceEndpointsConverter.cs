﻿using System;
using System.Collections.Generic;
using System.Text;
using AgentFramework.Core.Messages;
using AgentFramework.Core.Models.Credentials;
using AgentFramework.Core.Models.Proofs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AgentFramework.Core.Models.Dids
{
    internal class DidDocServiceEndpointsConverter : JsonConverter
    {
        /// <inheritdoc />
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) =>
            writer.WriteRawValue(JsonConvert.SerializeObject(value));

        /// <inheritdoc />
        public override bool CanWrite => false;

        /// <inheritdoc />
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            var items = JArray.Load(reader);

            IList<IDidDocServiceEndpoint> serviceEndpoints = new List<IDidDocServiceEndpoint>();
            foreach (var item in items)
            {
                IDidDocServiceEndpoint serviceEndpoint;
                switch (item["type"].ToObject<string>())
                {
                    case DidDocServiceEndpointTypes.IndyAgent:
                        serviceEndpoint = new IndyAgentDidDocService();
                        break;
                    default: throw new TypeLoadException("Unsupported serialization type.");
                }

                serializer.Populate(item.CreateReader(), serviceEndpoint);
                serviceEndpoints.Add(serviceEndpoint);
            }
            return serviceEndpoints;
        }

        /// <inheritdoc />
        public override bool CanConvert(Type objectType) => true;
    }
}
