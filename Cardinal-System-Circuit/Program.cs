using System;

namespace Cardinal_System_Circuit
{
    class Program
    {
        static void Main(string[] args)
        {
            var circuit = new CsCircuit(25251);
            circuit.StartTest();
            Console.ReadKey();
        }
    }
}
