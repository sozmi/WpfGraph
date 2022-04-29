using System.Collections.Generic;
namespace Model
{
    /// <summary>
    /// Класс описывающий вершину графа
    /// </summary>
    public class Vertex
    {
        #region Property
        /// <summary>
        /// Индекс текущей вершины в списке смежности
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Наименование текущей вершины
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Статус вершины.
        /// Посещена - 1, Не посещена - 0, Пройдена - 2
        /// </summary>
        public Status Status { get; set; }

        /// <summary>
        /// Список вершин, смежных с текущей
        /// </summary>
        public List<Vertex> Vertices { get; } = new();


        #endregion

        #region Additional Property

        /// <summary>
        /// Координаты текущей вершины
        /// </summary>
        public DPoint Point { get; set; }

        /// <summary>
        /// Номер компоненты связности
        /// </summary>
        public int NumberComponent { get; set; } = -1;

        /// <summary>
        /// Глубина обхода вершины
        /// </summary>
        public int Depth { get; set; } = -1;


        /// <summary>
        /// Вспомогательное свойство для алгоритмов путей
        /// </summary>
        public Vertex Parent { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Создание вершины
        /// </summary>
        /// <param name="id">Идентификатор вершины</param>
        /// <param name="p">Координаты вершины</param>
        public Vertex(int index, int id, DPoint p)
        {
            Index = index;
            Name = GetName(ref id, false);
            Point = p;
        }

        /// <summary>
        /// Создание вершины
        /// </summary>
        /// <param name="ID">Идентификатор вершины</param>
        /// <param name="point">Координаты вершины</param>
        /// <param name="status">Статус вершины</param>
        public Vertex(int ID, int index, DPoint point, Status status)
        {
            Index = index;
            Name = GetName(ref ID, false);
            Point = point;
            Status = status;
        }

        /// <summary>
        /// Создание вершины
        /// </summary>
        /// <param name="name">Наименование вершины</param>
        /// <param name="point">Координаты вершины</param>
        /// <param name="status">Статус вершины</param>
        /// <param name="index">Индекс вершины в списке смежности</param>
        public Vertex(int index, string name, DPoint point, Status status)
        {
            Name = name;
            Point = point;
            Status = status;
            Index = index;
        }

        #endregion

        #region Methods
        /// <summary>
        /// Назначение буквенного обозначения вершины по индексу
        /// </summary>
        /// <param name="index">индекс вершины</param>
        /// <returns></returns>
        private static string GetName(ref int num, bool rec)
        {
            string res = "";
            int count = 0;
            while (num >= 26)
            {
                num -= 26;
                count++;
            }
            if (count > 25)
                res += GetName(ref count, true);
            if (count != 0)
                res += (char)('A' + count - 1);
            if (!rec)
                res += (char)('A' + num);
            return res;
        }

        /// <summary>
        /// Создание клона вершины
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Vertex Clone(int index) => new(index, Name, Point, Status);
        #endregion
    }
}
