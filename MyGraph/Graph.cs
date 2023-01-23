using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace GraphTheory
{
    public class Graph
    {
        protected Dictionary<string, Dictionary<string, double>> adjList;//Список смежности
        protected Dictionary<string, string> nov = new Dictionary<string, string>();//Список предшественников
        protected bool weighted;//Свойства графа
        protected bool oriented;

        public Graph(bool weighted, bool oriented)//Конструктор для создания нового графа
        {
            this.weighted = weighted;
            this.oriented = oriented;
            adjList = new Dictionary<string, Dictionary<string, double>>();
            nov = new Dictionary<string, string>();
        }

        public Graph(string fileName)//Конструктор для чтения графа из файла
        {
            string[] replace = fileName.Split("\\");
            String.Join("/", replace);
            adjList = new Dictionary<string, Dictionary<string, double>>();

            using (StreamReader file = new StreamReader(fileName))
            {
                if (file.Peek() != -1)//Считываем параметры графа
                    weighted = bool.Parse(file.ReadLine());
                if (file.Peek() != -1)
                    oriented = bool.Parse(file.ReadLine());
                while (file.Peek() != -1)
                {
                    string line = file.ReadLine();
                    string[] mas = line.Split('|', StringSplitOptions.RemoveEmptyEntries);
                    Dictionary<string, double> temp = new Dictionary<string, double>();
                    if (weighted)
                    {
                        for (int i = 1; i < mas.Length; i += 2)
                        {
                            temp.Add(mas[i], double.Parse(mas[i + 1]));
                        }
                        adjList.Add(mas[0], temp);
                    }
                    else
                    {
                        for (int i = 1; i < mas.Length; i++)
                        {
                            temp.Add(mas[i], 0);
                        }
                        adjList.Add(mas[0], temp);
                    }
                }
            }
            nov = new Dictionary<string, string>();
        }

        public Graph(string fileName, bool oriented)//Вспомогательный конструктор для чтения из файла
        {
            string[] replace = fileName.Split("\\");
            String.Join("/", replace);
            adjList = new Dictionary<string, Dictionary<string, double>>();

            using (StreamReader file = new StreamReader(fileName))
            {
                if (file.Peek() != -1)
                {
                    this.oriented = oriented;
                    weighted = bool.Parse(file.ReadLine());
                }
                while (file.Peek() != -1)
                {
                    string line = file.ReadLine();
                    string[] mas = line.Split('|', StringSplitOptions.RemoveEmptyEntries);
                    Dictionary<string, double> temp = new Dictionary<string, double>();
                    if (weighted)
                    {
                        for (int i = 1; i < mas.Length; i += 2)
                        {
                            temp.Add(mas[i], double.Parse(mas[i + 1]));
                        }
                        adjList.Add(mas[0], temp);
                    }
                    else
                    {
                        for (int i = 1; i < mas.Length; i++)
                        {
                            temp.Add(mas[i], 0);
                        }
                        adjList.Add(mas[0], temp);
                    }
                }
            }
            nov = new Dictionary<string, string>();
        }

        public Graph(Graph init)//Конструктор копия
        {
            weighted = init.weighted;
            oriented = init.oriented;
            Dictionary<string, Dictionary<string, double>> temp = new Dictionary<string, Dictionary<string, double>>();
            foreach (var item in init.adjList)
                temp.Add(item.Key, new Dictionary<string, double>(item.Value));
            adjList = temp;
            nov = new Dictionary<string, string>();
        }

        public bool Weighted//Свойства графа
        {
            get
            {
                return weighted;
            }
        }

        public bool Oriented
        {
            get
            {
                return oriented;
            }
        }

        public virtual bool AddNode(string name)//Метод для добавления вершины
        {
            if (!adjList.ContainsKey(name))//Если вершины еще нет, то добавляем
            {
                Dictionary<string, double> temp = new Dictionary<string, double>();
                adjList.Add(name, temp);
                return true;
            }
            else
                throw new Exception("Such node already exists");
        }

        public virtual bool DeleteNode(string name)//Удаление вершины
        {
            if (adjList.ContainsKey(name))//Если вершина есть, удаляем ее и инцедентные её ребра
            {
                adjList.Remove(name);
                foreach (var item in adjList)
                    foreach (var node in item.Value)
                        if (node.Key == name)
                            item.Value.Remove(name);
                return true;
            }
            else
                throw new Exception("There is no such node");
        }

        public virtual bool AddEdge(string first, string second, double weight)//Добавление ребра/дуги
        {
            if (oriented)//В случае орграфа добавляем дугу
            {
                if (!adjList.ContainsKey(first) || !adjList.ContainsKey(second))
                {
                    throw new Exception("There are no such nodes");
                }
                else
                {
                    if (!adjList[first].ContainsKey(second))
                    {
                        adjList[first].Add(second, weight);
                        return true;
                    }
                    else
                        throw new Exception("Such edge already exists");
                }
            }
            else//иначе добавляем ребро, в обоих случаях проверяем наличие вершин
            {
                if (!adjList.ContainsKey(first) || !adjList.ContainsKey(second))
                {
                    throw new Exception("There are no such nodes");
                }
                else
                {
                    if (!adjList[first].ContainsKey(second))
                    {
                        adjList[first].Add(second, weight);
                        adjList[second].Add(first, weight);
                        return true;
                    }
                    else
                        throw new Exception("Such edge already exists");
                }
            }
        }

        public virtual bool DeleteEdge(string first, string second)//Удаление ребра/дуги
        {
            if (oriented)//Проверяем на ориентированность
            {
                if (adjList.ContainsKey(first) && adjList[first].ContainsKey(second))
                {
                    if (adjList[first].ContainsKey(second))
                    {
                        adjList[first].Remove(second);//удаляем дугу
                        return true;
                    }
                    else
                        throw new Exception("There is no such edge");
                }
                else
                    throw new Exception("There are no such nodes");
            }
            else
                if (adjList.ContainsKey(first) && adjList[first].ContainsKey(second))
            {
                if (adjList[first].ContainsKey(second))
                {
                    if (first != second)
                    {
                        adjList[first].Remove(second);//удаляем ребро
                        adjList[second].Remove(first);
                    }
                    else
                        adjList[first].Remove(second);
                    return true;
                }
                else
                    throw new Exception("There is no such edge");
            }
            else
                throw new Exception("There are no such nodes");
        }

        public virtual void Show()//Вывод графа в консоль
        {
            foreach (var item1 in adjList)
            {
                Console.Write(item1.Key + ": ");
                if (item1.Value != null)
                    foreach (var item2 in item1.Value)
                    {
                        if (weighted)
                            Console.Write(item2.Key + " " + item2.Value + "; ");//если взвешенный, выводим веса
                        else
                            Console.Write(item2.Key + "; ");
                    }
                Console.WriteLine();
            }
        }
        public virtual void Show(string name)//вывод в файл
        {
            using (StreamWriter fileIn = new StreamWriter(name))
            {
                if (weighted)
                    fileIn.WriteLine("True");
                else
                    fileIn.WriteLine("False");
                if (oriented)
                    fileIn.WriteLine("True");
                else
                    fileIn.WriteLine("False");
                foreach (var item1 in adjList)
                {
                    fileIn.Write(item1.Key + "|");
                    if (item1.Value != null)
                        foreach (var item2 in item1.Value)
                        {
                            if (weighted)
                                fileIn.Write(item2.Key + "|" + item2.Value + "|");//если взвешенный, выводим веса
                            else
                                fileIn.Write(item2.Key + "|");
                        }
                    fileIn.WriteLine();
                }
            }
        }

        protected virtual List<string> DfsRec(string node, string prev, ref List<string> ans)
        {
            if (adjList.ContainsKey(node))
            {
                ans.Add(node);
                nov.Add(node, prev);
                foreach (var item in adjList[node])
                {
                    if (!nov.ContainsKey(item.Key))
                        DfsRec(item.Key, node, ref ans);
                }
                return ans;
            }
            else
                throw new Exception("There is no such node");
        }

        public virtual List<string> Dfs(string node)
        {
            if (adjList.ContainsKey(node))
            {
                List<string> ans = new List<string>();
                nov = new Dictionary<string, string>();
                return DfsRec(node, "\0", ref ans);
            }
            else
                throw new Exception("There is no such node");
        }

        public virtual List<string> Bfs(string node)
        {
            nov = new Dictionary<string, string>();
            List<string> ans = new List<string>();

            if (adjList.ContainsKey(node))
            {
                Queue<string> q = new Queue<string>();
                q.Enqueue(node);
                nov.Add(node, "\0");
                while (q.Count != 0)
                {
                    node = q.Dequeue();
                    ans.Add(node);
                    foreach (var item in adjList[node])
                    {
                        if (!nov.ContainsKey(item.Key))
                        {
                            q.Enqueue(item.Key);
                            nov.Add(item.Key, node);
                        }
                    }
                }
                return ans;
            }
            else
                throw new Exception("There is no such node");
        }

        public virtual Dictionary<string, double> InitializeSingleSource(string node)
        {
            Dictionary<string, double> d = new Dictionary<string, double>();

            foreach (var item in adjList.Keys)
            {
                if (item == node)
                    d.Add(item, 0);
                else
                    d.Add(item, double.MaxValue);
            }

            return d;
        }

        public virtual bool Relax(string u, string v, double w, ref Dictionary<string, double> d)
        {
            if (d[v] > d[u] + w)
            {
                d[v] = d[u] + w;
                return true;
            }
            return false;
        }

        public virtual Dictionary<string, double> BellmanFord(string node)
        {
            if (weighted)
            {
                if (adjList.ContainsKey(node))
                {
                    Dictionary<string, double> d = InitializeSingleSource(node);

                    for (int i = 0; i < adjList.Keys.Count - 1; i++)
                    {
                        foreach (var item1 in adjList.Keys)
                        {
                            foreach (var item2 in adjList[item1].Keys)
                            {
                                Relax(item1, item2, adjList[item1][item2], ref d);
                            }
                        }
                    }

                    for (int i = 0; i < adjList.Keys.Count - 1; i++)
                    {
                        foreach (var item1 in adjList.Keys)
                        {
                            foreach (var item2 in adjList[item1].Keys)
                            {
                                if (d[item2] > d[item1] + adjList[item1][item2])
                                {
                                    throw new Exception("There is a cycle of negative length in the graph");
                                }
                            }
                        }
                    }


                    return d;
                }
                else
                    throw new Exception("There is no such node");
            }
            else
                throw new Exception("You need to choose a weighted graph");
        }

        public virtual Dictionary<string, double> BellmanFordUpgraded(string node)
        {
            if (weighted)
            {
                if (adjList.ContainsKey(node))
                {
                    Graph temp = new Graph(this);
                    Dictionary<string, double> d = InitializeSingleSource(node);

                    for (int i = 0; i < temp.adjList.Keys.Count - 1; i++)
                    {
                        foreach (var item1 in temp.adjList.Keys)
                        {
                            foreach (var item2 in temp.adjList[item1].Keys)
                            {
                                Relax(item1, item2, temp.adjList[item1][item2], ref d);
                            }
                        }
                    }

                    List<string> cycle = new List<string>();

                    for (int i = 0; i < temp.adjList.Keys.Count - 1; i++)
                    {
                        foreach (var item1 in temp.adjList.Keys)
                        {
                            foreach (var item2 in temp.adjList[item1].Keys)
                            {
                                if (d[item2] > d[item1] + temp.adjList[item1][item2] && !cycle.Contains(item2))
                                {
                                    cycle.Add(item2);
                                }
                            }
                        }
                    }

                    foreach(var item in cycle)
                    {
                        Queue<string> Q = new Queue<string>();
                        Q.Enqueue(item);

                        while (Q.Count != 0)
                        {
                            string cur = Q.Dequeue();
                            foreach(var item2 in temp.adjList[cur].Keys)
                            {
                                Q.Enqueue(item2);
                                temp.DeleteEdge(cur, item2);
                            }
                        }
                    }

                    return temp.BellmanFord(node);
                }
                else
                    throw new Exception("There is no such node");
            }
            else
                throw new Exception("You need to choose a weighted graph");
        }

        public virtual Dictionary<string, double> Dijkstra(string node)
        {
            if (weighted)
            {
                if (adjList.ContainsKey(node))
                {
                    Dictionary<string, double> d = InitializeSingleSource(node);
                    Dictionary<string, double> Q = new Dictionary<string, double>();

                    foreach (var item in d)
                        Q.Add(item.Key, item.Value);

                    while (Q.Count > 0)
                    {
                        string u = Q.MinBy(x => x.Value).Key;
                        Q.Remove(u);

                        foreach (var item in adjList[u].Keys)
                        {
                            if (Q.ContainsKey(item))
                            {
                                if (Relax(u, item, adjList[u][item], ref d))
                                {
                                    Q[item] = d[item];
                                }
                            }
                        }
                    }

                    return d;
                }
                else
                    throw new Exception("There is no such node");
            }
            else
                throw new Exception("You need to choose a weighted graph");
        }

        public virtual Dictionary<string, Dictionary<string, double>> Floyd()
        {
            if (weighted)
            {
                Dictionary<string, Dictionary<string, double>> D = new Dictionary<string, Dictionary<string, double>>();

                foreach (var item in adjList.Keys)
                {
                    D.Add(item, new Dictionary<string, double>());
                    foreach (var item2 in adjList.Keys)
                    {
                        if (item == item2)
                            D[item].Add(item2, 0);
                        else
                            if (adjList[item].ContainsKey(item2))
                            D[item].Add(item2, adjList[item][item2]);
                        else
                            D[item].Add(item2, double.MaxValue);
                    }
                }

                foreach (var k in adjList.Keys)
                {
                    foreach (var j in adjList.Keys)
                    {
                        foreach (var i in adjList.Keys)
                        {
                            D[i][j] = Math.Min(D[i][j], D[i][k] + D[k][j]);
                        }
                    }
                }

                return D;
            }
            else
                throw new Exception("You need to choose an weighted graph");
        }

        public virtual List<string> ShowHangingNodes()
        {
            if (!oriented)
            {
                var nodes = new List<string>();

                foreach (var item in adjList)
                {
                    if (item.Value.Count == 1)
                        nodes.Add(item.Key);
                }

                return nodes;
            }
            else
                throw new Exception("You need to choose an ordinary graph");
        }

        public virtual List<string> ShowCircled(string node)
        {
            if (oriented)
            {
                var nodes = new List<string>();

                if (adjList.ContainsKey(node))
                {
                    foreach (var item in adjList[node])
                        if (adjList[item.Key].ContainsKey(node))
                            nodes.Add(item.Key);

                    return nodes;
                }
                else
                    throw new Exception("There is no such node");
            }
            else
                throw new Exception("You need to choose an oriented graph");
        }

        public virtual void DeleteHanging(Graph init)
        {
            if (!oriented)
            {
                foreach (var item in init.adjList.Keys)
                {
                    if (init.adjList[item].Count == 1)
                    {
                        foreach (var node in init.adjList[item].Keys)
                        {
                            if (this.adjList[item].ContainsKey(node))
                                this.DeleteEdge(item, node);
                            break;
                        }

                    }
                }
            }
            else
                throw new Exception("You need to choose an ordinary graph");
        }

        public virtual bool IsStronglyConnected()
        {
            if (oriented)
            {
                if (adjList.Keys.Count != 0)
                {
                    foreach (var item in adjList.Keys)
                    {
                        if (Dfs(item).Count != adjList.Keys.Count)
                        {
                            return false;
                        }
                    }
                    return true;
                }
                else
                    throw new Exception("Graph is empty");
            }
            else
                throw new Exception("You need to choose an oriented graph");
        }

        public virtual bool IsConnected()
        {
            if (!oriented)
            {
                if (adjList.Keys.Count != 0)
                {
                    foreach (var item in adjList.Keys)
                    {
                        if (Dfs(item).Count != adjList.Keys.Count)
                        {
                            return false;
                        }
                        return true;
                    }
                    return true;
                }
                else
                    throw new Exception("Graph is empty");
            }
            else
                throw new Exception("You need to choose not oriented graph");
        }

        public virtual Dictionary<string, Stack<string>> ShortestWays(string node)
        {
            if (adjList.ContainsKey(node))
            {
                Dictionary<string, Stack<string>> ways = new Dictionary<string, Stack<string>>();
                foreach (var item in adjList.Keys)
                    if (item != node)
                        ways.Add(item, new Stack<string>());

                Bfs(node);

                foreach (var item in ways.Keys)
                {
                    if (nov.ContainsKey(item))
                    {
                        string temp = item;
                        while (temp != "\0")
                        {
                            ways[item].Push(temp);
                            temp = nov[temp];
                        }
                    }
                }

                return ways;
            }
            else
                throw new Exception("There is no such node");
        }

        public virtual void Prim(string node, ref Graph result)
        {
            if (weighted && !oriented)
            {
                if (adjList.ContainsKey(node))
                {
                    Graph temp = new Graph(true, false);
                    result.AddNode(node);
                    temp.AddNode(node);

                    while (temp.adjList.Keys.Count != this.Bfs(node).Count)
                    {
                        double min = double.MaxValue;
                        string second = "";
                        string first = "";

                        foreach (var item in temp.adjList.Keys)
                        {
                            foreach (var item2 in adjList[item].Keys)
                            {
                                if (adjList[item][item2] < min && !temp.adjList.ContainsKey(item2) && first != item2)
                                {
                                    first = item;
                                    min = adjList[item][item2];
                                    second = item2;
                                }
                            }
                        }
                        if (min != double.MaxValue)
                        {
                            temp.AddNode(second);
                            temp.AddEdge(first, second, min);
                            result.AddNode(second);
                            result.AddEdge(first, second, min);
                        }
                    }
                }
                else
                    throw new Exception("There is no such node");
            }
            else
                throw new Exception("You need to choose weighted, not oriented graph");
        }

        public virtual Graph Frame(string node)
        {
            if (weighted && !oriented)
            {
                if (adjList.ContainsKey(node))
                {
                    Graph result = new Graph(true, false);

                    if (IsConnected())
                    {
                        Prim(node, ref result);
                        return result;
                    }
                    else
                    {
                        foreach (var item in adjList.Keys)
                            if (!result.adjList.ContainsKey(item))
                                Prim(item, ref result);
                        return result;
                    }
                }
                else
                    throw new Exception("There is no such node");
            }
            else
                throw new Exception("You need to choose weighted, not oriented graph");
        }

        public virtual List<string> NSet(string node, double n)
        {
            if (weighted)
            {
                if (adjList.ContainsKey(node))
                {
                    List<string> nodes = new List<string>();

                    foreach (var item in adjList.Keys)
                    {
                        if (item != node && Dijkstra(item)[node] <= n)
                        {
                            nodes.Add(item);
                        }
                    }

                    return nodes;
                }
                else
                    throw new Exception("There is no such node");
            }
            else
                throw new Exception("You need to choose a weighted graph");
        }

        public virtual List<string> Eccentricity()
        {
            if (weighted)
            {
                List<string> nodes = new List<string>();

                var D = Floyd();
                double R = double.MaxValue;

                foreach (var item1 in adjList.Keys)
                {
                    double max = double.MinValue;

                    foreach (var item2 in adjList.Keys)
                    {
                        if (D[item2][item1] >= max && D[item2][item1] != double.MaxValue)
                            max = D[item2][item1];
                    }

                    if (max < R)
                    {
                        nodes = new List<string>();
                        R = max;
                    }

                    if (max == R)
                        nodes.Add(item1);
                }

                return nodes;
            }
            else
                throw new Exception("You need to choose a weighted graph");
        }

        public virtual double FordFalkerson(string outNode, string inNode)
        {
            if (weighted)
            {
                Graph temp = new Graph(this);
                double flow = 0;

                while (true)
                {
                    temp.nov = new Dictionary<string, string>();
                    temp.nov.Add(outNode, "\0");
                    List<string> nodes = temp.Bfs(outNode);
                    if (!nodes.Contains(inNode))
                        break;
                    else
                    {
                        double min = double.MaxValue;
                        string first = "";
                        string second = first;
                        string cur = inNode;

                        while (temp.nov[cur] != "\0")
                        {
                            if (temp.adjList[temp.nov[cur]][cur] <= min && temp.adjList[temp.nov[cur]][cur] > 0)
                            {
                                min = temp.adjList[temp.nov[cur]][cur];
                                first = temp.nov[cur];
                                second = cur;
                            }
                            cur = temp.nov[cur];
                        }


                        cur = inNode;
                        while (temp.nov[cur] != "\0")
                        {
                            if(temp.adjList[temp.nov[cur]][cur] == 0)
                                temp.adjList[temp.nov[cur]][cur] += min;
                            else
                                temp.adjList[temp.nov[cur]][cur] -= min;
                            cur = temp.nov[cur];
                        }

                        temp.DeleteEdge(first, second);
                        flow += min;
                    }
                }

                return flow;
            }
            else
                throw (new Exception("You need to choose weighted graph"));
        }
    }
}
