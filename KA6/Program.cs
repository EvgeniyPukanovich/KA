using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KA6
{
    class Program
    {
        private static int OverallDistance;
        private static int TankCapacity;
        private static int KilometersPerLiter;
        private static double LitersPerKilometer;
        private static int MaxRange;

        static void Main(string[] args)
        {
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
            List<PetrolStation> graph = GetGraph("in.txt");
            graph = FindPath(graph);
            PrintPath(graph, "out.txt");
        }

        private static List<PetrolStation> GetGraph(string path)
        {
            List<PetrolStation> petrolStations = new List<PetrolStation>();

            using (StreamReader sr = new StreamReader(path))
            {
                OverallDistance = int.Parse(sr.ReadLine());

                string[] words = sr.ReadLine().Split();
                TankCapacity = int.Parse(words[0]);
                KilometersPerLiter = int.Parse(words[1]);
                double firstPrice = double.Parse(words[2]);
                PetrolStation firstStation = new PetrolStation(0, 0, TankCapacity, 0) { Evaluation = firstPrice };
                petrolStations.Add(firstStation);
                int psCount = int.Parse(words[3]);

                for (int i = 0; i < psCount; i++)
                {
                    words = sr.ReadLine().Split();
                    PetrolStation ps = new PetrolStation(int.Parse(words[0]), double.Parse(words[1]), TankCapacity, i + 1);
                    petrolStations.Add(ps);
                }

                petrolStations.Add(new PetrolStation(OverallDistance, 0, TankCapacity, psCount + 1));
            }

            LitersPerKilometer = (double)1 / KilometersPerLiter;
            MaxRange = TankCapacity * KilometersPerLiter;

            for (int i = 0; i < petrolStations.Count - 1; i++)
            {
                for (int j = i + 1; j < petrolStations.Count; j++)
                {
                    if (petrolStations[j].Disatnce - petrolStations[i].Disatnce <= MaxRange)
                        petrolStations[i].FurtherStations.Add(petrolStations[j]);
                    else
                        break;
                }
            }
            return petrolStations;
        }

        //по сути, переделанный алгоритм дейкстры
        private static List<PetrolStation> FindPath(List<PetrolStation> graph)
        {
            List<PetrolStation> notUsed = new List<PetrolStation>(graph);
            PetrolStation curentStation = null;
            
            while (true)
            {
                if (notUsed.Count == graph.Count)
                    curentStation = graph[0];
                else
                    curentStation = GetNextStation(notUsed, curentStation);

                if (curentStation == graph.Last())
                    return graph;

                foreach (var nextStation in curentStation.FurtherStations)
                {
                    int dist = nextStation.Disatnce - curentStation.Disatnce;
                    double fuelOnRoad = dist * LitersPerKilometer;

                    if (dist >= MaxRange / 2)
                    {
                        ChangeEvaluation(curentStation, nextStation, fuelOnRoad);
                    }
                    else
                    {
                        bool needToFuel = EstimateNeedToFuel(nextStation, fuelOnRoad);
                        if (needToFuel)
                            ChangeEvaluation(curentStation, nextStation, fuelOnRoad);
                    }
                }

                notUsed.Remove(curentStation);
            }
        }

        //выбираем заправку с наименьшей оценкой
        private static PetrolStation GetNextStation(List<PetrolStation> notUsed, PetrolStation curentStation)
        {
            double minEval = double.MaxValue;

            foreach (var item in notUsed)
            {
                if (item.Evaluation < minEval)
                {
                    minEval = item.Evaluation;
                    curentStation = item;
                }
            }

            return curentStation;
        }

        private static bool EstimateNeedToFuel(PetrolStation nextStation, double fuelOnRoad)
        {
            bool needToFuel = true;
            //просматривем вторые по дальности заправки от текущей
            foreach (var nextNextSt in nextStation.FurtherStations)
            {
                int nextDist = nextNextSt.Disatnce - nextStation.Disatnce;
                double nextFuelOnRoad = nextDist * LitersPerKilometer;
                //если доедем хоть до какой нибудь, то заправляться нельзя
                if (fuelOnRoad + nextFuelOnRoad <= TankCapacity)
                {
                    needToFuel = false;
                    break;
                }
            }

            return needToFuel;
        }

        private static void ChangeEvaluation(PetrolStation curentStation, PetrolStation nextStation, double fuelOnRoad)
        {
            double newEval = Math.Round(curentStation.Evaluation + fuelOnRoad * nextStation.PricePerLiter + 20, 1);
            if (newEval < nextStation.Evaluation)
            {
                nextStation.Evaluation = newEval;
                nextStation.CameFrom = curentStation;
            }
        }

        private static void PrintPath(List<PetrolStation> graph, string pth)
        {
            List<PetrolStation> path = new List<PetrolStation>();
            PetrolStation current = graph.Last();
            double price = current.Evaluation - 20;

            while (current != null)
            {
                path.Add(current);
                current = current.CameFrom;
            }
            path.Reverse();

            using (StreamWriter sw = new StreamWriter(pth))
            {
                sw.WriteLine(price);

                for (int i = 1; i < path.Count - 1; i++)
                    sw.Write(path[i].Number + " ");
            }
        }
    }



    class PetrolStation
    {
        public readonly int Disatnce;
        public readonly double PricePerLiter;
        public List<PetrolStation> FurtherStations;
        public double Evaluation = 99999;
        public PetrolStation CameFrom;
        public int Number;

        public PetrolStation(int disatnce, double pricePerLitre, double fuelLeft, int number)
        {
            Disatnce = disatnce;
            PricePerLiter = pricePerLitre;
            FurtherStations = new List<PetrolStation>();
            Number = number;
        }
    }
}
