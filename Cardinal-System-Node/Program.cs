using System;
using System.Linq;
using System.Net;

namespace Cardinal_System_Node
{
    class Program
    {
        public static CsNode _node;
        static void Main(string[] args)
        {
            bool doLoop = true;
            while (doLoop)
            {
                var line = Console.ReadLine();
                if (line.StartsWith("QUIT"))
                    doLoop = false;
                else if (line.StartsWith("REGISTER "))
                {
                    string[] splitLine = line.Split(' ');
                    if (_node != null)
                        _node.SendRegister(new Tuple<long, long>(long.Parse(splitLine[1]), long.Parse(splitLine[2])));
                }
                else if (line.StartsWith("CONNECT "))
                {
                    string[] splitLine = line.Split(' ');
                    ConnectToNode(long.Parse(splitLine[1]), splitLine[2], int.Parse(splitLine[3]));
                }
                else if (line.StartsWith("DISCONNECT"))
                {
                    if (_node != null)
                    {
                        _node.Stop();
                        _node = null;
                    }
                }
                else if (line.StartsWith("CREATE"))
                {
                    _node.CreatePhysicalEntity();
                }
            }

            if (_node != null)
                _node.Stop();
        }

        static void ConnectToNode(long identity, string ipaddress, int port)
        {
            if (_node == null)
            {
                _node = new CsNode(identity, IPAddress.Parse(ipaddress), 25251, port);
                _node.Start();
            }
        }
    }
}
