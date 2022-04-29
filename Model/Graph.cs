using System;
using System.Collections.Generic;

namespace Model
{
    /// <summary>
    /// Класс описывающий граф
    /// </summary>
    public class Graph
    {
        private const string VERTEX_EQUAL = "Ошибка. Значения начальной вершины и конечной совпадают.";

        #region Varaibles
        /// <summary>
        /// Список смежности в графе
        /// </summary>
        public List<Vertex> Vertexes { get; } = new();

        /// <summary>
        /// Определение является ли граф Эйлеровым. 
        /// Если граф - Эйлеров, то true, иначе false.
        /// </summary>
        public bool IsEuler
        {
            get
            {
                foreach (Vertex el in Vertexes)
                {
                    if (el.Vertices.Count % 2 != 0)
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        /// <summary>
        /// Определение является ли граф связным.
        /// Если граф - связный, то true, иначе false.
        /// </summary>
        public bool IsConnected => GraphAlg.CheckConnectivity(Vertexes);

        /// <summary>
        /// Определение является ли граф ориентированным.
        /// Если граф - ориентированный, то true, иначе false.
        /// </summary>
        public bool IsOriented { get; set; } = true;

        /// <summary>
        /// Последний уникальный индекс вершины
        /// </summary>
        private int LastIndex { get; set; }
        #endregion

        #region Construction
        /// <summary>
        /// Формирует граф на основе матрицы смежности
        /// </summary>
        /// <param name="matrix">Матрица смежности</param>
        public Graph(int[,] matrix)
        {
            int N = matrix.GetUpperBound(0) + 1;

            for (int i = 0; i < N; i++) //добавляем вершины
                AddVertex(default);

            for (int i = 0; i < N; i++) //добавляем рёбра
                for (int j = 0; j < N; j++)
                    if (matrix[i, j] != 0)
                        AddEdge(i, j);
        }
        /// <summary>
        /// Создание пустого графа
        /// </summary>
        public Graph() { }
        #endregion

        #region Public Methods
        /// <summary>
        /// Добавление вершины с координатами p в конец списка смежности
        /// </summary>
        /// <param name="p">координаты центра вершины</param>
        /// <returns>Индекс текущего элемента</returns>
        public int AddVertex(DPoint p)
        {
            Vertex n = new(Vertexes.Count, LastIndex++, new(p.X, p.Y));
            Vertexes.Add(n);
            return Vertexes.Count - 1;
        }

        /// <summary>
        /// Удаление вершины по индексу
        /// </summary>
        /// <param name="index">индекс вершины</param>
        public int RemoveVertex(int index)
        {
            Vertexes.RemoveAt(index);
            foreach (Vertex v in Vertexes)
                foreach (Vertex adj_v in v.Vertices)
                    if (adj_v.Index == index)
                    {
                        v.Vertices.Remove(adj_v);
                        break;
                    }

            for (int j = index; j < Vertexes.Count; j++) //уменьшаем все индексы
                Vertexes[j].Index -= 1;
            return index;
        }

        /// <summary>
        /// Добавление ребра между вершинами
        /// </summary>
        /// <param name="from">Индекс вершины от которой идёт ребро</param>
        /// <param name="to">Индекс вершины к которой идёт ребро</param>
        /// <returns>false-ребро не добавлено, true - связь создана </returns>
        public bool AddEdge(int from, int to)
        {
            if (Vertexes.Count == 0)
                throw new GraphException("Ошибка. Не удалось добавить рёбра. В графе нет вершин.");

            if (Vertexes[from].Vertices.IndexOf(Vertexes[to]) == -1)
            {
                Vertexes[from].Vertices.Add(Vertexes[to]);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Находит эйлерову цепь в графе
        /// </summary>
        /// <returns>Массив индексов для обхода по циклу Эйлера</returns>
        public int[] Fluery()
        {
            Validation();
            if(IsOriented)
                throw new GraphException("Ошибка. Алгоритм не может проверить является ли граф Эйлеровым для ориентированного графа.");
            if (!IsEuler)
                throw new GraphException("Ошибка. Не удалось применить алгоритм. Граф не Эйлеров");

            return GraphAlg.Fleury(Vertexes);
        }

        /// <summary>
        /// Метод поиска в ширину по списку смежности
        /// </summary>
        /// <param name="from">индекс вершины от которой ищем</param>
        /// <param name="to">индекс вершины от которую ищем</param>
        /// <returns>список индексов вершин</returns>
        public int[] BFS(int from, int to)
        {
            Validation();
            return from == to ? throw new GraphException(VERTEX_EQUAL) : GraphAlg.BFS(from, to, Vertexes);
        }

        /// <summary>
        /// Метод поиска в глубину по списку смежности
        /// </summary>
        /// <param name="from">индекс вершины от которой ищем</param>
        /// <param name="to">индекс вершины от которую ищем</param>
        /// <returns>список индексов вершин</returns>
        public int[] DFS(int from, int to)
        {
            Validation();
            if (from == to) throw new GraphException(VERTEX_EQUAL);
            return GraphAlg.DFS(from, to, Vertexes);
        }

        /// <summary>
        /// Метод поиска в глубину по матрице смежности
        /// </summary>
        /// <param name="from">индекс вершины от которой ищем</param>
        /// <param name="to">индекс вершины от которую ищем</param>
        /// <returns>список индексов вершин</returns>
        public int[] MatrixDFS(int from, int to)
        {
            Validation();
            return from == to ? throw new GraphException(VERTEX_EQUAL) : GraphAlg.MatrixDFS(from, to, ToMatrix());
        }

        /// <summary>
        /// Метод поиска в ширину по матрице смежности
        /// </summary>
        /// <param name="from">индекс вершины от которой ищем</param>
        /// <param name="to">индекс вершины от которую ищем</param>
        /// <returns>список индексов вершин</returns>
        public int[] MatrixBFS(int from, int to)
        {
            Validation();
            return from == to ? throw new GraphException(VERTEX_EQUAL) : GraphAlg.MatrixBFS(from, to, ToMatrix());
        }

        /// <summary>
        /// Вывод всех путей в графе
        /// </summary>
        /// <param name="index">Индекс вершины</param>
        /// <returns>Строку путей</returns>
        public string AllWay(int index)
        {
            Validation();
            return GraphAlg.Dijkstra(index, ToMatrix(), Vertexes);
        }

        /// <summary>
        /// По алгоритму Флойда-Уоршалла вычисляет матрицу путей
        /// </summary>
        /// <returns>Строку с записанной матрицей</returns>
        public string CostWay()
        {
            return GraphAlg.FloydWarshall(ToMatrix(), Vertexes);
        }

        /// <summary>
        /// Построение скобочной структуры графа
        /// </summary>
        /// <returns>Строку с демонстрацией скобочной структуры</returns>
        public string BraceStruct()
        {
            Validation();
            return GraphAlg.BraceStruct(Vertexes, true);
        }

        /// <summary>
        /// Поиск сильно связных компонент
        /// </summary>
        /// <returns></returns>
        public string SCC()
        {
            Validation();
            return GraphAlg.SCC(Vertexes);
        }

        /// <summary>
        /// Вывод списка смежности
        /// </summary>
        /// <returns></returns>
        public string Output(List<Vertex> n)
        {
            return OutputList(n ?? Vertexes);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Преобразование списка смежности в матрицу смежности
        /// </summary>
        /// <returns>матрицу смежности</returns>
        private int[,] ToMatrix()
        {
            int N = Vertexes.Count;
            int[,] matrix = new int[N, N];
            for (int i = 0; i < N; i++)
            {
                foreach (Vertex v in Vertexes[i].Vertices)
                {
                    matrix[i, v.Index] = 1;
                }
            }
            return matrix;
        }

        /// <summary>
        /// Вывод списка смежности
        /// </summary>
        /// <returns>Список смежности записанный в виде строки</returns>
        private static string OutputList(List<Vertex> n)
        {
            string res = "";
            foreach (Vertex el in n)
            {
                string vertex = "";
                foreach (Vertex v in el.Vertices)
                {
                    vertex += v.Name + ", ";
                }
                vertex = vertex.Trim(' ', ',');
                res = res + el.Name + ": " + (vertex == "" ? "нет" : vertex) + "\n";
            }
            return res;
        }

        /// <summary>
        /// Проверка есть ли в графе вершины и ребра
        /// </summary>
        private void Validation()
        {
            const string NOT_VERTICES = "Ошибка.Граф не содержит вершин.";
            const string NOT_EDGES = "Ошибка.Граф не содержит ребер.";
            if (Vertexes.Count == 0)
                throw new GraphException(NOT_VERTICES);
            bool error = true;
            foreach (Vertex v in Vertexes)
                if (v.Vertices.Count != 0)
                {
                    error = false;
                    break;
                }
            if (error)
                throw new GraphException(NOT_EDGES);
        }
        #endregion
    }
    /// <summary>
    /// Класс для вызова ошибок в графе
    /// </summary>
    public class GraphException : Exception
    {
        public GraphException(string message) : base(message) { }
    }
}
