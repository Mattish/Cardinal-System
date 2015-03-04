using System;
using System.Collections.Generic;
using Cardinal_System_Common;
using Cardinal_System_Shared;

namespace Cardinal_System_HeathCliff
{
    public class NetworkComponent
    {
        public long ComponentId { get; private set; }
        public ComponentType ComponentType { get; private set; }
        public ComponentConnection ComponentConnection { get; private set; }
        public DateTime LastHeartbeatReceived { get; set; }
        public Dictionary<long, NetworkComponent> ConnectedComponents { get; private set; }


        public NetworkComponent(long componentId, ComponentType componentType, ComponentConnection componentConnection)
        {
            ComponentId = componentId;
            ComponentType = componentType;
            ComponentConnection = componentConnection;
            LastHeartbeatReceived = DateTime.UtcNow;
            ConnectedComponents = new Dictionary<long, NetworkComponent>();
        }

        public void AddConnectionTo(NetworkComponent networkComponent)
        {
            ConnectedComponents[networkComponent.ComponentId] = networkComponent;
        }

        public void RemoveConnectionTo(NetworkComponent networkComponent)
        {
            ConnectedComponents.Remove(networkComponent.ComponentId);
        }
    }
}