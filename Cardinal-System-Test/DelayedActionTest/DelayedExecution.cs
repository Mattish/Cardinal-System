using System;
using System.Collections.Generic;
using System.Threading;

namespace Cardinal_System_Test.DelayedActionTest
{
    public class DelayedExecution
    {
        private readonly TimeSpan _actionDelay;
        private readonly List<DelayAction> _sortedDelayActions;
        private readonly object _setLock = new object();
        private readonly AutoResetEvent _autoResetEvent;

        public DelayedExecution(TimeSpan actionDelay)
        {
            _actionDelay = actionDelay;
            _sortedDelayActions = new List<DelayAction>();
            _autoResetEvent = new AutoResetEvent(false);
        }

        public int GetAmountOfDelayedActions()
        {
            lock (_setLock)
            {
                return _sortedDelayActions.Count;
            }
        }

        public void AddAction(DelayAction delayAction, TimeSpan extraActionTime = default(TimeSpan))
        {
            delayAction.TimeSent += _actionDelay;
            delayAction.TimeSent += extraActionTime;
            lock (_setLock)
            {
                _sortedDelayActions.Add(delayAction);
                _sortedDelayActions.Sort();
            }
            _autoResetEvent.Set();
        }

        private TimeSpan CheckDelayTillNextAction()
        {
            DelayAction delayAction;
            lock (_setLock)
            {
                if (_sortedDelayActions.Count == 0)
                    return TimeSpan.FromDays(1);
                delayAction = _sortedDelayActions[0];
            }
            TimeSpan delay = delayAction.TimeSent - DateTime.UtcNow;
            return delay > TimeSpan.Zero ? delay : TimeSpan.Zero;
        }

        public void CheckActions()
        {
            Console.WriteLine("Started CheckActions");
            while (true)
            {
                do
                {
                    var waitTime = CheckDelayTillNextAction();
                    if (!_autoResetEvent.WaitOne(waitTime))
                    {
                        DelayAction delayedAction;
                        DateTime dateTimeNow;
                        lock (_setLock)
                        {
                            delayedAction = _sortedDelayActions[0];
                            dateTimeNow = DateTime.UtcNow;
                            delayedAction.Action();
                            _sortedDelayActions.Remove(delayedAction);
                        }
                        Console.WriteLine("TimeToExecute:{0}.{1} ExecutedAt:{2}.{3} - {4}", 
                            delayedAction.TimeSent.Second, delayedAction.TimeSent.Millisecond, dateTimeNow.Second, dateTimeNow.Millisecond, delayedAction.Name);
                        _autoResetEvent.Reset();
                    }
                } while (false);
            }
        }
    }
}