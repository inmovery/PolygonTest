using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolygonTest {
    public class Polygon {
        
        /// <summary>
        /// Список вершин многоугольника
        /// </summary>
        public List<Point> Vertices { get; set; }

        /// <summary>
        /// Список рёбер многоугольника
        /// </summary>
        public List<Segment> Edges { get; set; }

        public int CountVertices => Vertices.Count;

        public int CountEdges => Edges.Count;

        public Polygon() {
            Vertices = new List<Point>();
            Edges = new List<Segment>();
        }

        /// <summary>
        /// Добавление новой вершины
        /// </summary>
        /// <param name="vertex"></param>
        public void AddVertex(Point vertex) {
            Vertices.Add(vertex);
        }

        /// <summary>
        /// Добавление нового ребра многоугольника
        /// </summary>
        /// <param name="edge"></param>
        public void AddEdge(Segment edge) {
            Edges.Add(edge);
        }

    }
}
