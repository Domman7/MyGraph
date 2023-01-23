using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml.Linq;

namespace GraphTheory
{
    class Program
    {
        private const string CreateGraph = "CREATE_GRAPH";
        private const string Select = "SELECT";
        private const string AddNode = "ADD_NODE";
        private const string AddEdge = "ADD_EDGE";
        private const string DeleteNode = "DELETE_NODE";
        private const string DeleteEdge = "DELETE_EDGE";

        private const string ShowHangingNodes = "SHOW_HANG";
        private const string ShowCircled = "SHOW_CIRCLED";
        private const string HangingDelete = "HANGING_DELETE";
        private const string StronglyConnected = "STRONGLY_CONNECTED";
        private const string ShortestWays = "SHORTEST_WAYS";
        private const string Frame = "FRAME";
        private const string NSet = "NSET";
        private const string Eccentricity = "ECCENTRICITY";
        private const string Ford = "FORD";
        private const string Flow = "FLOW";

        private const string Show = "SHOW";
        private const string WriteToFile = "WRITE_TO_FILE";

        private const string Hint = "HINT";
        private const string Clear = "CLEAR";
        private const string Exit = "EXIT";

        private const string UnknownCommand = "UNKNOWN COMMAND";
        private const string WrongArgument = "Wrong argument(s)";

        private static void SelectGraph(ref List<Tuple<string, Graph>> lst)//метод для вывода нумерованого списка
        {
            int index = 1;
            foreach (var item in lst)
            {
                Console.WriteLine(index + ". " + item.Item1);
                index++;
            }
        }

        private static bool TypeSelector(out bool oriented)//Метод для выбора типа графа
        {
            Console.WriteLine("Select the type: \n1. Ordinary\n2. Oriented");
            int type = int.Parse(Console.ReadLine());
            oriented = true;
            if (type == 1)
                oriented = false;
            else
                if (type == 2)
                oriented = true;
            else
            {
                Console.WriteLine(WrongArgument);
                return false;
            }
            return true;
        }

        private static String GetHint()//вывод подсказки
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(CreateGraph).Append('\n');
            sb.Append(Select).Append('\n');
            sb.Append(AddNode).Append('\n');
            sb.Append(AddEdge).Append('\n');
            sb.Append(DeleteNode).Append('\n');
            sb.Append(DeleteEdge).Append('\n');
            sb.Append(Show).Append('\n');
            sb.Append(WriteToFile).Append("\n\n");
            sb.Append(ShowHangingNodes).Append('\n');
            sb.Append(ShowCircled).Append("\n");
            sb.Append(HangingDelete).Append("\n");
            sb.Append(StronglyConnected).Append("\n");
            sb.Append(ShortestWays).Append("\n");
            sb.Append(Frame).Append("\n");
            sb.Append(NSet).Append("\n");
            sb.Append(Eccentricity).Append("\n");
            sb.Append(Ford).Append("\n");
            sb.Append(Flow).Append("\n\n");
            sb.Append(Hint).Append('\n');
            sb.Append(Clear).Append('\n');
            sb.Append(Exit).Append('\n');

            return sb.ToString();
        }
        public static void Start()//вывод консольного интерфейса
        {
            Graph cur = new Graph(true, true);//ссылка на текущий граф и его свойства
            bool curOriented = true;
            int curID = 0;
            Console.WriteLine(GetHint());
            List<Tuple<string, Graph>> data = new List<Tuple<string, Graph>>();//списки для хранени созданных графов
            List<Tuple<string, Graph>> dataOrdinary = new List<Tuple<string, Graph>>();
            data.Add(new Tuple<string, Graph>("initial", cur));

            data.Add(new Tuple<string, Graph>("OrientedExample", new Graph(@"C:\Users\Egor\source\repos\MyGraph\MyGraph\Examples\OrientedInput.txt")));
            dataOrdinary.Add(new Tuple<string, Graph>("OrdinaryExample", new Graph(@"C:\Users\Egor\source\repos\MyGraph\MyGraph\Examples\OrdinaryInput.txt")));
            data.Add(new Tuple<string, Graph>("Flow", new Graph(@"C:\Users\Egor\source\repos\MyGraph\MyGraph\Examples\Flow.txt")));
            dataOrdinary.Add(new Tuple<string, Graph>("Frame", new Graph(@"C:\Users\Egor\source\repos\MyGraph\MyGraph\Examples\Frame.txt")));
            data.Add(new Tuple<string, Graph>("NegativeCycle", new Graph(@"C:\Users\Egor\source\repos\MyGraph\MyGraph\Examples\NegativeCycle.txt")));
            data.Add(new Tuple<string, Graph>("Flow2", new Graph(@"C:\Users\Egor\source\repos\MyGraph\MyGraph\Examples\Flow2.txt")));

            for (; ; )
            {
                try
                {
                    Console.Write(">>> ");
                    List<String> arguments = new List<String>(Console.ReadLine().Split(" "));//считывание команды
                    string command = arguments[0].ToUpper();//и соответсвующая реакция на нее
                    arguments.RemoveAt(0);
                    switch (command)
                    {
                        case CreateGraph:
                            {
                                Console.WriteLine("Enter name: ");
                                string name = Console.ReadLine();
                                bool oriented;
                                if (TypeSelector(out oriented))
                                {
                                    Console.WriteLine("Select the way:\n1. COPY\n2. NEW\n3. FILE");
                                    int way = int.Parse(Console.ReadLine());
                                    switch (way)
                                    {
                                        case 1:
                                            {
                                                Console.WriteLine("Select initial graph: ");
                                                int length = 0;
                                                if (oriented)
                                                {
                                                    SelectGraph(ref data);
                                                    length = data.Count - 1;
                                                }
                                                else
                                                {
                                                    SelectGraph(ref dataOrdinary);
                                                    length = dataOrdinary.Count - 1;
                                                }
                                                int id = int.Parse(Console.ReadLine()) - 1;
                                                if (id < 0 || id > length)
                                                    Console.WriteLine(WrongArgument);
                                                else
                                                {
                                                    if (oriented)
                                                    {
                                                        Tuple<string, Graph> temp = new Tuple<string, Graph>(name, new Graph(data[id].Item2));
                                                        data.Add(temp);
                                                        Console.WriteLine("Graph created");
                                                    }
                                                    else
                                                    {
                                                        Tuple<string, Graph> temp = new Tuple<string, Graph>(name, new Graph(dataOrdinary[id].Item2));
                                                        dataOrdinary.Add(temp);
                                                        Console.WriteLine("Graph created");
                                                    }
                                                }
                                            }
                                            break;
                                        case 2:
                                            {
                                                Console.WriteLine("Select the type: \n1. Weighted\n2. Not Weighted");
                                                int type = int.Parse(Console.ReadLine());
                                                bool weighted = true;
                                                if (type == 1)
                                                    weighted = true;
                                                else
                                                    if (type == 2)
                                                    weighted = false;
                                                else
                                                {
                                                    Console.WriteLine(WrongArgument);
                                                    break;
                                                }
                                                if (oriented)
                                                {
                                                    Tuple<string, Graph> temp = new Tuple<string, Graph>(name, new Graph(weighted, oriented));
                                                    data.Add(temp);
                                                    Console.WriteLine("Graph created");
                                                }
                                                else
                                                {
                                                    Tuple<string, Graph> temp = new Tuple<string, Graph>(name, new Graph(weighted, oriented));
                                                    dataOrdinary.Add(temp);
                                                    Console.WriteLine("Graph created");
                                                }
                                            }
                                            break;
                                        case 3:
                                            {
                                                Console.WriteLine("Enter the way: ");
                                                string destination = Console.ReadLine();
                                                if (oriented)
                                                {
                                                    Tuple<string, Graph> temp = new Tuple<string, Graph>(name, new Graph(destination, oriented));
                                                    data.Add(temp);
                                                    Console.WriteLine("Graph created");
                                                }
                                                else
                                                {
                                                    Tuple<string, Graph> temp = new Tuple<string, Graph>(name, new Graph(destination, oriented));
                                                    dataOrdinary.Add(temp);
                                                    Console.WriteLine("Graph created");
                                                }
                                            }
                                            break;
                                        default:
                                            {
                                                Console.WriteLine(WrongArgument);
                                            }
                                            break;
                                    }
                                }
                            }
                            break;
                        case Select:
                            {
                                int length = 0;
                                if (TypeSelector(out curOriented))
                                {
                                    if (curOriented)
                                    {
                                        SelectGraph(ref data);
                                        length = data.Count;
                                    }
                                    else
                                    {
                                        SelectGraph(ref dataOrdinary);
                                        length = dataOrdinary.Count;
                                    }
                                    if (length == 0)
                                    {
                                        Console.WriteLine("List is empty");
                                    }
                                    else
                                    {
                                        int id = int.Parse(Console.ReadLine()) - 1;
                                        if (id < 0 || id > length - 1)
                                            Console.WriteLine(WrongArgument);
                                        else
                                        {
                                            if (curOriented)
                                            {
                                                cur = data[id].Item2;
                                            }
                                            else
                                            {
                                                cur = dataOrdinary[id].Item2;
                                            }
                                            curID = id;
                                        }
                                    }
                                }
                            }
                            break;
                        case Show:
                            {
                                cur.Show();
                            }
                            break;
                        case WriteToFile:
                            {
                                {
                                    Console.WriteLine("Enter the way: ");
                                    string way = Console.ReadLine();
                                    cur.Show(way);
                                }
                            }
                            break;
                        case AddNode:
                            {
                                Console.WriteLine("Enter name of node:");
                                string name = Console.ReadLine();
                                if (cur.AddNode(name))
                                    Console.WriteLine("Node added");
                            }
                            break;
                        case DeleteNode:
                            {
                                cur.Show();
                                Console.WriteLine("Select node: ");
                                if (cur.DeleteNode(Console.ReadLine()))
                                    Console.WriteLine("Node deleted");
                            }
                            break;
                        case AddEdge:
                            {
                                cur.Show();
                                Console.WriteLine("Select nodes: ");
                                if (cur.Weighted)
                                {
                                    string[] line = Console.ReadLine().Split(' ');
                                    if (line.Length != 3)
                                        Console.WriteLine("Wrong argument(s)");
                                    else
                                    {
                                        if (cur.AddEdge(line[0], line[1], double.Parse(line[2])))
                                            Console.WriteLine("Edge added");
                                    }
                                }
                                else
                                {
                                    string[] line = Console.ReadLine().Split(' ');
                                    if (line.Length != 2)
                                        Console.WriteLine("Wrong argument(s)");
                                    else
                                    {
                                        if (cur.AddEdge(line[0], line[1], 0))
                                            Console.WriteLine("Edge added");
                                    }
                                }
                            }
                            break;
                        case DeleteEdge:
                            {
                                cur.Show();
                                Console.WriteLine("Select nodes: ");
                                string[] line = Console.ReadLine().Split(' ');
                                if (line.Length != 2)
                                    Console.WriteLine("Wrong argument(s)");
                                else
                                {
                                    if (cur.DeleteEdge(line[0], line[1]))
                                        Console.WriteLine("Edge deleted");
                                }
                            }
                            break;
                        case ShowHangingNodes:
                            {
                                Console.Write("Hanging nodes: ");
                                foreach (var item in cur.ShowHangingNodes())
                                {         
                                        Console.Write(item + " ");
                                }
                                Console.WriteLine();
                            }
                            break;
                        case ShowCircled:
                            {
                                Console.WriteLine("Select node: ");
                                foreach (var item in cur.ShowCircled(Console.ReadLine()))
                                        Console.Write(item + " ");
                                Console.WriteLine();
                            }
                            break;
                        case HangingDelete:
                            {
                                if (!curOriented)
                                {
                                    Console.WriteLine("Enter name: ");
                                    string name = Console.ReadLine();
                                    Graph result = new Graph(dataOrdinary[curID].Item2);
                                    result.DeleteHanging(cur);
                                    Tuple<string, Graph> temp = new Tuple<string, Graph>(name, result);
                                    dataOrdinary.Add(temp);
                                    Console.WriteLine("Graph created");
                                }
                                else
                                    Console.WriteLine("You need to choose an ordinary graph");
                            }
                            break;
                        case StronglyConnected:
                            {
                                if (cur.IsStronglyConnected())
                                    Console.WriteLine("Graph is strongly connected");
                                else
                                    Console.WriteLine("Graph is not strongly connected");
                            }
                            break;
                        case ShortestWays:
                            {
                                Console.WriteLine("Select node");
                                foreach (var item1 in cur.ShortestWays(Console.ReadLine()))
                                {
                                    Console.Write(item1.Key + ": ");
                                    if (item1.Value != null)
                                        while (item1.Value.Count != 0)
                                        {
                                            Console.Write(item1.Value.Pop() + " ");
                                        }
                                    Console.WriteLine();
                                }
                            }
                            break;
                        case Frame:
                            {
                                if (cur.IsConnected())
                                {
                                    Console.WriteLine("Select node");
                                    cur.Frame(Console.ReadLine()).Show();
                                }
                                else
                                {
                                    Console.WriteLine("Graph is not connected, it is possible to build only a forest");
                                    Console.WriteLine("Select node");
                                    cur.Frame(Console.ReadLine()).Show();
                                }
                            }
                            break;
                        case NSet:
                            {
                                Console.WriteLine("Enter n:");
                                double n = double.Parse(Console.ReadLine());
                                Console.WriteLine("Select node");
                                string node = Console.ReadLine();

                                Console.WriteLine("NSet: ");
                                foreach (var item in cur.NSet(node, n))
                                {
                                    Console.Write(item + " ");
                                }
                                Console.WriteLine();
                            }
                            break;
                        case Eccentricity:
                            {
                                Console.WriteLine("Eccentricity set: ");
                                foreach (var item in cur.Eccentricity())
                                    Console.Write(item + " ");
                                Console.WriteLine();
                            }
                            break;
                        case Ford:
                            {
                                Console.WriteLine("Select node");
                                foreach (var item in cur.BellmanFordUpgraded(Console.ReadLine()))
                                {
                                    if (item.Value == double.MaxValue)
                                        Console.WriteLine(item.Key + ": inf");
                                    else
                                        Console.WriteLine(item.Key + ": " + item.Value);
                                }
                            }
                            break;
                        case Flow:
                            {
                                Console.WriteLine("Enter out node:");
                                string outNode = Console.ReadLine();
                                Console.WriteLine("Enter in node:");
                                string inNode = Console.ReadLine();

                                Console.WriteLine("Max flow: " + cur.FordFalkerson(outNode, inNode));

                            }
                            break;
                        case Hint:
                            Console.WriteLine(GetHint());
                            break;
                        case Clear:
                            Console.Clear();
                            break;
                        case Exit:
                            return;
                        default:
                            {
                                Console.WriteLine(UnknownCommand);
                            }
                            break;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
        static void Main(string[] args)
        {
            Start();
        }
    }
}