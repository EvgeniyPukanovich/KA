using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KA_2
{
    class Program
    {
        static int row = 0;
        static int col = 0;
        static string outPath = @"out.txt";
        static string inPath = @"in.txt";

        static Stack<Point> callStack = new Stack<Point>();
        static bool flag = false;

        static void Main(string[] args)
        {
            int[,] map;
            Point source;
            Point target;
            (map, source, target) = ReadMap();

            callStack.Push(source);
            DFS(map, source, target);
            if (!flag)
                PrintResult(outPath, false);

        }

        private static (int[,] map, Point source, Point target) ReadMap()
        {
            int[,] map;
            Point source;
            Point target;

            using (StreamReader sr = new StreamReader(inPath, Encoding.Default))
            {
                row = Convert.ToInt32(sr.ReadLine());
                col = Convert.ToInt32(sr.ReadLine());

                map = new int[row + 1, col + 1];
                string[] data;
                for (int i = 1; i < row + 1; i++)
                {
                    data = sr.ReadLine().Split();
                    for (int j = 1; j < col + 1; j++)
                    {
                        map[i, j] = Convert.ToInt32(data[j - 1]);
                    }
                }

                data = sr.ReadLine().Split();
                source = new Point
                {
                    X = Convert.ToInt32(data[0]),
                    Y = Convert.ToInt32(data[1])
                };

                data = sr.ReadLine().Split();
                target = new Point
                {
                    X = Convert.ToInt32(data[0]),
                    Y = Convert.ToInt32(data[1])
                };
            }

            return (map, source, target);
        }

        static private void DFS(int[,] map, Point point, Point target)
        {
            if (point.X < 1 || point.X > row || point.Y < 1 || point.Y > col)
                return;
            if (map[point.X, point.Y] != 0)
                return;

            if (point == target)
            {
                PrintResult(outPath, true);
                flag = true;
                return;
            }

            map[point.X, point.Y] = 2;

            for (var dy = -1; dy <= 1; dy++)
            {
                for (var dx = -1; dx <= 1; dx++)
                {
                    if (dx != 0 && dy != 0)
                        continue;
                    Point newPoint = new Point() { X = point.X + dx, Y = point.Y + dy };
                    callStack.Push(newPoint);
                    DFS(map, newPoint, target);
                    if (callStack.Count != 0)
                        callStack.Pop();
                }
            }
        }

        static private void PrintResult(string writePath, bool isFound)
        {
            using (StreamWriter sw = new StreamWriter(writePath, false, Encoding.Default))
            {
                if (isFound)
                {
                    sw.WriteLine("Y");
                    List<Point> lst = new List<Point>(callStack);
                    lst.Reverse();
                    foreach (var item in lst)
                    {
                        sw.WriteLine(item.X + " " + item.Y);
                    }
                }
                else
                    sw.WriteLine("N");
            }
        }
    }

    class Point
    {
        public int X { get; set; }
        public int Y { get; set; }

        // override object.Equals
        public override bool Equals(Object obj)
        {
            //Check for null and compare run-time types.
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else
            {
                Point p = (Point)obj;
                return (X == p.X) && (Y == p.Y);
            }
        }

        public override int GetHashCode()
        {
            return (X << 2) ^ Y;
        }

        public static bool operator ==(Point p1, Point p2)
        {
            return p1.X == p2.X && p1.Y == p2.Y;
        }

        public static bool operator !=(Point p1, Point p2)
        {
            return !(p1 == p2);
        }
    }
}
