using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cardinal_System_Common;
using Cardinal_System_Common.MessageNetworking;
using Cardinal_System_Shared;
using Cardinal_System_Shared.Component;

namespace Cardinal_System_HeathCliff
{
    public class ComponentManager : Getter<Heartbeat>
    {
        private readonly TimeSpan _heartbeatKeepsComponentAlive = TimeSpan.FromSeconds(2);
        private readonly TimeSpan _deadComponentGetsRemoved = TimeSpan.FromSeconds(4000); //TODO: Lower value irl
        private readonly TimeSpan _checkerLoopTime = TimeSpan.FromSeconds(2);

        readonly ConcurrentDictionary<long, NetworkComponent> _components = new ConcurrentDictionary<long, NetworkComponent>();
        private Task _heartbeatCheckerTask;
        private bool _heathbeatCheckerRunning;

        public ComponentManager()
        {
            _heathbeatCheckerRunning = true;
            _heartbeatCheckerTask = Task.Factory.StartNew(HeartbeatCheckerLoop, TaskCreationOptions.LongRunning);
            MessageHubV2.Register<ComponentConnectionDisconnect>(this, ComponentHasDiconnected);
        }

        private void ComponentHasDiconnected(ComponentConnectionDisconnect componentConnectionDisconnect)
        {
            var component = _components.FirstOrDefault(pair => pair.Value.ComponentConnection == componentConnectionDisconnect.ComponentConnection);
            if (component.Value != null)
            {
                NetworkComponent networkComponent;
                _components.TryRemove(component.Key, out networkComponent);
                // TODO: Shift the network?(consideration for distant future) [3/3] 
            }
        }

        public long AddAndConnectTo(long componentId, ComponentType componentType, ComponentConnection componentConnection)
        {
            long returnConnectId = _components.Count == 0 ? -1 : _components.First().Value.ComponentId;
            var result = _components.TryAdd(componentId, new NetworkComponent(componentId, componentType, componentConnection));
            if (!result)
                throw new Exception("Exception when trying to add new NetworkComponent to Manager");
            Console.WriteLine("Added NetworkComponent {0} - {1}", componentId, componentType);
            return returnConnectId;
        }

        public ComponentConnection GetComponentConnection(long componentId)
        {
            NetworkComponent componentConnection;
            if (_components.TryGetValue(componentId, out componentConnection))
            {
                return componentConnection.ComponentConnection;
            }
            return null;
        }

        protected override void SpecificAction(Heartbeat message)
        {
            var sourceComponentId = message.SourceComponent;
            NetworkComponent hcComponent;
            if (_components.TryGetValue(sourceComponentId, out hcComponent))
            {
                hcComponent.LastHeartbeatReceived = DateTime.UtcNow;
            }
        }

        private void HeartbeatCheckerLoop()
        {
            var componentsToRemove = new List<long>();
            while (_heathbeatCheckerRunning)
            {
                componentsToRemove.Clear();
                foreach (var heathCliffComponent in _components)
                {
                    var isAlive = (heathCliffComponent.Value.LastHeartbeatReceived + _heartbeatKeepsComponentAlive) > DateTime.UtcNow;
                    if (!isAlive)
                    {
                        var isDeadForLong = (heathCliffComponent.Value.LastHeartbeatReceived + _heartbeatKeepsComponentAlive + _deadComponentGetsRemoved) < DateTime.UtcNow;
                        if (isDeadForLong)
                        {
                            componentsToRemove.Add(heathCliffComponent.Key);
                        }
                    }
                }
                foreach (var componentId in componentsToRemove)
                {
                    NetworkComponent component;
                    _components.TryRemove(componentId, out component);
                    Console.WriteLine("Removing ComponentId:{0} because Dead", componentId);
                }
                Thread.Sleep(_checkerLoopTime); // TODO: Remove Thread.Sleep [3/3]
            }
        }
    }
}