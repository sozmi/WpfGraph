using Model;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using WpfApp.View;

namespace WpfApp.ViewModels
{
    public static class Draw
    {
        /// <summary>
        /// Получение графического изображения вершины
        /// </summary>
        /// <param name="v">Вершина на основе которой выполняется построение</param>
        /// <returns>Графическое построение вершины</returns>
        public static VertexUC Vertex(Vertex v)
        {
            return new VertexUC()
            {
                MyText = v.Name,
                MyMargin = new System.Windows.Thickness(v.Point.X - 25, v.Point.Y - 25, 0, 0),
                Index = v.Index
            };
        }
        /// <summary>
        /// Получение графического представления ребра
        /// </summary>
        /// <param name="f">Координаты центра вершины из которой строим ребро</param>
        /// <param name="t">Координаты центра вершины в которую строим ребро</param>
        /// <param name="i">Надпись над ребром или null, если её нет</param>
        /// <param name="oriented">true-граф ориентирован, иначе -false</param>
        /// <returns>Графическое представление ребра</returns>
        public static EdgeUC Edge(DPoint f, DPoint t, string i, bool oriented)
        {
            double xMin = Math.Min(t.X, f.X), yMin = Math.Min(t.Y, f.Y);
            
            EdgeUC line = new()
            {
                PathData = DrawLine(GetPoint(f, t), GetPoint(t, f), oriented),
                Index = i,
                MyMargin = new Thickness(xMin, yMin, 0, 0)
            };
            return line;
        }

        /// <summary>
        /// Получение координат точек на окружности вершины
        /// </summary>
        /// <param name="f">Координаты центра вершины из которой строим ребро</param>
        /// <param name="t">Координаты центра вершины в которую строим ребро</param>
        /// <returns>Новые координаты</returns>
        private static Point GetPoint(DPoint f, DPoint t)
        {
            double xMin = Math.Min(t.X, f.X),
                yMin = Math.Min(t.Y, f.Y),
                x = Math.Abs(f.X - t.X),
                y = Math.Abs(f.Y - f.X),
                r = 25,
                y0 = y * r / Math.Sqrt(x * x + y * y),
                x0 = Math.Sqrt(r * r - y0 * y0);

            return new Point(f.X + Ratio(f.X, t.X, x0) - xMin, f.Y + Ratio(f.Y, t.Y, y0) - yMin);
        }

        /// <summary>
        /// Делает кооэффициент положительным или отрицательным в зависимости от того,
        /// какая точка ближе к началу системы координат
        /// </summary>
        /// <param name="x">Первая точка</param>
        /// <param name="x2">Вторая точка</param>
        /// <param name="i">Коэффициент</param>
        /// <returns>Коэффициент с соответствующим знаком</returns>
        private static double Ratio(double x, double x2, double i)
        {
            return x > x2 ? -i : i;
        }

        private static PolyLineSegment DrawArrow(Point a, Point b)
        {
            double HeadWidth = 10; // Ширина между ребрами стрелки
            double HeadHeight = 5; // Длина ребер стрелки

            double X1 = a.X;
            double Y1 = a.Y;

            double X2 = b.X;
            double Y2 = b.Y;

            double theta = Math.Atan2(Y1 - Y2, X1 - X2);
            double sint = Math.Sin(theta);
            double cost = Math.Cos(theta);

            Point pt3 = new(
                X2 + (HeadWidth * cost - HeadHeight * sint),
                Y2 + (HeadWidth * sint + HeadHeight * cost));

            Point pt4 = new(
                X2 + (HeadWidth * cost + HeadHeight * sint),
                Y2 - (HeadHeight * cost - HeadWidth * sint));

            PolyLineSegment arrow = new();
            arrow.Points.Add(b);
            arrow.Points.Add(pt3);
            arrow.Points.Add(pt4);
            arrow.Points.Add(b);

            return arrow;
        }

        private static PathGeometry DrawLine(Point start, Point end, bool oriented)
        {
            PathFigure pathFigure = new()
            {
                StartPoint = (System.Windows.Point)start,
                IsClosed = false
            };
            PathGeometry path = new();
            path.Figures.Add(pathFigure);
            Point startPoint = start;
            Point endPoint = end;

            //Кривая Безье
            Vector vector = endPoint - startPoint;
            Point point1 = new(startPoint.X + 3 * vector.X / 8,
                                     startPoint.Y + 1 * vector.Y / 8);
            Point point2 = new(startPoint.X + 5 * vector.X / 8,
                                     startPoint.Y + 7 * vector.Y / 8);

            BezierSegment curve = new(point1, point2, endPoint, true);


            path.Figures[0].Segments.Add(curve);
            if (oriented)
            {
                PolyLineSegment arrow = DrawArrow(point2, endPoint);
                path.Figures[0].Segments.Add(arrow);
            }
            return path;
        }
        /// <summary>
        /// Получение списка графических рёбер на основе графа
        /// </summary>
        /// <param name="g">Граф на основе которого выполняется построение</param>
        /// <returns>Список рёбер</returns>
        public static List<EdgeUC> Graph(Graph g)
        {
            List<EdgeUC> edges = new();
            foreach (Vertex v in g.Vertexes)
            {
                foreach (Vertex adj_v in v.Vertices)
                {
                    edges.Add(Edge(v.Point, adj_v.Point, null, g.IsOriented));
                }
            }
            return edges;
        }
    }
}
