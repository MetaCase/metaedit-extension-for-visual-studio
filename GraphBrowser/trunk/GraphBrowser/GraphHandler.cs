using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;
using System.IO;

namespace MetaCase.GraphBrowser
{
    class GraphHandler
    {
        /**
	     * Initializes the graph view by getting the graphs from MetaEdit.
	     * @return Array of graphs.
	     * @throws RemoteException 
	     */
	    public static Graph [] Init() {
            MetaEditAPIPortTypeClient port = Launcher.Port;
		    METype graphType = new METype();
		    graphType.name = "Graph";
		    List<Graph> graphs = new List<Graph>();
		    List<Graph> topLevelGraphs = new List<Graph>();
		    MEOop [] meOops = new MEOop[0];
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

	    /**
	     * Reads usernames and passwords or projects from manager.ab file depending on the section parameter.
	     * @param path to manager.ab file.
	     * @param section if "areas" reads the project names. If "users" reads usernames and passwords and returns
	     * them as single String separated with ';'. (eg. "root;root")
	     * @return Array containing Strings.
	     */
	    public static String [] ReadFromManagerAb(String path, String section) {
		    List<String> list = new List<String>();

            String line;
            // if path is null or does not exist return.
            if (!File.Exists(path.ToString())) return list.ToArray();
            // Read the file and display it line by line.
            System.IO.StreamReader file = new System.IO.StreamReader(path.ToString());
            while ((line = file.ReadLine()) != null)
            {
	    		if (line.Contains("[" + section + "]")) {
                    while (!(line = file.ReadLine()).StartsWith("["))
                    {
	    				if (line.Length > 1) {
	    					if (section == "areas") list.Add(ParseProjectFromLine(line));
	    					if (section == "users") list.Add(ParseNameAndPasswordFromLine(line));
	    			    }
	    		    }  
	    	    }
	    	}
            list.RemoveAll(item => item == null);
		    return list.ToArray();
	    }

	    /**
	     * Parses project name from manager.ab file line.
	     * @param line read from manager.ab
	     * @return project name.
	     */
	    private static String ParseProjectFromLine(String line) {	
		    String [] inValidProjects = {"Administration-Common", "Administration-System" };
            String project = line.Split(new Char[] { ';' })[1];
		    for (int i=0; i<inValidProjects.Length; i++) {
			    if (project.Equals(inValidProjects[i])) return null;
		    }
		    return project;
	    }
	
	    /**
	     * Parses name and password from manager.ab file line
	     * @param line read from manager.ab [users] section.
	     * @return name and password (name;password)
	     */
	    private static String ParseNameAndPasswordFromLine(String line) {
            String[] splitted = line.Split(new Char[] { ';' });
		    return splitted[1] + ";" + splitted[2];
	    }
    }
}
