using System;
using Cardinal_System_Common.MessageNetworking;
using Cardinal_System_Shared.Dto;

namespace Cardinal_System_Circuit
{
    public class Circuit : Node
    {
        public Circuit(string address, int port)
            : base(address, port)
        {
        }

        private void HandleDtoNotForUs(MessageDto messageDto)
        {
            if (ComponentConnections.ContainsKey(messageDto.TargetComponent))
            {
                ComponentConnections[messageDto.TargetComponent].SendMessageDto(messageDto);
                Console.WriteLine("Forwarding {0} to {1}", messageDto.Type, messageDto.TargetComponent);
            }
            else
            {
                throw new Exception("Received message not for us, but don't have connection to intended target");
            }
        }

        protected override void ComponentSpecificMessageRegisters()
        {
            MessageHubV2.Register<MessageDto>(this, HandleDtoNotForUs);
        }
    }
}