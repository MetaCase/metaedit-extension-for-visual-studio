using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.ObjectModel;

namespace MetaCase.GraphBrowser
{
    /// <summary>
    /// Graph class represents MetaEdit+ graphs.
    /// </summary>
    public class Graph : IComparable
    {
        Graph[] children = new Graph[0];
        private static Hashtable ProjectTable = new Hashtable();
        private static Hashtable TypeNameTable = new Hashtable();
        
        /// <summary>
        /// Graph name
        /// </summary>
        public string Name              { get; set; }

        /// <summary>
        /// Graph type's internal name, e.g. Graph_WatchApplication_user_10232123
        /// </summary>
        public string Type              { get; set; }

        /// <summary>
        /// Graph type's user-visible name, e.g. Watch Application
        /// </summary>
        public string TypeName          { get; set; }

        /// <summary>
        /// Graph area id
        /// </summary>
        public int AreaID               { get; set; }

        /// <summary>
        /// Graph object id
        /// </summary>
        public int ObjectID             { get; set; }

        /// <summary>
        /// Is this graph known to be a subgraph of another graph?
        /// </summary>
        public bool isChild             { get; set; }

        /// <summary>
        /// Constructor - PRIVATE: use static MEOopToGraph instead
        /// </summary>
        /// <param name="name">Name of the graph</param>
        /// <param name="type">Type of the graph</param>
        /// <param name="typeName">Internal name of the graph type</param>
        /// <param name="areaID">Graphs area Id</param>
        /// <param name="objectID">Graphs object Id</param>
       	private Graph(string name, string type, string typeName, int areaID, int objectID) 
        {
		    this.Name = name;
		    this.Type = type;
            this.TypeName = typeName;
		    this.AreaID = areaID;
		    this.ObjectID = objectID;
	    }

        /// <summary>
        /// Creates graph from MEOop, or returns it from cache if already there
        /// </summary>
        /// <param name="m">The MEOop object</param>
        /// <returns>Graph object</returns>
        public static Graph MEOopToGraph(MetaEditAPI.MEOop m) 
        {
            Hashtable graphTable = (Hashtable) ProjectTable[m.areaID];
		    Graph graph = null;
            if (graphTable == null)
            {
                graphTable = new Hashtable();
                ProjectTable.Add(m.areaID, graphTable);
            }
			graph = (Graph) graphTable[m.objectID];
            MetaEditAPI.MetaEditAPI port = Launcher.Port;

		    if (graph == null) {
                MetaEditAPI.METype _graphType = port.type(m);
                string _typeName = (string) TypeNameTable[_graphType.name];
                if (_typeName == null)
                {
                    _typeName = port.typeName(_graphType);
                    TypeNameTable.Add(_graphType.name, _typeName);
                }

                graph = new Graph(port.userPrintString(m), _graphType.name, _typeName, m.areaID, m.objectID);
                graphTable.Add(m.objectID, graph);
		    }
		    return graph;
	    }

        /// <summary>
        /// Resets all cached graph and type information from MetaEdit+
        /// </summary>
        public static void ResetCaches()
        {
            ProjectTable = new Hashtable();
            TypeNameTable = new Hashtable();
        }

        /// <summary>
        /// Returns children of the graph
        /// </summary>
        /// <returns>Array containing the children.</returns>
	    public Graph[] GetChildren() {
		    return this.children;
	    }
	
        /// <summary>
        /// Creates MEOop from Graph
        /// </summary>
        /// <returns>created MEOop</returns>
        public MetaEditAPI.MEOop ToMEOop()
        {
            MetaEditAPI.MEOop m = new MetaEditAPI.MEOop();
            m.areaID = this.AreaID;
            m.objectID = this.ObjectID;
            return m;
	    }
	
	    /// <summary>
	    /// Gets the MeType of Graph
	    /// </summary>
	    /// <returns>METype</returns>
        public MetaEditAPI.METype GetMEType()
        {
            MetaEditAPI.METype type = new MetaEditAPI.METype();
		    type.name = this.Type;
		    return type;
	    }
	
        /// <summary>
        /// Graph ToString method
        /// </summary>
        /// <returns>Graph name</returns>
	    public override string ToString() {
		    return this.Name;
	    }
	
        /// <summary>
        /// Sets the graph isChild property showing if the graph is subgraph of another graph or not. 
        /// </summary>
	    public void SetIsChild(Boolean _isChild){
		    this.isChild = _isChild;
	    }

        ///<summary>
        /// Getter for graphs isChild property.  
        ///</summary>
	    public Boolean getIsChild(){
		    return this.isChild;
	    }
	
        ///<summary>
        /// Setter for children array.
        ///</summary>
	    public void SetChildren(Graph[] children){
		    this.children = children;
	    }

        ///<summary>
        /// Runs generator for caller Graph. After calling ME+ to run generator, tries
        /// to import project with same name as the graph to workspace. 
        ///</summary>
        ///<param name="generator">generator name of the generator to be run.</param>
	    public void ExecuteGenerator(string generator) {
            this.WritePluginIniFile();
            MetaEditAPI.MetaEditAPI port = Launcher.Port;
            
            // Run generator
            bool RunSuccess = this.RunGenerator(port, generator); // true if successful or unknown
            this.ReadAndRemoveIniFile(Settings.GetSettings().WorkingDir);
            if (RunSuccess) this.ImportProject();
	    }


        /// <summary>
        /// Runs the named generator
        /// </summary>
        /// <param name="port">Connection to MetaEdit+ API server.</param>
        /// <param name="generator">Name of the generator</param>
        /// <returns>Was the generation successful (unknown=true, e.g. for 4.5)</returns>
        public bool RunGenerator(MetaEditAPI.MetaEditAPI port, String generator)
        {
            bool success = true;
            MEAPI.AllowSetForegroundWindow();
            try
            {
                if (Settings.GetSettings().Version.IsEqualOrGreaterThan("5.0"))
                {
                    success = port.forGraphRun(this.ToMEOop(), generator);
                }
                else
                {
                    MetaEditAPI.MENull meNull = new MetaEditAPI.MENull();
                    port.forName(meNull, this.Name, this.TypeName, generator);
                }
            }
            catch (Exception e)
            {
                DialogProvider.ShowMessageDialog("API error: " + e.Message, "API error");
                success = false;
            }
            return success;
	    }

        /// <summary>
        /// Calls for project import method
        /// </summary>
        public void ImportProject()
        {
            // Try to import generated project.
            try
            {
                Importer.ImportProject(this.ToString());
            }
            catch (Exception)
            {
                // Maybe no visual studio solution was generated.
            }
        }

        /// <summary>
        /// Writes the plugin.ini file for the MetaEdit+ generator
        /// </summary>
        private void WritePluginIniFile()
        {
            Importer.WritePluginIniFile(Settings.GetSettings().WorkingDir, this.ToString());
        }

        /// <summary>
        /// Removes the written INI file.
        /// </summary>
        /// <param name="path">Path to the file.</param>
        private void ReadAndRemoveIniFile(string path)
        {
            IniParser h = new IniParser(path + "\\plugin.ini");
            Importer.RemoveIniFile(Settings.GetSettings().WorkingDir);
        }

        public int CompareTo(object obj)
        {
            int result = 0;
            if (obj is Graph)
            {
                result = this.Name.CompareTo((obj as Graph).Name);
                if (result == 0)
                {
                    result = this.TypeName.CompareTo((obj as Graph).TypeName);
                }
            }
            return result;
        }

        /// <summary>
        /// Init graphs children recursively calling method for every children. Except 
        /// those that are already initialized.
        /// </summary>
        public void InitChildren(MetaEditAPI.MetaEditAPI port, List<Graph> done)
        {
		    if (done.Contains(this)) return;
		    done.Add(this);
            MetaEditAPI.MEOop[] subgraphOops = null;
		    subgraphOops = port.subgraphs(this.ToMEOop());
		    // Set the subgraph items to be children of this graph.
		    if (subgraphOops.Length > 0 && subgraphOops != null) {
			    Graph [] graphs = new Graph[subgraphOops.Length];
			    for (int i=0; i < subgraphOops.Length; i++){
				    Graph g = graphs[i] = MEOopToGraph(subgraphOops[i]);
				    g.SetIsChild(true);
				    g.InitChildren(port, done); 
			    }
                this.SetChildren(graphs);
		     }
	    }
    }
}
