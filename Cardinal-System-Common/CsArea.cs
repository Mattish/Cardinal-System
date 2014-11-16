using System;
using System.Collections.Concurrent;

namespace Cardinal_System_Common
{
    public class CsArea
    {
        public CsBox Box { get; private set; }
        public string Name { get; private set; }

        private readonly ConcurrentDictionary<string, CsBox> _transistionAreas;

        public CsArea(string name, Tuple<double, double> lowerRight, Tuple<double, double> upperLeft)
        {
            Name = name;
            Box = new CsBox(lowerRight, upperLeft);
            _transistionAreas = new ConcurrentDictionary<string, CsBox>();
        }

        public void AddTransitionArea(string name, CsBox transistionArea)
        {
            _transistionAreas.TryAdd(name, transistionArea);
        }
    }

    public class CsGrid
    {
        
    }

    public class CsCell
    {
    }

    public struct CsBox
    {
        public Tuple<double, double> UpperLeft { get; private set; }
        public Tuple<double, double> LowerRight { get; private set; }
        public Tuple<double, double> Centre { get; private set; }

        public CsBox(Tuple<double, double> upperLeft, Tuple<double, double> lowerRight)
            : this()
        {
            UpperLeft = upperLeft;
            LowerRight = lowerRight;
            Centre = new Tuple<double, double>((UpperLeft.Item1 - LowerRight.Item1) / 2D, (UpperLeft.Item2 - lowerRight.Item2) / 2D);
        }
    }
}
