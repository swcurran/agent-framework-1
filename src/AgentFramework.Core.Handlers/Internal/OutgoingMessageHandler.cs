﻿using System.Collections.Generic;
using AgentFramework.Core.Contracts;
using System.Threading.Tasks;
using AgentFramework.Core.Decorators;
using AgentFramework.Core.Extensions;
using AgentFramework.Core.Messages;
using AgentFramework.Core.Utils;

namespace AgentFramework.Core.Handlers.Internal
{
    internal class OutgoingMessageHandler : MessageHandlerBase<OutgoingMessage>
    {
        private readonly IEnumerable<IOutgoingMessageDecoratorHandler> _outgoingHandlers;

        public OutgoingMessageHandler(IEnumerable<IOutgoingMessageDecoratorHandler> handlers)
        {
            _outgoingHandlers = handlers;
        }

        protected override async Task ProcessAsync(OutgoingMessage message, IAgentContext agentContext)
        {
            foreach (var handler in _outgoingHandlers)
                message = await handler.ProcessAsync(message, agentContext);

            var inner = await CryptoUtils.PackAsync(
                agentContext.Wallet, agentContext.Connection.TheirVk, agentContext.Connection.MyVk, message.OutboundMessage.Payload);

            if (agentContext is AgentContext context) 
                context.AddNext(new MessagePayload(new HttpOutgoingMessage { Message = inner.GetUTF8String() }));
        }
    }
}