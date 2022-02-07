using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace KA4_framework
{
    class Program
    {
        static void Main(string[] args)
        {
            int[,] graph = ReadGraph("in.txt");
            (int fullWeight, List<List<int>> lists) = FindMST(graph);
            PrintAnswer("out.txt", lists, fullWeight);
        }

        static int[,] ReadGraph(string path)
        {
            using (StreamReader sr = new StreamReader(path, Encoding.Default))
            {
                int numberOfNodes = Convert.ToInt32(sr.ReadLine());
                int[,] graph = new int[numberOfNodes, numberOfNodes];
                for (int i = 0; i < numberOfNodes; i++)
                {
                    string[] words = sr.ReadLine().Split();
                    for (int j = 0; j < numberOfNodes; j++)
                        graph[i, j] = Convert.ToInt32(words[j]);
                }
                return graph;
            }
        }

        static void PrintAnswer(string path, List<List<int>> adjacencyList, int fullWeight)
        {
            using (StreamWriter sw = new StreamWriter(path, false, Encoding.Default))
            {
                foreach (var list in adjacencyList)
                {
                    list.Sort();
                    foreach (var node in list)
                    {
                        //в исходном графе вершины нумеруются с 1
                        //Console.Write(node + 1 + " ");
                        sw.Write(node + 1 + " ");
                    }
                    //Console.WriteLine(0);
                    sw.WriteLine("0");
                }
                //Console.WriteLine(fullWeight);
                sw.WriteLine(fullWeight);
            }
        }

        static (int weight, List<List<int>> adjLists) FindMST(int[,] graph)
        {
            //уже включенные в дерево вершины
            bool[] used = new bool[graph.GetLength(0)];
            //"расстояние" от строящегося дерева до остальных вершин
            int[] dist = new int[graph.GetLength(0)];
            //prev[i] - та вершина, из которой мы дотянулись до i
            int[] prev = new int[graph.GetLength(0)];

            dist[0] = 0;
            prev[0] = -1;

            for (int i = 1; i < dist.Length; i++)
                dist[i] = int.MaxValue;

            for (int i = 0; i < graph.GetLength(0); i++)
            {
                int currentNode = ExtractMin(used, dist);
                used[currentNode] = true;

                for (int j = 0; j < graph.GetLength(0); j++)
                {
                    if (graph[currentNode, j] != 32767 && !used[j] && graph[currentNode, j] < dist[j])
                    {
                        prev[j] = currentNode;
                        dist[j] = graph[currentNode, j];
                    }

                }
            }
            //остовное дерево, заданное спиками смежности
            return GetAdjacencyLists(graph, dist, prev);
        }

        static (int weight, List<List<int>> adjLists) GetAdjacencyLists(int[,] graph, int[] dist, int[] prev)
        {
            List<List<int>> adjacencyLists = new List<List<int>>();
            int fullWeight = 0;

            for (int i = 0; i < graph.GetLength(0); i++)
                adjacencyLists.Add(new List<int>());

            for (int i = 0; i < graph.GetLength(0); i++)
            {
                if (prev[i] == -1)
                    continue;
                adjacencyLists[i].Add(prev[i]);
                adjacencyLists[prev[i]].Add(i);

                fullWeight += dist[i];
            }

            return (fullWeight, adjacencyLists);
        }

        static int ExtractMin(bool[] used, int[] dist)
        {
            int minVal = int.MaxValue;
            int minNode = -1;

            for (int i = 0; i < used.Length; i++)
            {
                if (!used[i] && dist[i] < minVal)
                {
                    minVal = dist[i];
                    minNode = i;
                }
            }

            return minNode;
        }
    }
}
