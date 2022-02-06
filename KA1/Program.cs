using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace KA1
{
    class Program
    {
        static string inPath = @"in.txt";
        static string outPath = @"out.txt";

        //индексы соответсвуют номерам вершин
        //считается, что первой идет вершина под номером 1
        static int[][] graph;
        static int[] colours;

        static void Main(string[] args)
        {
            bool? isBipartite = ReadGraph(inPath);
            if (isBipartite == false)
                PrintResult(false, outPath);
            else
                PrintResult(IsBipartite(), outPath);
        }

        //false, если смогли уже определить, что не двудольный
        //null, если пока нельязя сказать
        private static bool? ReadGraph(string path)
        {
            using (StreamReader sr = new StreamReader(path, Encoding.Default))
            {
                int numberOfNodes = Convert.ToInt32(sr.ReadLine());

                graph = new int[numberOfNodes + 1][];
                colours = new int[numberOfNodes + 1];

                graph[0] = null;
                for (int i = 1; i < numberOfNodes + 1; i++)
                {
                    string[] words = sr.ReadLine().Split();
                    graph[i] = new int[words.Length - 1];

                    for (int j = 0; j < words.Length - 1; j++)
                    {
                        int incNode = Convert.ToInt32(words[j]);
                        //есть петля
                        if (i == incNode)
                            return false;
                        graph[i][j] = incNode;
                    }
                }
            }
            return null;
        }

        private static void PrintResult(bool isBipartite, string writePath)
        {
            using (StreamWriter sw = new StreamWriter(writePath, false, Encoding.Default))
            {
                if (isBipartite)
                {
                    sw.WriteLine("Y");
                    int firstPart = colours[1];
                    for (int i = 1; i < colours.Length; i++)
                    {
                        if (colours[i] == firstPart)
                            sw.Write($"{i} ");
                    }
                    sw.WriteLine(0);
                    int secondPart = firstPart == 1 ? 2 : 1;
                    for (int i = 1; i < colours.Length; i++)
                    {
                        if (colours[i] == secondPart)
                            sw.Write($"{i} ");
                    }
                    sw.WriteLine(0);
                }
                else
                    sw.WriteLine("N");
            }
        }

        private static bool IsBipartite()
        {
            Queue<int> queue = new Queue<int>();

            queue.Enqueue(1);
            colours[1] = 1;

            while (queue.Count != 0)
            {
                int node = queue.Dequeue();
                //0-не посещена, 1,2 - раскрашена в один из цветов
                int otherColour = colours[node] == 1 ? 2 : 1;

                for (int i = 0; i < graph[node].Length; i++)
                {
                    int incNode = graph[node][i];
                    if (colours[incNode] == 0)
                    {
                        colours[incNode] = otherColour;
                        queue.Enqueue(incNode);
                    }
                    else if (colours[incNode] == colours[node])
                        return false;
                }
            }
            return true;
        }
    }
}