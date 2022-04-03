using Model;
using Prism.Commands;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using WpfApp.View;

namespace WpfApp.ViewModels
{
    public class ViewModel : INotifyPropertyChanged
    {
        #region PropertyChanged
        /// <summary>
        /// Событие изменения свойства
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Изменение значения свойства
        /// </summary>
        /// <typeparam name="T">тип поля</typeparam>
        /// <param name="field">поле</param>
        /// <param name="newValue">новое значение</param>
        /// <param name="propertyName">наименование свойства</param>
        /// <returns></returns>
        protected bool SetProperty<T>(ref T field, T newValue, [CallerMemberName] string propertyName = null)
        {
            if (!Equals(field, newValue))
            {
                field = newValue;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
                return true;
            }
            return false;
        }
        #endregion
        #region Constante
        private static readonly SolidColorBrush brushUsually = Brushes.Orange;
        #endregion
        #region Construction
        public ViewModel()
        {
            VertexUC.Selected += new(Select);

            ClearDelegateCommand = new DelegateCommand(Clear);
            AddVertexDelegateCommand = new DelegateCommand<object>(AddVertex);
            DeleteVertexDelegateCommand = new DelegateCommand(DeleteVertex);
            RecreateEdgeDelegateCommand = new DelegateCommand(RecreateEdges);

            FlueryDelegateCommand = new DelegateCommand(Fluery);
            SCCDelegateCommand = new DelegateCommand(SCC);

            BFSDelegateCommand = new DelegateCommand(BFS);
            DFSDelegateCommand = new DelegateCommand(DFS);
            BFSMatrixDelegateCommand = new DelegateCommand(BFSMatrix);
            DFSMatrixDelegateCommand = new DelegateCommand(DFSMatrix);
            AllWaysDelegateCommand = new DelegateCommand(AllWays);
            CostWaysDelegateCommand = new DelegateCommand(CostWays);
            BraceStructDelegateCommand = new DelegateCommand(BraceStruct);
        }
        #endregion

        #region DelegateCommand
        public DelegateCommand<object> AddVertexDelegateCommand { get; set; }
        public DelegateCommand DeleteVertexDelegateCommand { get; set; }
        public DelegateCommand RecreateEdgeDelegateCommand { get; set; }
        public DelegateCommand ClearDelegateCommand { get; set; }

        public DelegateCommand FlueryDelegateCommand { get; set; }
        public DelegateCommand SCCDelegateCommand { get; set; }

        public DelegateCommand BFSDelegateCommand { get; set; }
        public DelegateCommand BFSMatrixDelegateCommand { get; set; }
        public DelegateCommand DFSMatrixDelegateCommand { get; set; }
        public DelegateCommand DFSDelegateCommand { get; set; }
        public DelegateCommand AllWaysDelegateCommand { get; set; }
        public DelegateCommand CostWaysDelegateCommand { get; set; }
        public DelegateCommand BraceStructDelegateCommand { get; set; }
        #endregion

        #region Command
        /// <summary>
        /// Добавляет новую вершину на холст
        /// </summary>
        /// <param name="p">Координаты мышки в момент нажатия</param>
        private void AddVertex(object p) 
        {
            Point pn = (Point)p;
            if (ModeAddVertex)
            {
                int i = graph.AddVertex(new(pn.X, pn.Y));
                CollectionVertex.Add(Draw.Vertex(graph.Vertexes[i]));
            }
        }

        /// <summary>
        /// Добавление ребра
        /// </summary>
        private void AddEdge()
        {
            int i0 = Indexs[0], i1 = Indexs[1];
            if (Oriented)
            {
                if ((!Oriented && graph.AddEdge(i0, i1) && graph.AddEdge(i1, i0)) || (Oriented && graph.AddEdge(i0, i1)))//если такого ребра ещё нет
                {
                    EdgeUC edge = Draw.Edge(GetPoint(i0), GetPoint(i1), null, graph.IsOriented);
                    CollectionEdge.Add(edge);
                }
                else
                {
                    _ = System.Windows.MessageBox.Show("Такое ребро уже существует.");
                }
                CollectionVertex[i0].MyColor = brushUsually;
                CollectionVertex[i1].MyColor = brushUsually;
            }
        }

        /// <summary>
        /// Восстановление рисунка рёбер
        /// </summary>
        private void RecreateEdges()
        {
            CollectionEdge.Clear();
            foreach (EdgeUC el in Draw.Graph(graph))
            {
                CollectionEdge.Add(el);
            }
        }

        /// <summary>
        /// Удаление вершины
        /// </summary>
        private void DeleteVertex()
        {
            if (!ModeDelete)
            {
                _ = MessageBox.Show("Ошибка. Включите режим удаления вершин.");
                return;
            }
            for (int i1 = 0; i1 < Indexs.Count; i1++)
            {
                for (int j = i1 + 1; j < Indexs.Count; j++)
                {
                    if (Indexs[j] > Indexs[i1])
                        Indexs[j]--;
                }
                int i = Indexs[i1];
                int index = graph.RemoveVertex(i);
                if (index == -1)
                {
                    _ = MessageBox.Show("Ошибка. Вершина не найдена.");
                    return;
                }
                else
                {
                    CollectionVertex.RemoveAt(index);
                    for (int j = index; j < CollectionVertex.Count; j++)
                    {
                        CollectionVertex[j].Index--;
                    }
                    RecreateEdges();
                }
            }
            Indexs.Clear();
        }

        /// <summary>
        /// Получение координат точки и снятие с нее выделения
        /// </summary>
        /// <param name="index">индекс точки</param>
        /// <returns>координаты точки</returns>
        private DPoint GetPoint(int index)
        {
            CollectionVertex[index].MyColor = brushUsually;
           
            return graph.Vertexes[index].Point;
        }

        /// <summary>
        /// Выделение вершин
        /// </summary>
        private void Select(int index)
        {
            if (ModeAddEdge || ModeDelete)
            {
                if (!Indexs.Remove(index))//если вершина уже была выделена, то она удалится и if не выполниться
                {
                    Indexs.Add(index);
                    CollectionVertex[index].MyColor = Brushes.Red;
                    if (ModeAddEdge)
                    {
                        if (Indexs.Count > 2)
                        {
                            _ = MessageBox.Show("Слишком много выделенных вершин. Невозможно создать ребро.");
                        }
                        else if (Indexs.Count == 2)
                        {
                            AddEdge();
                            Indexs.Clear();
                        }
                    }
                    return;
                }
                CollectionVertex[index].MyColor = brushUsually;
            }
            else
            {
                _ = MessageBox.Show("Невозможно выделить вершину. Включите режим удаления вершин/добавления ребер");
                return;
            }

        }

        /// <summary>
        /// Очистка холста
        /// </summary>
        private void Clear()
        {
            graph = new();
            CollectionVertex.Clear();
            CollectionEdge.Clear();

        }

        /// <summary>
        /// Алгоритм Флёри
        /// </summary>
        private void Fluery()
        {
            int[] indexs;
            try
            {
                indexs = graph.Fluery();
            }
            catch (Model.GraphException e)
            {
                _ = MessageBox.Show(e.Message);
                return;
            }
            CollectionEdge.Clear();
            for (int k = 0; k < indexs.Length - 1; k++)
            {
                DPoint from = graph.Vertexes[indexs[k]].Point;
                DPoint to = graph.Vertexes[indexs[k + 1]].Point;
                CollectionEdge.Add(Draw.Edge(from, to, (k + 1).ToString(), graph.IsOriented));
            }
        }


        private delegate int[] SomeHandler(int i1, int i2);

        /// <summary>
        /// Алгоритм поиска в ширину
        /// </summary>
        public void BFS() => DoAlgorithm(graph.BFS);

        /// <summary>
        /// Алгоритм поиска в глубину
        /// </summary>
        public void DFS() => DoAlgorithm(graph.DFS);

        /// <summary>
        /// Алгоритм поиска в ширину по матрице смежности
        /// </summary>
        private void BFSMatrix() => DoAlgorithm(graph.MatrixBFS);

        /// <summary>
        /// Алгоритм поиска в глубину
        /// </summary>
        private void DFSMatrix() => DoAlgorithm(graph.MatrixDFS);

        /// <summary>
        /// Обобщенный метод поиска
        /// </summary>
        /// <param name="a"></param>
        private void DoAlgorithm(SomeHandler a)
        {
            int[] indexs;
            try
            {
                int i1 = GetIndex(FromNameVertex), i2 = GetIndex(ToNameVertex);
                if (i1 == -1 || i2 == -1)
                {
                    _ = MessageBox.Show("Не найдена вершина начала или конца пути.");
                    return;
                }
                indexs = a.Invoke(i1, i2);
                if (indexs == null)
                {
                    _ = MessageBox.Show("Не найден путь или произошла ошибка.");
                    return;
                }
            }
            catch (GraphException e)
            {
                _ = MessageBox.Show(e.Message);
                return;
            }
            CollectionEdge.Clear();
            for (int k = 0; k < indexs.Length - 1; k++)
            {
                DPoint from = graph.Vertexes[indexs[k]].Point;
                DPoint to = graph.Vertexes[indexs[k + 1]].Point;
                CollectionEdge.Add(Draw.Edge(from, to, (k + 1).ToString(), graph.IsOriented));
            }
        }

        /// <summary>
        /// Выводит все пути графа
        /// </summary>
        private void AllWays()
        {
            try
            {
                int i1 = GetIndex(FromNameVertex);
                if (i1 == -1)
                {
                    _ = MessageBox.Show("Неверна введена начальная вершина.");
                    return;
                }
                Result = graph.AllWay(i1);
            }
            catch (GraphException e)
            {
                _ = MessageBox.Show(e.Message);
                return;
            }


        }

        /// <summary>
        /// Выводит матрицу стоимости путей графа
        /// </summary>
        private void CostWays()
        {
            try
            {
                Result = graph.CostWay();
            }
            catch (GraphException e)
            {
                _ = MessageBox.Show(e.Message);
                return;
            }
        }

        /// <summary>
        /// Выводит скобочную структуру графа
        /// </summary>
        private void BraceStruct()
        {
            try
            {
                Result = graph.BraceStruct();
            }
            catch (GraphException e)
            {
                _ = MessageBox.Show(e.Message);
                return;
            }
        }
        
        /// <summary>
        /// Выводит сильно связные компоненты
        /// </summary>
        private void SCC()
        {
            try
            {
                Result = graph.SCC();
            }
            catch (GraphException e)
            {
                _ = MessageBox.Show(e.Message);
                return;
            }
        }
        /// <summary>
        /// Получение индекса вершины
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private int GetIndex(string name)
        {
            foreach (Vertex v in graph.Vertexes)
                if (v.Name == name) return v.Index;
            return -1;
        }
        #endregion

        #region Property
        /// <summary>
        /// Коллекция вершин
        /// </summary>
        public ObservableCollection<VertexUC> CollectionVertex { get; } = new();

        /// <summary>
        /// Коллекция ребер
        /// </summary>
        public ObservableCollection<EdgeUC> CollectionEdge { get; } = new();

        /// <summary>
        /// Граф
        /// </summary>
        private Graph graph = new();

        private bool _Oriented = true;
        /// <summary>
        /// Ориентирован ли граф
        /// </summary>
        public bool Oriented
        {
            get => _Oriented;
            set
            {
                Clear();
                graph.IsOriented = value;
                _ = SetProperty(ref _Oriented, value);
            }
        }

        private string _FromNameVertex = "";
        /// <summary>
        /// Наименование вершины откуда идём
        /// </summary>
        public string FromNameVertex
        {
            get => _FromNameVertex;
            set => SetProperty(ref _FromNameVertex, value);
        }

        private string _ToNameVertex = "";
        /// <summary>
        /// Наименование вершины куда идём
        /// </summary>
        public string ToNameVertex
        {
            get => _ToNameVertex;
            set => SetProperty(ref _ToNameVertex, value);
        }
        /// <summary>
        /// Список индексов выделенных вершин
        /// </summary>
        private static List<int> Indexs { get; } = new();

        private string _Result = "";
        /// <summary>
        /// Поле вывода текстовых ответов
        /// </summary>
        public string Result
        {
            get => _Result;
            set => SetProperty(ref _Result, value);
        }
        #endregion

        #region Property Mode
        /// <summary>
        /// Режим добавления вершин
        /// </summary>
        public bool ModeAddVertex
        {
            get => _ModeAddVertex;
            set => SetProperty(ref _ModeAddVertex, value);
        }
        private bool _ModeAddVertex = true;//поле режима добавления вершин

        /// <summary>
        /// Режим удаления вершин
        /// </summary>
        public bool ModeDelete
        {
            get => _ModeDelete;
            set
            {
                ClearIndex();
                _ = SetProperty(ref _ModeDelete, value);
            }
        }
        private bool _ModeDelete;//поле режима удаления вершин

        /// <summary>
        /// Режим добавления ребер
        /// </summary>
        public bool ModeAddEdge
        {
            get => _ModeAddEdge;
            set
            {
                ClearIndex();
                _ = SetProperty(ref _ModeAddEdge, value);
            }
        }
        private void ClearIndex()
        {
            foreach (int i in Indexs)
                CollectionVertex[i].MyColor = brushUsually;
            Indexs.Clear();
        }
        private bool _ModeAddEdge;//поле режима добавления ребер
        #endregion
    }

}
