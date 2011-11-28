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

        ///<summary>
        ///Reads usernames and passwords or projects from manager.ab file depending on the section parameter.
        ///</summary>
        ///<param name="path">path to manager.ab file.</param>
        ///<param name="section">
        ///section if "areas" reads the project names. If "users" reads usernames and passwords and returns 
        ///them as single String separated with ';'. (eg. "root;root")
        ///</param>
        ///<returns>
        ///Array containing the strings.
        ///</returns>
	    public static String [] ReadFromManagerAb(String path, String section) {
		    List<String> list = new List<String>();

            String line;
            // if path is null or does not exist return.
            if (!File.Exists(path.ToString())) return list.ToArray();
            // Read the file and display it line by line.
            using (StreamReader reader = new StreamReader(path.ToString()))
            {
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.Contains("[" + section + "]"))
                    {
                        while (!(line = reader.ReadLine()).StartsWith("["))
                        {
                            if (line.Length > 1)
                            {
                                switch (section)
                                {
                                    case "areas":
                                        list.Add(ParseProjectFromLine(line));
                                        break;
                                    case "users":
                                        list.Add(ParseNameAndPasswordFromLine(line));
                                        break;
                                }
                                //if (section == "areas") list.Add(ParseProjectFromLine(line));
                                //if (section == "users") list.Add(ParseNameAndPasswordFromLine(line));
                            }
                        }
                    }
                }
            }
            list.RemoveAll(item => item == null);
		    return list.ToArray();
	    }

        ///<summary>
        ///Parses project name from manager.ab file line.
        ///</summary>
        ///<param name="line">line to read from manager.ab file</param>
        ///<returns>Project name</returns>
	    private static String ParseProjectFromLine(String line) {	
		    String [] inValidProjects = {"Administration-Common", "Administration-System" };
            String project = line.Split(new Char[] { ';' })[1];
		    for (int i=0; i<inValidProjects.Length; i++) {
			    if (project.Equals(inValidProjects[i])) return null;
		    }
		    return project;
	    }

        ///<summary>
        ///Parses name and password from manager.ab file line
        ///</summary>
        ///<param name="line">line to read from manager.ab file [users] section.</param>
        ///<returns>name and password in string.</returns>
	    private static String ParseNameAndPasswordFromLine(String line) {
            String[] splitted = line.Split(new Char[] { ';' });
		    return splitted[1] + ";" + splitted[2];
	    }
    }
}
