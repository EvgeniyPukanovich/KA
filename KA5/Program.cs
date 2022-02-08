using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KA5
{
    class Program
    {
        private static List<Node> partX = new List<Node>();
        private static List<Node> partY = new List<Node>();

        public static void Main(string[] args)
        {
            ReadGraph("in.txt");
            FindMatching();
            PrintAnswer("out.txt");
        }

        private static void ReadGraph(string path)
        {
            using (StreamReader sr = new StreamReader(path, Encoding.Default))
            {
                int[] words = sr.ReadLine().Split().Select(x => Convert.ToInt32(x)).ToArray();

                for (int i = 1; i <= words[1]; i++)
                {
                    partY.Add(new Node(i, 1));
                }

                for (int i = 0; i < words[0]; i++)
                {
                    Node xNode = new Node(i + 1, 0);
                    partX.Add(xNode);
                    string raw = sr.ReadLine();
                    if (raw == null)
                        continue;
                    int[] adjacencyArray = raw.Split().Select(x => Convert.ToInt32(x)).ToArray();
                    for (int j = 0; j < adjacencyArray.Length - 1; j++)
                    {
                        Node yNode = partY[adjacencyArray[j] - 1];
                        xNode.Connect(yNode);
                    }
                }
            }
        }

        private static HashSet<Edge> matching = new HashSet<Edge>();
        private static HashSet<Node> darkNodes = new HashSet<Node>();

        private static void FindMatching()
        {
            bool isChainFound = false;
            //собственно алгоритм из лекций
            do
            {
                isChainFound = false;
                foreach (var xNode in partX)
                {
                    if (!darkNodes.Contains(xNode))
                    {
                        List<Edge> chain = FindChain(xNode, false);
                        if (chain != null)
                        {
                            isChainFound = true;
                            matching.SymmetricExceptWith(chain);
                            darkNodes.Clear();
                            foreach (var edge in matching)
                            {
                                darkNodes.Add(edge.From);
                                darkNodes.Add(edge.To);
                            }
                            break;

                        }
                    }
                }
            } while (isChainFound);
        }

        private static Stack<Edge> callStack = new Stack<Edge>();
        private static HashSet<Node> visited = new HashSet<Node>();

        //Поиск М-чередующейся цепи(DFS)
        private static List<Edge> FindChain(Node start, bool findDark)
        {
            visited.Add(start);
            if (!darkNodes.Contains(start) && findDark)
            {
                visited.Clear();
                return callStack.ToList();
            }

            for (int i = 0; i < start.incidentNodes.Count; i++)
            {
                //смотрим, является ли IncidentEdges[i] светлым или темным
                if (matching.Contains(start.IncidentEdges[i]) == findDark && !visited.Contains(start.IncidentEdges[i].To))
                {
                    callStack.Push(start.IncidentEdges[i]);
                    //ищем ребро противоположного цвета
                    var ret = FindChain(start.IncidentEdges[i].To, !findDark);
                    if (callStack.Count != 0)
                        callStack.Pop();
                    if (ret != null)
                        return ret;
                }
            }

            visited.Clear();
            return null;
        }

        private static void PrintAnswer(string writePath)
        {
            var ls = matching.Select(x =>
            {
                if (x.From.Part == 0)
                    return (x.From, x.To);
                else
                    return (x.To, x.From);
            }).ToList();

            ls.Sort((x, y) =>
            {
                if (x.Item1.NodeNumber < y.Item1.NodeNumber)
                    return -1;
                else
                    return 1;
            });

            int[] resArray = new int[partX.Count];

            foreach (var edge in ls)
                resArray[edge.Item1.NodeNumber - 1] = edge.Item2.NodeNumber;

            using (StreamWriter sw = new StreamWriter(writePath, false, Encoding.Default))
            {
                foreach (var node in resArray)
                {
                    sw.Write((node) + " ");
                    Console.Write((node) + " ");
                }
            }
        }
    }

    public class Node
    {
        public readonly List<Node> incidentNodes = new List<Node>();
        public readonly int Part;
        public readonly int NodeNumber;
        public readonly List<Edge> IncidentEdges = new List<Edge>();

        public Node(int number, int part)
        {
            NodeNumber = number;
            Part = part;
        }

        public void Connect(Node node)
        {
            incidentNodes.Add(node);
            node.incidentNodes.Add(this);
            IncidentEdges.Add(new Edge(this, node));
            node.IncidentEdges.Add(new Edge(node, this));
        }
    }

    public class Edge
    {
        public readonly Node From;
        public readonly Node To;

        public Edge(Node first, Node second)
        {
            From = first;
            To = second;
        }

        public bool IsIncident(Node node)
        {
            return From == node || To == node;
        }

        public Node OtherNode(Node node)
        {
            if (!IsIncident(node)) throw new ArgumentException();
            if (From == node) return To;
            return From;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Edge) || obj == null)
                return false;
            Edge other = obj as Edge;
            if ((From == other.From && To == other.To) || (From == other.To && To == other.From))
                return true;
            return false;
        }

        public override int GetHashCode()
        {
            return From.GetHashCode() + To.GetHashCode();
        }
    }
}
