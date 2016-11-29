using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetroParis.Classes
{
    /*
* Use a depth first search method to find a path from source vertex s to target vertex v in a directed
* graph (Digraph).  See a previous post for code for Digraph.
* */
    public class DepthFirstDirectedPaths
    {
        private Boolean[] _marked;  //keep track of whether vertex v is reachable from s
        private Int32[] _edgeTo;  //keep track of the last edge on path s to v
        private readonly Int32 _s; //source vertex

        public DepthFirstDirectedPaths(Digraph G, Int32 s)
        {
            _marked = new Boolean[G.V()]; //create a boolean array for all vertices
            _edgeTo = new Int32[G.V()]; //create a boolean array for all vertices
            this._s = s; //mark the source vertex
            DFS(G, s);
        }

        /*
        * A recursive function to do depth first search.
        * We start with the source vertex s, find all vertices connected to it and recursively call
        * DFS as move out from s to connected vertices.  We avoid "going backwards" or needlessly looking
        * at all paths by keeping track of which vertices we've already visited using the _marked[] array.
        * We keep track of how we're moving through the graph (from s to v) using _edgeTo[].
        */
        private void DFS(Digraph G, Int32 v)
        {
            _marked[v] = true;
            foreach (Int32 w in G.Adj(v))
            {
                if (!_marked[w])
                {
                    _edgeTo[w] = v;
                    DFS(G, w);
                }
            }
        }

        /*
        * In the DFS method we've kept track of vertices connected to the source s
        * using the _marked[] array.
        * */
        public Boolean HasPathTo(Int32 v)
        {
            return _marked[v];
        }

        /*
        * We can find the path from s to v working backwards using the _edgeTo array.
        * For example, if we want to find the path from 3 to 0.  We look at _edgeTo[0] which gives us
        * a vertex, say 2.  We then look at _edgeTo[2] and so on until _edgeTo[x] equals 3 (our 
        * source vertex).
        * */
        public IEnumerable<Int32> PathTo(Int32 v)
        {
            if (!HasPathTo(v)) return null;
            Stack<Int32> path = new Stack<Int32>();
            for (Int32 x = v; x != _s; x = _edgeTo[x])
                path.Push(x);
            path.Push(_s);
            return path;
        }
    }
}
