using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolygonTest {
    public class Point {
        public double X { get; set; }

        public double Y { get; set; }

        public Point(double mX, double mY) {
            this.X = mX;
            this.Y = mY;
        }

        public override string ToString() {
            return "X = " + X.ToString() + "\nY = " + Y.ToString();
        }

    }
}
