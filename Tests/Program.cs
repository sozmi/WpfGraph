using Model;
using System;
using System.Collections.Generic;

//namespace Test
//{
//    class Program
//    {
//        static void Main(string[] args)
//        {
//            Graph g = new();
//            g.AddVertex(new(0, 0));
//            g.AddVertex(new(0, 0));
//            g.AddVertex(new(0, 0));
//            g.AddVertex(new(0, 0));
//            g.AddVertex(new(0, 0));

//            g.AddEdge(0, 1);
//            g.AddEdge(1, 4);
//            g.AddEdge(4, 0);
//            g.AddEdge(1, 2);
//            g.AddEdge(2, 3);
//            g.AddEdge(3, 2);

//            //g.AddVertex(new(0, 0));
//            //g.AddVertex(new(0, 0));
//            //g.AddVertex(new(0, 0));
//            //g.AddVertex(new(0, 0));
//            //g.AddVertex(new(0, 0));
//            //g.AddVertex(new(0, 0));
//            //g.AddVertex(new(0, 0));
//            //g.AddVertex(new(0, 0));

//            //g.AddEdge(0, 1);
//            //g.AddEdge(1, 2);
//            //g.AddEdge(2, 0);
//            //g.AddEdge(3, 1);
//            //g.AddEdge(3, 2);
//            //g.AddEdge(3, 4);
//            //g.AddEdge(4, 3);
//            //g.AddEdge(4, 5);
//            //g.AddEdge(5, 2);
//            //g.AddEdge(5, 6);
//            //g.AddEdge(6, 5);
//            //g.AddEdge(7 , 4);
//            //g.AddEdge(7, 6);
//            //g.AddEdge(7, 7);
//            Console.WriteLine(g.Output(null));
//            Console.WriteLine(g.SCC());
//        }
//    }
//}
namespace Test
{


    class Program
    {
        private static int cnt = 0; //счётчик времени обхода вершины
        private static int scnt = 0; //счётчик компонент

        private static Stack<Vertex> additional_stack; //главный стек; путь обхода вершин
        private static Stack<Vertex> main_stack; //дополнительный стек для алгоритма Габова

        static public void DFS_mod(Vertex v)
        {
            v.Depth = cnt++; //время когда выполнен обход вершины
            main_stack.Push(v); //добавляем вершину в путь обхода
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
                (v2 = main_stack.Pop()).NumberComponent = scnt; //достаём вершину из главного стека и присваиваем ей номер компоненты, которой она принадлежит
            } while (v2 != v); //и делаем это до тех пор, пока (вершина которая была на верхушке дополнительного стека) НЕ РАВНА (рассматриваемой вершине)
            scnt++; //номер следующей компоненты
        }
        static void Main(string[] args)
        {
            Graph g = new();
            g.AddVertex(new(0, 0));
            g.AddVertex(new(0, 0));
            g.AddVertex(new(0, 0));
            g.AddVertex(new(0, 0));
            g.AddVertex(new(0, 0));

            g.AddEdge(0, 1);
            g.AddEdge(1, 4);
            g.AddEdge(4, 0);
            g.AddEdge(2, 1);
            g.AddEdge(2, 3);
            g.AddEdge(3, 2);


            Console.WriteLine(StronglyCC(g.Vertexes));
        }
        private static string StronglyCC(List<Vertex> vertices)
        {
            additional_stack = new(); main_stack = new(); //инициализация стеков

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
    }
}
