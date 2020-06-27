using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolygonTest {
    public class Segment {
        /// <summary>
        /// Начальная точка отрезка
        /// </summary>
        public Point Start { get; set; }

        /// <summary>
        /// Конечная точка отрезка
        /// </summary>
        public Point End { get; set; }

        public Segment(Point mStart, Point mEnd) {
            this.Start = mStart;
            this.End = mEnd;
        }

    }
}
