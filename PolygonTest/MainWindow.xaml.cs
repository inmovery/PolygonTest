using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PolygonTest {
    public partial class MainWindow : Window {

        Polygon polygon = new Polygon();
        List<Segment> segments = new List<Segment>();
        double result = 0;

        public MainWindow() {
            InitializeComponent();

            PlotView pv = new PlotView();
            Graph.Model = new PlotModel();

            CreatePolygon();
            CreateSegments();

            Graph.Model.InvalidatePlot(true);

        }

        public void OnClickCalculate(object sender, RoutedEventArgs e) {
            result = Calculate();
            MessageBox.Show(result.ToString());
            Graph.Model.InvalidatePlot(true);
        }

        /// <summary>
        /// Вычисление суммы
        /// </summary>
        /// <returns></returns>
        private double Calculate() {
            
            List<double> values = new List<double>();

            // Проверяем координаты концов отрезков на принадлежность многоугольнику
            for(int i = 0; i < segments.Count; i++) {

                //MessageBox.Show((i + 1).ToString());

                List<int> inter = new List<int>(); // Список индексов рёбер многоугольника, с которыми пересекается отрезок

                // Если оба конца принадлежат многоугольнику
                if(CheckInPolygon(segments[i].Start) && CheckInPolygon(segments[i].End)) {
                    // Считаем количество пересечений отрезка с многоугольником
                    for(int j = 0; j < polygon.CountEdges; j++) {
                        // Если проверяемый отрезок пересекается с ребром
                        if(IntersectDirect(polygon.Edges[j].Start, polygon.Edges[j].End, segments[i].Start, segments[i].End)) {
                            // то записываем индекс ребра
                            inter.Add(j);
                        }                        
                    }
                    // Если точек пересечения две
                    if(inter.Count == 2) {
                        // Вычисляем координаты точек пересечения
                        Point first = FindLineIntersection(segments[i].Start, segments[i].End, polygon.Edges[inter[0]].Start, polygon.Edges[inter[0]].End);
                        Point second = FindLineIntersection(segments[i].Start, segments[i].End, polygon.Edges[inter[1]].Start, polygon.Edges[inter[1]].End);

                        // Вычисляем нарезку длин
                        double first_part = getLength(first, segments[i].Start, i);
                        double second_part = getLength(second, segments[i].End, i);

                        FunctionSeries segLine = new FunctionSeries();
                        segLine.Color = OxyColors.Red;

                        segLine.Points.Add(new DataPoint(first.X, first.Y));
                        segLine.Points.Add(new DataPoint(second.X, second.Y));

                        segLine.Points.Add(new DataPoint(segments[i].Start.X, segments[i].Start.Y));
                        segLine.Points.Add(new DataPoint(segments[i].End.X, segments[i].End.Y));
                        Graph.Model.Series.Add(segLine);

                        values.Add(first_part + second_part);
                    }
                    // Если только одна точка пересечения
                    else if(inter.Count == 1) {

                        FunctionSeries segLine = new FunctionSeries();
                        segLine.Color = OxyColors.Red;
                        segLine.Points.Add(new DataPoint(segments[i].Start.X, segments[i].Start.Y));
                        segLine.Points.Add(new DataPoint(segments[i].End.X, segments[i].End.Y));
                        Graph.Model.Series.Add(segLine);

                        // то берём всю длину
                        values.Add(getLength(segments[i].Start, segments[i].End, i));
                    } else {

                        FunctionSeries segLine = new FunctionSeries();
                        segLine.Color = OxyColors.Red;
                        segLine.Points.Add(new DataPoint(segments[i].Start.X, segments[i].Start.Y));
                        segLine.Points.Add(new DataPoint(segments[i].End.X, segments[i].End.Y));
                        Graph.Model.Series.Add(segLine);

                        values.Add(getLength(segments[i].Start, segments[i].End, i));
                    }
                }
                // Если многоугольнику принадлежит только начальная точка отрезка
                else if(CheckInPolygon(segments[i].Start) && !CheckInPolygon(segments[i].End)) {

                    // Считаем количество пересечений отрезка с многоугольником
                    for(int j = 0; j < polygon.CountEdges; j++) {
                        // Если проверяемый отрезок пересекается с ребром
                        if(IntersectDirect(segments[i].Start, segments[i].End, polygon.Edges[j].Start, polygon.Edges[j].End)) {
                            // то записываем индекс ребра
                            inter.Add(j);
                        }
                    
                    }

                    Point general = FindLineIntersection(segments[i].Start, segments[i].End, polygon.Edges[inter[0]].Start, polygon.Edges[inter[0]].End);

                    FunctionSeries segLine = new FunctionSeries();
                    segLine.Color = OxyColors.Red;
                    segLine.Points.Add(new DataPoint(segments[i].Start.X, segments[i].Start.Y));
                    segLine.Points.Add(new DataPoint(general.X, general.Y));
                    Graph.Model.Series.Add(segLine);

                    values.Add(getLength(segments[i].Start, general, i));
                }
                // Если многоугольнику принадлежит только конечная точка отрезка
                else if(!CheckInPolygon(segments[i].Start) && CheckInPolygon(segments[i].End)) {

                    // Считаем количество пересечений отрезка с многоугольником
                    for(int j = 0; j < polygon.CountEdges; j++) {
                        // Если проверяемый отрезок пересекается с ребром
                        if(IntersectDirect(polygon.Edges[j].Start, polygon.Edges[j].End, segments[i].Start, segments[i].End)) {
                            // то записываем индекс ребра
                            inter.Add(j);
                        }

                    }

                    Point general = FindLineIntersection(segments[i].Start, segments[i].End, polygon.Edges[inter[0]].Start, polygon.Edges[inter[0]].End);

                    FunctionSeries segLine = new FunctionSeries();
                    segLine.Color = OxyColors.Red;
                    segLine.Points.Add(new DataPoint(general.X, general.Y));
                    segLine.Points.Add(new DataPoint(segments[i].End.X, segments[i].End.Y));
                    Graph.Model.Series.Add(segLine);

                    values.Add(getLength(general, segments[i].End, i));
                }
                // Если оба конца не принадлежат многоугольнику
                else {
                    // Считаем количество пересечений отрезка с многоугольником
                    for(int j = 0; j < polygon.CountEdges; j++) {
                        // Если проверяемый отрезок пересекается с ребром
                        if(IntersectDirect(polygon.Edges[j].Start, polygon.Edges[j].End, segments[i].Start, segments[i].End)) {
                            // то записываем индекс ребра
                            inter.Add(j);
                        }
                        
                    }

                    // Если точек пересечения две
                    if(inter.Count == 2) {
                        // Вычисляем координаты точек пересечения
                        Point first = FindLineIntersection(segments[i].Start, segments[i].End, polygon.Edges[inter[0]].Start, polygon.Edges[inter[0]].End);
                        Point second = FindLineIntersection(segments[i].Start, segments[i].End, polygon.Edges[inter[1]].Start, polygon.Edges[inter[1]].End);

                        FunctionSeries segLine = new FunctionSeries();
                        segLine.Color = OxyColors.Red;
                        segLine.Points.Add(new DataPoint(first.X, first.Y));
                        segLine.Points.Add(new DataPoint(second.X, second.Y));
                        Graph.Model.Series.Add(segLine);

                        values.Add(getLength(first, second, i));
                    }
                    // Если точек пересечения четыре
                    else if(inter.Count == 4) {
                        Point first = FindLineIntersection(segments[i].Start, segments[i].End, polygon.Edges[inter[0]].Start, polygon.Edges[inter[0]].End);
                        Point second = FindLineIntersection(segments[i].Start, segments[i].End, polygon.Edges[inter[1]].Start, polygon.Edges[inter[1]].End);
                        Point third = FindLineIntersection(segments[i].Start, segments[i].End, polygon.Edges[inter[2]].Start, polygon.Edges[inter[2]].End);
                        Point fouth = FindLineIntersection(segments[i].Start, segments[i].End, polygon.Edges[inter[3]].Start, polygon.Edges[inter[3]].End);
                        double first_part = getLength(first, second, i);
                        double second_part = getLength(third, fouth, i);

                        FunctionSeries segLine = new FunctionSeries();
                        segLine.Color = OxyColors.Red;

                        segLine.Points.Add(new DataPoint(first.X, first.Y));
                        segLine.Points.Add(new DataPoint(second.X, second.Y));

                        segLine.Points.Add(new DataPoint(third.X, third.Y));
                        segLine.Points.Add(new DataPoint(fouth.X, fouth.Y));

                        segLine.Points.Add(new DataPoint(segments[i].Start.X, segments[i].Start.Y));
                        segLine.Points.Add(new DataPoint(segments[i].End.X, segments[i].End.Y));
                        Graph.Model.Series.Add(segLine);

                        values.Add(first_part + second_part);
                    }
                    // Если только одна точка пересечения, то - это точка касания
                    else if(inter.Count == 1) {
                        // которая засчитывается как 0
                        values.Add(0);
                    }
                }

                inter = new List<int>();

            }

            return values.Sum();
        }

        /// <summary>
        /// Ручное вычисление корня числа (для больших дробных чисел)
        /// </summary>
        /// <param name="x"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        public decimal Sqrt(decimal x, decimal epsilon = 0.0M) {
            if(x < 0)
                throw new OverflowException("Cannot calculate square root from a negative number");

            decimal current = (decimal)Math.Sqrt((double)x), previous;
            do {
                previous = current;
                if(previous == 0.0M)
                    return 0;
                current = (previous + x / previous) / 2;
            }
            while(Math.Abs(previous - current) > epsilon);
            return current;
        }

        /// <summary>
        /// Вычисление длины отрезка
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public double getLength(Point first, Point second, int segment) {
            double one = second.X - first.X;
            double two = second.Y - first.Y;
            double x_res = Math.Pow(one, 2);
            double y_res = Math.Pow(two, 2);
            decimal xy_res = (decimal)x_res + (decimal)y_res;
            decimal resp = Sqrt(xy_res);
            //MessageBox.Show("Отрезок №" + (segment + 1).ToString() + "\n\nFirst:\n" + first.ToString() + "\n\n" + "Second:\n" + second.ToString() +
            //                "\nX: " + second.X.ToString() + " - " + first.X.ToString() + " = " + one.ToString() +
            //                "\nY: " + second.Y.ToString() + " - " + first.Y.ToString() + " = " + two.ToString() +
            //                "\n\nSqrt( (" + one.ToString() + " + " + two.ToString() + ")^2 " + ") = " + resp.ToString());

            return (double)resp;
        }

        /// <summary>
        /// Проверка пересечения отрезка и ребра многоугольника
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="d"></param>
        /// <returns></returns>
        private bool IntersectDirect(Point a, Point b, Point c, Point d) {
            
            double ax1 = a.X;
            double ay1 = a.Y;

            double ax2 = b.X;
            double ay2 = b.Y;

            double bx1 = c.X;
            double by1 = c.Y;

            double bx2 = d.X;
            double by2 = d.Y;


            double v1 = (bx2 - bx1) * (ay1 - by1) - (by2 - by1) * (ax1 - bx1);
            double v2 = (bx2 - bx1) * (ay2 - by1) - (by2 - by1) * (ax2 - bx1);
            double v3 = (ax2 - ax1) * (by1 - ay1) - (ay2 - ay1) * (bx1 - ax1);
            double v4 = (ax2 - ax1) * (by2 - ay1) - (ay2 - ay1) * (bx2 - ax1);
            return (v1 * v2 < 0) && (v3 * v4 < 0);

        }

        /// <summary>
        /// Построение уравнения прямой
        /// </summary>
        /// <param name="p1">Первая точка</param>
        /// <param name="p2">Вторая точка</param>
        /// <returns>Коэффициенты A, B и C</returns>
        public double[] LineEquation(Point p1, Point p2) {
            double  A = p2.Y - p1.Y;
            double B = p1.X - p2.X;
            double C = -p1.X * (p2.Y - p1.Y) + p1.Y * (p2.X - p1.X);
            return new double[] {A, B, C};
        }

        /// <summary>
        /// Поиск координат точки пересечения
        /// </summary>
        /// <param name="start1"></param>
        /// <param name="end1"></param>
        /// <param name="start2"></param>
        /// <param name="end2"></param>
        /// <returns></returns>
        private Point FindLineIntersection(Point start1, Point end1, Point start2, Point end2) {

            double[] seg1 = LineEquation(start1, end1);
            double a1 = seg1[0];
            double b1 = seg1[1];
            double c1 = seg1[2];

            double[] seg2 = LineEquation(start2, end2);
            double a2 = seg2[0];
            double b2 = seg2[1];
            double c2 = seg2[2];

            double d = (double)(a1 * b2 - b1 * a2);
            double dx = (double)(-c1 * b2 + b1 * c2);
            double dy = (double)(-a1 * c2 + c1 * a2);
            return new Point(dx / d, dy / d);
        }

        /// <summary>
        /// Проверка принадлежности точки многоугольнику
        /// </summary>
        /// <returns>true/false</returns>
        private bool CheckInPolygon(Point test) {
            int npol = polygon.CountVertices;
            int j = npol - 1;
            bool c = false;
            for(int i = 0; i < npol; i++) {
                if((((polygon.Vertices[i].Y <= test.Y) && (test.Y < polygon.Vertices[j].Y)) || ((polygon.Vertices[j].Y <= test.Y) && (test.Y < polygon.Vertices[i].Y))) &&
                    (test.X > (polygon.Vertices[j].X - polygon.Vertices[i].X) * (test.Y - polygon.Vertices[i].Y) / (polygon.Vertices[j].Y - polygon.Vertices[i].Y) + polygon.Vertices[i].X)) {
                    c = !c;
                }
                j = i;
            }
            return c;
        }

        /// <summary>
        /// Создание многоугольника
        /// </summary>
        private void CreatePolygon() {
            polygon.AddVertex(new Point(6254502.76289210, 7959144.61893362)); // 6254502.76284217, 7959144.61893362
            polygon.AddVertex(new Point(6254354.66776772, 7958316.63171740)); // 6254354.66756772, 7958316.63171740
            polygon.AddVertex(new Point(6255391.33448884, 7957811.76146361)); // 6255391.33448884, 7957811.76146361
            polygon.AddVertex(new Point(6256148.63986953, 7958521.94542061)); // 6256148.63986953, 7958521.94562061
            polygon.AddVertex(new Point(6255677.42463366, 7959386.95665544)); // 6255677.42763266, 7959386.95665544
            polygon.AddVertex(new Point(6255027.82490611, 7959514.85211973)); // 6255027.82790611, 7959514.85711973

            for(int i = 0; i < polygon.CountVertices; i++) {
                // Если достигнута последняя вершина
                if(i == polygon.CountVertices - 1) {
                    // то соединяем с первой
                    polygon.AddEdge(new Segment(polygon.Vertices[i], polygon.Vertices[0]));
                } else {
                    polygon.AddEdge(new Segment(polygon.Vertices[i], polygon.Vertices[i + 1]));
                }
            }

            FunctionSeries lines = new FunctionSeries();

            for(int i = 0; i < polygon.CountVertices; i++) {
                lines.Points.Add(new DataPoint(polygon.Vertices[i].X, polygon.Vertices[i].Y));
            }
            lines.Points.Add(new DataPoint(polygon.Vertices[0].X, polygon.Vertices[0].Y));
            Graph.Model.Series.Add(lines);

        }

        /// <summary>
        /// Создание отрезков
        /// </summary>
        private void CreateSegments() {
            segments.Add(new Segment(new Point(6254708.076745, 7958521.945621), new Point(6254024.819002, 7959228.763976)));
            segments.Add(new Segment(new Point(6254024.819002, 7959228.763976), new Point(6254708.076745, 7958521.945621)));
            segments.Add(new Segment(new Point(6256020.739405, 7957986.783152), new Point(6254300.814741, 7959545.149335)));
            segments.Add(new Segment(new Point(6254300.814741, 7959545.149335), new Point(6256020.739405, 7957986.783152)));
            segments.Add(new Segment(new Point(6255293.726240, 7958461.361190), new Point(6255519.234953, 7958451.263785)));
            segments.Add(new Segment(new Point(6255519.234953, 7958451.263785), new Point(6255293.726240, 7958461.361190)));
            segments.Add(new Segment(new Point(6255293.726240, 7958461.361190), new Point(6255111.972948, 7959043.644883)));
            segments.Add(new Segment(new Point(6255111.972948, 7959043.644883), new Point(6255293.726240, 7958461.361190)));
            segments.Add(new Segment(new Point(6255438.455713, 7959710.073618), new Point(6255111.972948, 7959043.644883)));
            segments.Add(new Segment(new Point(6255111.972948, 7959043.644883), new Point(6255438.455713, 7959710.073618)));
            segments.Add(new Segment(new Point(6255876.009932, 7959649.489187), new Point(6255438.455713, 7959710.073618)));
            segments.Add(new Segment(new Point(6255438.455713, 7959710.073618), new Point(6255876.009932, 7959649.489187)));
            segments.Add(new Segment(new Point(6256138.542464, 7959195.105959), new Point(6255876.009932, 7959649.489187)));
            segments.Add(new Segment(new Point(6255876.009932, 7959649.489187), new Point(6256138.542464, 7959195.105959)));
            segments.Add(new Segment(new Point(6256138.542464, 7959195.105959), new Point(6256475.122634, 7958979.694651)));
            segments.Add(new Segment(new Point(6256475.122634, 7958979.694651), new Point(6256138.542464, 7959195.105959)));
            segments.Add(new Segment(new Point(6256475.122634, 7958979.694651), new Point(6256357.319574, 7958720.527920)));
            segments.Add(new Segment(new Point(6256357.319574, 7958720.527920), new Point(6256475.122634, 7958979.694651)));
            segments.Add(new Segment(new Point(6256485.220039, 7958289.705304), new Point(6256434.733013, 7957714.153215)));
            segments.Add(new Segment(new Point(6256434.733013, 7957714.153215), new Point(6256485.220039, 7958289.705304)));
            segments.Add(new Segment(new Point(6256020.739405, 7957986.783152), new Point(6255519.234953, 7958451.263785)));
            segments.Add(new Segment(new Point(6255519.234953, 7958451.263785), new Point(6256020.739405, 7957986.783152)));
            segments.Add(new Segment(new Point(6256138.542464, 7959195.105959), new Point(6255027.827906, 7957589.618552)));
            segments.Add(new Segment(new Point(6255027.827906, 7957589.618552), new Point(6256138.542464, 7959195.105959)));
            segments.Add(new Segment(new Point(6254623.931703, 7957919.467118), new Point(6256485.220039, 7958289.705304)));
            segments.Add(new Segment(new Point(6256485.220039, 7958289.705304), new Point(6254623.931703, 7957919.467118)));
            segments.Add(new Segment(new Point(6256138.542464, 7959195.105959), new Point(6253826.236702, 7958010.343763)));
            segments.Add(new Segment(new Point(6253826.236702, 7958010.343763), new Point(6256138.542464, 7959195.105959)));
            segments.Add(new Segment(new Point(6254024.819002, 7959228.763976), new Point(6256357.319574, 7958720.527920)));
            segments.Add(new Segment(new Point(6256357.319574, 7958720.527920), new Point(6254024.819002, 7959228.763976)));
            segments.Add(new Segment(new Point(6254300.814741, 7959545.149335), new Point(6253880.089529, 7958774.380748)));
            segments.Add(new Segment(new Point(6253880.089529, 7958774.380748), new Point(6254300.814741, 7959545.149335)));
            segments.Add(new Segment(new Point(6255293.726240, 7958461.361190), new Point(6254024.819002, 7959228.763976)));
            segments.Add(new Segment(new Point(6254024.819002, 7959228.763976), new Point(6255293.726240, 7958461.361190)));
            segments.Add(new Segment(new Point(6255438.455713, 7959710.073618), new Point(6255270.165628, 7957310.257012)));
            segments.Add(new Segment(new Point(6255270.165628, 7957310.257012), new Point(6255438.455713, 7959710.073618)));
            segments.Add(new Segment(new Point(6254300.814741, 7959545.149335), new Point(6256024.105207, 7957555.960535)));
            segments.Add(new Segment(new Point(6256024.105207, 7957555.960535), new Point(6254300.814741, 7959545.149335)));
            segments.Add(new Segment(new Point(6255438.455713, 7959710.073618), new Point(6256434.733013, 7957714.153215)));
            segments.Add(new Segment(new Point(6256434.733013, 7957714.153215), new Point(6255438.455713, 7959710.073618)));
            segments.Add(new Segment(new Point(6256020.739405, 7957986.783152), new Point(6254623.931703, 7957919.467118)));
            segments.Add(new Segment(new Point(6254623.931703, 7957919.467118), new Point(6256020.739405, 7957986.783152)));
            segments.Add(new Segment(new Point(6255027.827906, 7957589.618552), new Point(6254708.076745, 7958521.945621)));
            segments.Add(new Segment(new Point(6254708.076745, 7958521.945621), new Point(6255027.827906, 7957589.618552)));
            segments.Add(new Segment(new Point(6255519.234953, 7958451.263785), new Point(6254708.076745, 7958521.945621)));
            segments.Add(new Segment(new Point(6254708.076745, 7958521.945621), new Point(6255519.234953, 7958451.263785)));
            segments.Add(new Segment(new Point(6256475.122634, 7958979.694651), new Point(6255111.972948, 7959043.644883)));
            segments.Add(new Segment(new Point(6255111.972948, 7959043.644883), new Point(6256475.122634, 7958979.694651)));

            foreach(var segment in segments) {
                FunctionSeries segLine = new FunctionSeries();
                segLine.Color = OxyColors.Black;
                //if(!segment.InPolygon(poly))
                //    segLine.Color = OxyColors.Black;
                segLine.Points.Add(new DataPoint(segment.Start.X, segment.Start.Y));
                segLine.Points.Add(new DataPoint(segment.End.X, segment.End.Y));
                Graph.Model.Series.Add(segLine);
            }


        }


    }
}

