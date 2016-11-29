using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetroParis
{
    /*
    * Implementation of a directed graph
    * */
    public class Digraph
    {
        private readonly Int32 _v; //The number of vertices
        private Int32 _e; //The number of edges
        private LinkedList<Int32>[] adj; //Use a LinkedList for the adjacency-list representation

        //Create a new directed graph with V vertices
        public Digraph(Int32 V)
        {
            if (V < 0) throw new Exception("Number of vertices in a Digraph must be nonnegative");
            this._v = V;
            this._e = 0;
            //Create a new adjecency-list for each vertex
            adj = new LinkedList<Int32>[V];
            for (Int32 v = 0; v < V; v++)
            {
                adj[v] = new LinkedList<Int32>();
            }
        }

        //return the number of vertices
        public Int32 V()
        {
            return _v;
        }

        //return the number of edges
        public Int32 E()
        {
            return _e;
        }

        //Add an edge to the directed graph from v to w
        public void AddEdge(Int32 v, Int32 w)
        {
            if (v < 0 || v >= _v) throw new Exception("vertex " + v + " is not between 0 and " + (_v - 1));
            if (w < 0 || w >= _v) throw new Exception("vertex " + w + " is not between 0 and " + (_v - 1));
            adj[v].AddFirst(w);
            _e++;
        }

        /*
            * Return the adjacency-list for vertex v, which
            * are the vertices connected to v pointing from v
            * */
        public IEnumerable<Int32> Adj(Int32 v)
        {
            if (v < 0 || v >= _v) throw new Exception();
            return adj[v];
        }

        //Return the directed graph as a string
        public String toString()
        {
            StringBuilder s = new StringBuilder();
            String NEWLINE = Environment.NewLine;
            s.Append(_v + " vertices, " + _e + " edges " + NEWLINE);
            for (int v = 0; v < _v; v++)
            {
                s.Append(String.Format("{0:d}: ", v));
                foreach (int w in adj[v])
                {
                    s.Append(String.Format("{0:d} ", w));
                }
                s.Append(NEWLINE);
            }
            return s.ToString();
        }
    }
}
