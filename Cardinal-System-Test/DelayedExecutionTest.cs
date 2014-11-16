using System;
using System.Threading;
using System.Threading.Tasks;

namespace Cardinal_System_Test
{
    public class DelayedExecutionTest
    {
        /* Expected output order
         * 
            Started CheckActions
            Started CheckActions
            Started CheckActions
            Started CheckActions
            TimeToExecute:48.357 ExecutedAt:48.356 - box1ToBox2Action
            TimeToExecute:48.370 ExecutedAt:48.369 - box2P1AttackStart
            TimeToExecute:48.370 ExecutedAt:48.369 - box1P1AttackStart
            TimeToExecute:48.372 ExecutedAt:48.372 - box2ToBox1Action
            TimeToExecute:48.385 ExecutedAt:48.385 - box2P2Block
            TimeToExecute:48.385 ExecutedAt:48.385 - box1P2Block
            TimeToExecute:48.405 ExecutedAt:48.405 - box2P1Attack
            TimeToExecute:48.405 ExecutedAt:48.406 - box1P1Attack
        */
        static public void Main()
        {
            var delayedExecutionBox1 = new DelayedExecution(TimeSpan.FromMilliseconds(16));
            var delayedExecutionBox2 = new DelayedExecution(TimeSpan.FromMilliseconds(16));
            var delayedExecutionBox1NetworkToBox2 = new DelayedExecution(TimeSpan.FromMilliseconds(3));
            var delayedExecutionBox2NetworkToBox1 = new DelayedExecution(TimeSpan.FromMilliseconds(3));

            Task.Factory.StartNew(delayedExecutionBox1.CheckActions);
            Task.Factory.StartNew(delayedExecutionBox2.CheckActions);
            Task.Factory.StartNew(delayedExecutionBox1NetworkToBox2.CheckActions);
            Task.Factory.StartNew(delayedExecutionBox2NetworkToBox1.CheckActions);
            Thread.Sleep(1000);

            DelayAction box1P1AttackStart = new DelayAction("box1P1AttackStart");
            DelayAction box2P1AttackStart = new DelayAction("box2P1AttackStart");
            DelayAction box2P2Block = new DelayAction("box2P2Block");
            DelayAction box1P2Block = new DelayAction("box1P2Block");
            DelayAction box1ToBox2Action = new DelayAction("box1ToBox2Action");
            DelayAction box2ToBox1Action = new DelayAction("box2ToBox1Action");
            DelayAction box1P1Attack = new DelayAction("box1P1Attack");
            DelayAction box2P1Attack = new DelayAction("box2P1Attack");

            box1P1Attack.Action = () => { };
            box2P1Attack.Action = () => { };
            box1P2Block.Action = () => { };
            box2P2Block.Action = () => { };

            box1P1AttackStart.Action = () =>
            {
                box1P1Attack.TimeSent = DateTime.UtcNow;
                delayedExecutionBox1.AddAction(box1P1Attack, TimeSpan.FromMilliseconds(20));
            };

            box2P1AttackStart.Action = () =>
            {
                box2P2Block.TimeSent = DateTime.UtcNow;
                box1P2Block.TimeSent = DateTime.UtcNow;
                box2ToBox1Action.TimeSent = DateTime.UtcNow;
                box2P1Attack.TimeSent = DateTime.UtcNow;

                delayedExecutionBox2.AddAction(box2P2Block);
                delayedExecutionBox2NetworkToBox1.AddAction(box2ToBox1Action);
                delayedExecutionBox2.AddAction(box2P1Attack, TimeSpan.FromMilliseconds(20));
            };

            box1ToBox2Action.Action = () => delayedExecutionBox2.AddAction(box2P1AttackStart);

            box2ToBox1Action.Action = () => delayedExecutionBox1.AddAction(box1P2Block);

            box1P1AttackStart.TimeSent = DateTime.UtcNow;
            box2P1AttackStart.TimeSent = DateTime.UtcNow;
            box1ToBox2Action.TimeSent = DateTime.UtcNow;

            delayedExecutionBox1.AddAction(box1P1AttackStart);
            delayedExecutionBox1NetworkToBox2.AddAction(box1ToBox2Action);

            Console.ReadKey();
        }
    }
}