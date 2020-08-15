using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace FourRowClient
{
    class Ball
    {
        public Ellipse El { get; set; }
        public SolidColorBrush Collor { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double XMove { get; set; }
        public double YMove { get; set; }


        public Ball (Point p)
        {
            El = new Ellipse();
            XMove = 20;
            YMove = 20;
            X = p.X - El.Width / 2;
            Y = p.Y - El.Height / 2;

        }

        public Ball(Point p, double ew, double eh, double xsp, double ysp)
        {
            El = new Ellipse();
            El.Width = ew;
            El.Height = eh;
            X = p.X - (El.Width / 2);
            Y = p.Y - (El.Height / 2);
            XMove = xsp;
            YMove = ysp;

        }
    }
}
