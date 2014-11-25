using System;
using System.Net;
using Cardinal_System_Common;
using Cardinal_System_Shared.Dtos;
using Cardinal_System_Shared.Dtos.Component;
using Cardinal_System_Shared.Entity;

namespace Cardinal_System_Server
{
    class CsServerConnector : CsComponentConnection
    {
        public CsServerConnector(IPAddress initialAddress, int initialPort, Action<CsComponentConnection> disconnectAction)
            : base(initialAddress, initialPort, GotMessage, disconnectAction)
        {

        }

        private static void GotMessage(MessageDto dto, CsComponentConnection sender)
        {

        }

        public void StartTest2()
        {
            Start();
            SendMessage(new RegisterEntityInterestMessage
            {
                SourceId = new EntityId(CsNode.ComponentId, 10),
                TargetId = new EntityId(CsNode.ComponentId, 69),
                Type = MessageType.RegisterEntityInterest
            }.ToDto());
        }

        public void SendInfo()
        {
            SendMessage(new RegisterWithCircuitMessage
            {
                SourceId = new EntityId(CsNode.ComponentId, 0)
            }.ToDto());
        }

        public void SendRegister(EntityId entityNumber)
        {
            SendMessage(new RegisterEntityInterestMessage
            {
                SourceId = new EntityId(CsNode.ComponentId, 0),
                TargetId = entityNumber
            }.ToDto());
        }

        public void SendNewEntity(PhysicalEntity newEntity)
        {
            SendMessage(new RegisterEntityOwnerMessage
            {
                SourceId = newEntity.Id,
            }.ToDto());
        }
    }
}