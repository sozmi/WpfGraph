using System;
using System.Collections.Generic;

namespace Model
{
    /// <summary>
    /// Класс алгоритмов работы с графом
    /// </summary>
    public static class GraphAlg
    {
        #region Public Methods

        /// <summary>
        /// Очередь для индексов
        /// </summary>
        private static readonly Queue<int> result = new();

        /// <summary>
        /// Алгоритм Флёри
        /// </summary>
        /// <param name="lst">Список смежности вершин</param>
        /// <returns>Список индексов вершин</returns>
        public static int[] Fleury(List<Vertex> lst)
        {
            List<Vertex> v = Clone(ref lst);
            result.Clear();
            Fleury(v[0], v[0], ref v);
            return result.ToArray();
        }


        /// <summary>
        /// Проверка графа на связность
        /// </summary>
        /// <param name="n">Список смежности графа</param>
        /// <returns>Если граф связный - true, иначе false</returns>
        public static bool CheckConnectivity(List<Vertex> n)
        {
            VisitToFalse(ref n);
            Queue<Vertex> q = new();
            foreach (Vertex el in n)
                if (el.Vertices.Count != 0 && el.Status != Status.Passed)
                {
                    el.Status = Status.Visit;
                    q.Enqueue(el);
                    break;
                }
            while (q.Count != 0)
            {
                foreach (Vertex el in q.Dequeue().Vertices)
                {
                    if (el.Status == Status.NoVisit)
                    {
                        el.Status = Status.Visit;
                        q.Enqueue(el);
                    }
                }
            }
            foreach (Vertex el in n)
                if (el.Status == Status.NoVisit) return false;
            return true;
        }

        /// <summary>
        /// Алгоритм поиска в ширину
        /// </summary>
        /// <param name="lst"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static int[] BFS(int from, int to, List<Vertex> lst)
        {
            List<Vertex> v = Clone(ref lst);
            result.Clear();
            VisitToFalse(ref v);
            return BFS(v[from], v[to]);
        }

        /// <summary>
        /// Алгоритм поиска в ширину основанный на матрице смежности
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static int[] MatrixBFS(int from, int to, int[,] matrix)
        {
            return BFS(from, to, ref matrix);
        }

        /// <summary>
        /// Алгоритм поиска в глубину
        /// </summary>
        /// <param name="lst"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static int[] DFS(int from, int to, List<Vertex> lst)
        {
            List<Vertex> v = Clone(ref lst);
            result.Clear();
            VisitToFalse(ref v);
            return DFS(v[from], v[to]);
        }

        /// <summary>
        /// Алгоритм поиска в глубину на матрице смежности
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static int[] MatrixDFS(int from, int to, int[,] matrix)
        {
            return DFS(from, to, ref matrix);
        }

        /// <summary>
        /// Нахождение и печать минимальных расстояний между вершинами по алгоритму Дейкстры
        /// </summary>
        public static string Dijkstra(int S, int[,] adj_matrix, List<Vertex> v)
        {
            int INF = int.MaxValue;

            int N = adj_matrix.GetUpperBound(0) + 1;
            for (int i = 0; i < N; i++)
                for (int j = 0; j < N; j++)
                    if (adj_matrix[i, j] == 0) adj_matrix[i, j] = INF;


            bool[] Visited = new bool[N]; //массив значений посетили ли вершину
            int[] Distance = new int[N]; //массив меток вершин
            for (int i = 0; i < N; ++i)
                Distance[i] = INF;

            Distance[S] = 0; //метка исходной вершины равна 0, метки остальных INF
            int MinD; //наименьшая метка
            do
            {
                MinD = INF;
                int MinV = -1; //индекс вершины
                for (int i = 0; i < N; ++i) // теле цикла осуществляется поиск непосещённой вершины (MinV) с наименьшей меткой (MinD)
                    if (Distance[i] < MinD && !Visited[i])
                    {
                        MinD = Distance[i];
                        MinV = i;
                    }
                if (MinV == -1) //если непосещённых вершин не осталось
                    break;
                for (int i = 0; i < N; ++i)//обход всех соседей вершины MinV
                    if (adj_matrix[MinV, i] < INF && !Visited[i]) //если есть ребро и вершина не посещена

                        Distance[i] = Math.Min(Distance[i], Distance[MinV] + adj_matrix[MinV, i]);//если сумма метки найденной вершины + длина ребра до соседа
                                                                                                  //меньше метки этого соседа, то заменяет этой суммой метку соседа.

                Visited[MinV] = true; //найденную вершину отмечаем посещённой
            }
            while (MinD < INF); //пока найденная метка меньше «бесконечной»
                                // Console.WriteLine("Кратчайшие расстояния до вершин:");
            return AllWay(Distance, S, ref adj_matrix, ref v);
        }

        /// <summary>
        /// Нахождение и печать минимальных расстояний между вершинами по алгоритму Флойда
        /// </summary>
        public static string FloydWarshall(int[,] matrix, List<Vertex> v)
        {
            const int INF = 1000000;
            int N = v.Count;
            int[,] R = new int[N, N];
            string result = "";
            for (int i = 0; i < N; ++i)
                for (int j = 0; j < N; ++j)
                    if (matrix[i, j] == 0) matrix[i, j] = INF;
            //зануляем главную диагональ матрицы
            for (int i = 0; i < N; ++i)
                for (int j = 0; j < N; ++j)
                    R[i, j] = i == j ? 0 : matrix[i, j];
            //получаем минимальное  расстояние
            for (int k = 0; k < N; ++k)
                for (int i = 0; i < N; ++i)
                    for (int j = 0; j < N; ++j)
                        R[i, j] = Math.Min(R[i, j], R[i, k] + R[k, j]);
            foreach (Vertex el in v)
            {
                result += "\t" + el.Name;
            }
            for (int i = 0; i < N; ++i)
            {
                result += "\n" + v[i].Name;
                for (int j = 0; j < N; ++j)
                    result += "\t" + (R[i, j] == INF ? 0 : R[i, j]);
            }

            return result;
        }

        /// <summary>
        /// Демонстрация скобочной структуры
        /// </summary>
        public static string BraceStruct(List<Vertex> lst, bool Directed)
        {
            string res = "Демонстрация скобочной структуры: Вершина(Время)\n";

            //bool Directed = !IsDirectedGraph();
            int[] d = new int[lst.Count];
            int[] f = new int[lst.Count];
            int time = 0;
            int[] Components = new int[lst.Count];
            Stack<Vertex> VisitPath = new();
            List<Vertex> Cycle = new();

            int ComponentsCount = 0;
            for (int i = 0; i < lst.Count; ++i)
                if (Components[i] == 0)
                {
                    ComponentsCount++;
                    VisitPath.Push(lst[i]);
                    while (VisitPath.Count > 0)
                    {
                        Vertex V = VisitPath.Peek();
                        if (V.Status == Status.NoVisit)
                        {
                            V.Status = Status.Visit;
                            d[V.Index] = ++time;
                            res += "(" + V.Name + "(" + d[V.Index] + ")" + " ";
                            Components[V.Index] = ComponentsCount;
                        }
                        bool FoundNoVisit = false;
                        foreach (var el in V.Vertices)
                        {
                            if (el.Status == Status.Visit)
                            {
                                VisitPath.Pop();
                                int Prev = VisitPath.Count == 0 ? -1 : VisitPath.Peek().Index;
                                VisitPath.Push(V);
                                if (Directed || !Directed && el.Index != Prev)
                                {
                                    Cycle.Clear();
                                    while (el != VisitPath.Peek())
                                        Cycle.Insert(0, VisitPath.Pop());
                                    foreach (Vertex U in Cycle)
                                        VisitPath.Push(U);
                                    Cycle.Insert(0, el);
                                }
                            }
                            if (el.Status == Status.NoVisit)
                            {
                                FoundNoVisit = true;
                                VisitPath.Push(el);
                                break;
                            }
                        }
                        if (!FoundNoVisit)
                        {
                            f[V.Index] = ++time;
                            res += "" + V.Name + "(" + f[V.Index] + ")" + ") ";
                            V.Status = Status.Passed;
                            VisitPath.Pop();
                        }
                    }
                }
            return res;
        }

        /// <summary>
        /// Сильно связные компоненты
        /// </summary>
        /// <param name="lst"></param>
        /// <returns></returns>
        public static string SCC(List<Vertex> lst)
        {
            List<Vertex> v = Clone(ref lst);

            VisitToFalse(ref v);
            return StronglyConnectedComponents(v);
        }

        #endregion

        #region Methods private
        /// <summary>
        /// Клонирование списка смежности вершин
        /// </summary>
        /// <returns>списка смежных вершин</returns>
        private static List<Vertex> Clone(ref List<Vertex> v)
        {
            List<Vertex> res = new();
            for (int i = 0; i < v.Count; i++) //создаём вершины
                res.Add(v[i].Clone(i));

            for (int i = 0; i < v.Count; i++) //создаем связи между ними
            {
                Vertex cur = v[i];
                for (int j = 0; j < cur.Vertices.Count; j++)
                    res[i].Vertices.Add(res[cur.Vertices[j].Index]);
            }

            return res;
        }

        /// <summary>
        /// Поиск кратчайших путей 
        /// </summary>
        private static string AllWay(int[] D, int S, ref int[,] adj_matrix, ref List<Vertex> v)
        {
            int INF = int.MaxValue;
            int N = D.Length;
            for (int i = 0; i < N; i++)
                adj_matrix[i, i] = INF;

            string res = "Кратчайшие пути:\n";
            for (int i = 0; i < N; ++i)
                if (D[i] > 0 && D[i] < INF)
                {
                    int T = i;
                    string R = "";
                    while (T != S)
                    {
                        for (int j = 0; j < N; ++j)
                            if (adj_matrix[j, T] < INF && D[j] == D[T] - adj_matrix[j, T])
                            {
                                T = j;
                                R = v[T].Name + "-" + R;
                                break;
                            }
                    }
                    res += R + $"{v[i].Name}\n";
                }
            return res;
        }

        /// <summary>
        /// Алгоритм Флёри.
        /// Служит для нахождения Эйлерова цикла в 
        /// связном графе.
        /// </summary>
        /// <param name="current">Текущая вершина</param>
        /// <param name="start">Начальная вершина</param>
        /// <param name="v">Список вершин</param>
        private static void Fleury(Vertex current, Vertex start, ref List<Vertex> v)
        {
            result.Enqueue(current.Index); //добавление index вершины в результат
            for (int i = 0; i < current.Vertices.Count; i++) //перебор по порядку всех соседей вершины current
            {
                Vertex adj_vertex = current.Vertices[i];
                current.Vertices.Remove(adj_vertex); //удаление ребра
                adj_vertex.Vertices.Remove(current);

                if (current.Vertices.Count == 0 ||                          //если остался только один путь из вершины
                    (CheckConnectivity(v) &&                                //или если граф связный и
                    (start != adj_vertex || adj_vertex.Vertices.Count == 1))) //текущая вершина не равна начальной  или  остался ёщё 1 путь
                {
                    if (current.Vertices.Count == 0)
                        current.Status = Status.Passed;
                    Fleury(adj_vertex, start, ref v);
                    break;
                }
                current.Vertices.Add(adj_vertex);
                adj_vertex.Vertices.Add(current);
            }
        }

        /// <summary>
        /// В списке смежности для всех вершин устанавливается статус NoVisit
        /// </summary>
        /// <param name="n"></param>
        private static void VisitToFalse(ref List<Vertex> n)
        {
            for (int i = 0; i < n.Count; i++)
            {
                if (n[i].Status != Status.NoVisit)
                    n[i].Status = Status.NoVisit;
            }
        }

        /// <summary>
        /// Поиск в глубину
        /// <param name="start">Индекс вершины от которой ищем</param>
        /// <param name="end">Индекс вершины которую ищем</param>
        /// </summary>
        /// <returns>Возвращает наикратчайший путь, null-если путь не найден</returns>
        private static int[] DFS(Vertex from, Vertex to)
        {
            Stack<Vertex> q = new();
            q.Push(from);
            from.Status = Status.Visit;
            result.Enqueue(from.Index);
            while (q.Count != 0)
            {
                Vertex current = q.Pop();
                if (current.Vertices == null) break;//если смежных вершин нет
                foreach (Vertex el in current.Vertices)
                    if (el.Status == Status.NoVisit) //если не посещали вершину
                    {
                        q.Push(el);
                        el.Status = Status.Visit;
                        el.Parent = current;
                        if (el.Parent == el) continue; //если цикл

                        if (el == to)
                        {
                            List<int> ind = new();
                            Vertex temp = el;
                            while (temp != null)
                            {
                                ind.Add(temp.Index);
                                temp = temp.Parent;
                            }
                            ind.Reverse();
                            return ind.ToArray(); //если дошли до конца
                        }
                    }
            }
            return null;
        }

        /// <summary>
        /// Поиск в ширину
        /// <param name="start">Индекс вершины от которой ищем</param>
        /// <param name="end">Индекс вершины которую ищем</param>
        /// </summary>
        /// <returns>Возвращает наикратчайший путь, null-если путь не найден</returns>
        private static int[] BFS(Vertex from, Vertex to)
        {
            Queue<Vertex> q = new();
            q.Enqueue(from);
            from.Status = Status.Visit;
            result.Enqueue(from.Index);
            while (q.Count != 0)
            {
                Vertex current = q.Dequeue();
                if (current.Vertices == null) break;//если смежных вершин нет
                foreach (Vertex el in current.Vertices)
                    if (el.Status == Status.NoVisit) //если не посещали вершину
                    {
                        q.Enqueue(el);
                        el.Status = Status.Visit;
                        el.Parent = current;
                        if (el.Parent == el) continue; //если цикл

                        if (el == to)
                        {
                            List<int> ind = new();
                            Vertex temp = el;
                            while (temp != null)
                            {
                                ind.Add(temp.Index);
                                temp = temp.Parent;
                            }
                            ind.Reverse();
                            return ind.ToArray(); //если дошли до конца
                        }
                    }
            }
            return null;
        }

        /// <summary>
        /// Поиск в ширину по матрице смежности
        /// </summary>
        /// <param name="from">начальный индекс</param>
        /// <param name="to"></param>
        /// <returns>Возвращает путь</returns>
        private static int[] BFS(int from, int to, ref int[,] matrix)
        {
            int N = matrix.GetUpperBound(0) + 1;
            bool[] visited = new bool[N];
            int[] parent = new int[N];
            Queue<int> q = new();
            q.Enqueue(from);
            for (int i = 0; i < parent.Length; i++)
                parent[i] = -1;
            visited[from] = true;
            while (q.Count != 0)
            {
                int current = q.Dequeue();
                for (int i = 0; i < N; i++)
                {
                    if (matrix[current, i] != 0 && !visited[i]) //если есть вершина и её не посещали
                    {
                        visited[i] = true;
                        if (i == current) continue;
                        q.Enqueue(i);
                        parent[i] = current;
                        if (i == to)
                            return IndexsPath(i, parent);
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Поиск в глубину
        /// </summary>
        /// <param name="from">начальный индекс</param>
        /// <param name="to"></param>
        /// <returns>Возвращает путь</returns>
        private static int[] DFS(int from, int to, ref int[,] matrix)
        {
            int N = matrix.GetUpperBound(0) + 1;
            bool[] visited = new bool[N];
            Stack<int> st = new();
            int[] parent = new int[N];
            for (int i = 0; i < parent.Length; i++)
                parent[i] = -1;
            st.Push(from);
            while (st.Count != 0)
            {
                int current = st.Pop();
                visited[current] = true;
                for (int i = 0; i < N; i++)
                {
                    if (matrix[current, i] != 0 && !visited[i]) //если есть вершина и её не посещали
                    {
                        visited[i] = true;
                        if (i == current) continue;
                        st.Push(i);
                        parent[i] = current;
                        if (i == to)
                            return IndexsPath(i, parent);
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Поиск пути по индексам
        /// </summary>
        /// <param name="last">индекс последней вершины</param>
        /// <param name="parent">массив родителей</param>
        /// <returns></returns>
        private static int[] IndexsPath(int last, int[] parent)
        {
            List<int> indexs = new();
            while (last != -1)
            {
                indexs.Add(last);
                last = parent[last];
            }
            indexs.Reverse();
            return indexs.ToArray();
        }

        #region SCC
        private static int cnt; //счётчик времени обхода вершины
        private static int scnt; //счётчик компонент

        private static Stack<Vertex> additional_stack; //дополнительный стек для алгоритма Габова
        private static Stack<Vertex> path; //главный стек; путь обхода вершин

        static public void DFS_mod(Vertex v)
        {
            v.Depth = cnt++; //время когда выполнен обход вершины
            path.Push(v); //добавляем вершину в путь обхода
            additional_stack.Push(v);
            foreach (var neighbour in v.Vertices) //перебираем всех соседей обходимой вершины
            {
                if (neighbour.Depth == -1) //если обход соседа не выполнялся, то выполняем его обход
                {
                    DFS_mod(neighbour);
                }
                else if (neighbour.NumberComponent == -1) //если обход вершины уже когда-то был то....
                {
                    while (additional_stack.Peek().Depth > neighbour.Depth)
                        additional_stack.Pop();
                    //пока (обход вершины находящейся на вершине стека) происходил ПОЗЖЕ, чем (рассматриваемой вершины)
                    //вытаскиваем вершину из дополнительного стека
                }
            }
            //если рассматриваемая вершина находится на верхушке стека, то мы её удалям из стека
            if (additional_stack.Peek() == v)
                additional_stack.Pop();
            else return; //если утверждение выше не верно, то здесь рекурсивный метод останавливается 
            Vertex v2;
            do
            {
                (v2 = path.Pop()).NumberComponent = scnt; //достаём вершину из главного стека и присваиваем ей номер компоненты, которой она принадлежит
            } while (v2 != v); //и делаем это до тех пор, пока (вершина которая была на верхушке дополнительного стека) НЕ РАВНА (рассматриваемой вершине)
            scnt++; //номер следующей компоненты
        }

        /// <summary>
        /// Поиск сильно связных компонент
        /// </summary>
        /// <param name="vertices">Список смежности графа</param>
        /// <returns>Строку компонент</returns>
        private static string StronglyConnectedComponents(List<Vertex> vertices)
        {
            additional_stack = new(); path = new(); //инициализация стеков
            cnt = 0; scnt = 0;
            string res = "Сильно связные компоненты:";

            foreach (var v in vertices)
                if (v.Depth == -1) DFS_mod(v); // запускаем алгоритм, пока не выполнен обход всех вершин

            for (int i = 0; i < scnt; i++)// вывод компонент связности
            {
                res += $"\nКомпонента {i + 1}: ";
                foreach (var v in vertices)
                    if (v.NumberComponent == i)
                        res += v.Name + " ";
            }
            return res;
        }
        #endregion

        #endregion
    }
}

























