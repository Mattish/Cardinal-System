using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cardinal_System_Common.MessageNetworking
{
    public class MessageHubAction<T> : MessageHubActionBase
    {
        private readonly object _sender;
        private readonly Action<T> _actionToCall;
        private readonly WeakReference _reference;
        public MessageHubAction(object sender, Action<T> action)
        {
            _sender = sender;
            _actionToCall = action;
            _reference = new WeakReference(sender);
        }

        public override void Execute(object o)
        {
            var obj = (T)o;
            if (obj != null && _reference.IsAlive)
            {
                _actionToCall(obj);
            }
            else
            {
                MessageHubV2.Unregister<T>(_sender);
            }
        }
    }
    public abstract class MessageHubActionBase
    {
        public abstract void Execute(object o);
    }

    public static class MessageHubV2
    {
        private static readonly ConcurrentQueue<object> _queue = new ConcurrentQueue<object>();
        private static readonly ManualResetEventSlim _manualResetEventSlim = new ManualResetEventSlim();
        private static readonly ConcurrentDictionary<Type, List<Tuple<object, MessageHubActionBase>>> _actionConcurrentDictionary = new ConcurrentDictionary<Type, List<Tuple<object, MessageHubActionBase>>>();

        private static readonly Dictionary<Type, IEnumerable<Type>> superTypes = new Dictionary<Type, IEnumerable<Type>>();
        private static readonly object superTypesLock = new object();

        private static object _collectionLock = new object();
        private static Task _task;
        private static bool _isRunning;

        public static void Start()
        {
            if (!_isRunning)
            {
                _isRunning = true;
                _task = Task.Factory.StartNew(TaskRunner, TaskCreationOptions.LongRunning);
            }
        }

        public static void Stop()
        {
            _isRunning = false;
        }

        public static void TaskRunner()
        {
            var listOfActions = new List<MessageHubActionBase>();
            while (_isRunning)
            {
                object tempObj;
                while (!_queue.IsEmpty)
                {
                    if (_queue.TryDequeue(out tempObj))
                    {
                        listOfActions.Clear();
                        lock (_collectionLock)
                        {
                            var type = tempObj.GetType();
                            var types = GetSuperTypes(type);

                            foreach (var superType in types)
                            {
                                var list = _actionConcurrentDictionary.GetOrAdd(superType, new List<Tuple<object, MessageHubActionBase>>());
                                listOfActions.AddRange(list.Select(actionTuple => actionTuple.Item2));
                            }
                        }
                        foreach (var action in listOfActions)
                        {
                            action.Execute(tempObj);
                        }
                    }
                }
                _manualResetEventSlim.Wait(TimeSpan.FromMilliseconds(10));
                _manualResetEventSlim.Reset();
            }

        }

        public static void Register<T>(object sender, Action<T> action)
        {
            lock (_collectionLock)
            {
                var list = _actionConcurrentDictionary.GetOrAdd(typeof(T), new List<Tuple<object, MessageHubActionBase>>());
                var newMessageAction = new MessageHubAction<T>(sender, action);
                list.Add(new Tuple<object, MessageHubActionBase>(sender, newMessageAction));
            }
        }

        public static void Unregister<T>(object sender)
        {
            lock (_collectionLock)
            {
                var list = _actionConcurrentDictionary.GetOrAdd(typeof(T), new List<Tuple<object, MessageHubActionBase>>());
                var registered = list.Where(tuple => ReferenceEquals(tuple.Item1, sender)).ToArray();
                foreach (var tuple in registered)
                {
                    list.Remove(tuple);
                }
            }
        }

        public static void Send(object objectToSend)
        {
            _queue.Enqueue(objectToSend);
            _manualResetEventSlim.Set();
        }

        private static IEnumerable<Type> GetSuperTypes(Type typeToTest)
        {
            IEnumerable<Type> types;
            if (superTypes.TryGetValue(typeToTest, out types))
            {
                return types;
            }

            lock (superTypesLock) // TODO: Potentially dont need this lock due to lock in loop above
            {
                if (superTypes.TryGetValue(typeToTest, out types))
                {
                    return types;
                }

                var superTypesFound = FindSuperTypes(typeToTest);
                superTypes.Add(typeToTest, superTypesFound);
                return superTypesFound;
            }
        }

        private static List<Type> FindSuperTypes(Type typeToTest)
        {
            var interfaces = typeToTest.GetInterfaces();
            var baseClass = typeToTest.BaseType;
            var types = new List<Type>();

            if (baseClass != null)
            {
                types.AddRange(FindSuperTypes(baseClass));
            }

            foreach (var interfaceType in interfaces)
            {
                if (!types.Contains(interfaceType))
                {
                    types.Add(interfaceType);
                }
            }
            types.Add(typeToTest);
            return types;
        }
    }
}
