using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;
using System.IO;
using MetaEditAPI;

namespace MetaCase.GraphBrowser
{
    class GraphHandler
    {
        ///<summary>
        /// Initializes the graph view by getting the graphs from MetaEdit.
        ///</summary>
        ///<returns>Array of graphs.</returns>
	    public static Graph [] Init() {
            MetaEditAPI.MetaEditAPI port = Launcher.Port;
            METype graphType = new MetaEditAPI.METype();
		    graphType.name = "Graph";
		    List<Graph> graphs = new List<Graph>();
		    List<Graph> topLevelGraphs = new List<Graph>();
            MetaEditAPI.MEOop[] meOops = new MetaEditAPI.MEOop[0];
		    if (!Launcher.IsApiOK()) return topLevelGraphs.ToArray();
		    try {
                meOops = port.allSimilarInstances(graphType);
		    } catch (Exception e) { 
			    DialogProvider.ShowMessageDialog("API error: " + e.Message, "API error");
			}
		    foreach (MEOop m in meOops) {
			    Graph g = Graph.MEOopToGraph(m);
			    graphs.Add(g);
		    }
		    List<Graph> done = new List<Graph>();
		    foreach(Graph g in graphs) {
			    g.InitChildren(port, done);
		    }
		    foreach(Graph g in graphs) {
			    if (!g.getIsChild()) topLevelGraphs.Add(g);
		    }
		    List<Graph> reachableGraphsList = ReachableGraphs(topLevelGraphs);

            graphs.Sort(delegate(Graph g1, Graph g2) { return g1.Name.CompareTo(g2.Name); });
		
		    foreach(Graph g in graphs) {
			    if (!reachableGraphsList.Contains(g)) {
				    topLevelGraphs.Add(g);
				    BuildReachableGraphs(g, reachableGraphsList);
			    }
		    }
            return topLevelGraphs.ToArray();
	    }
	
	    private static List<Graph> ReachableGraphs(List<Graph> topLevelGraphs) {
		    List<Graph> done = new List<Graph>();
		    foreach(Graph g in topLevelGraphs) {
			    BuildReachableGraphs(g, done);
		    }
		    return done;
	    }
	
	    private static void BuildReachableGraphs(Graph g, List<Graph> done) {
		    if(done.Contains(g)) return;
		    done.Add(g);
		    foreach(Graph child in g.GetChildren()) {
			    BuildReachableGraphs(child, done);
		    }
	    }
    }
}
