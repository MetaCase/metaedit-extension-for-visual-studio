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
    public class Graph
    {
        Graph[] children = new Graph[0];
        static Hashtable ProjectTable = new Hashtable();
        static Hashtable TypeNameTable = new Hashtable();
        public String Name              { get; set; }
        public String Type              { get; set; }
        public String TypeName          { get; set; }
        public int AreaID               { get; set; }
        public int ObjectID             { get; set; }
        public bool isChild             { get; set; }
        private bool CompileAndExecute  { get; set; }

        /// <summary>
        /// Constuctor
        /// </summary>
        /// <param name="name">Name of the graph</param>
        /// <param name="type">Type of the graph</param>
        /// <param name="typeName">Internal name of the graph type</param>
        /// <param name="areaID">Graphs areaID</param>
        /// <param name="objectID">Graphs Object ID</param>
       	public Graph(String name, String type, String typeName, int areaID, int objectID) 
        {
		    this.Name = name;
		    this.Type = type;
            this.TypeName = typeName;
		    this.AreaID = areaID;
		    this.ObjectID = objectID;
	    }

        /// <summary>
        /// Creates graph from MEOop
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

            MetaEditAPI.METype _graphType = port.type(m);
            String _typeName;
            if (GetTypeNameTable().ContainsKey(_graphType.name))
            {
                _typeName = (String)GetTypeNameTable()[_graphType.name];
            }
            else
            {
                _typeName = port.typeName(_graphType);
                GetTypeNameTable().Add(_graphType.name, _typeName);
            }

		    if (graph == null) {
		        graph = new Graph(port.userPrintString(m), _graphType.name, _typeName, m.areaID, m.objectID);
                graphTable.Add(m.objectID, graph);
		    }
		    else {
                graph.Name = port.userPrintString(m);
                graph.Type = _graphType.name;
                graph.TypeName = _typeName;
		    }
		    return graph;
	    }

        public static Hashtable GetTypeNameTable()
        {
            return TypeNameTable;
        }

        public static void ResetTypeNameTable()
        {
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
	
	    public override String ToString() {
		    return this.Name;
	    }
	
	    /**
	     * Sets the graph isChild property showing if the graph is subgraph of
	     * another graph or not.
	     * @param _isChild
	     */
	    public void SetIsChild(Boolean _isChild){
		    this.isChild = _isChild;
	    }
	
	    /**
	     * Getter for graphs isChild property.  
	     * @return isChild boolean property.
	     */
	    public Boolean getIsChild(){
		    return this.isChild;
	    }
	
	    /**
	     * Setter for children array.
	     * @param children - array of children graphs.
	     */
	    public void SetChildren(Graph[] children){
		    this.children = children;
	    }

       /**
	    * Runs generator for caller Graph. After calling ME+ to run generator, tries
	    * to import project with same name as the graph to workspace. Used for MetaEdit+ 5.0 API
	    * @param generator name of the generator to be run.
	    */
	    public void RunGenerator(String generator, bool autobuild) {
            this.WritePluginIniFile();
            MetaEditAPI.MetaEditAPI port = Launcher.Port;
            
            // Run generator
            if (autobuild) this.RunAutobuild(port);
            else this.RunGenerator(port, generator);

            this.RemoveIniFile(Settings.GetSettings().WorkingDir);
            this.ImportProject();
	    }
        

	    /**
	     * Runs Autobuild generator for selected graph. This method is used for MetaEdit+ 4.5 API.
	     */
        public void RunAutobuild(MetaEditAPI.MetaEditAPI port)
        {
            MetaEditAPI.MENull meNull = new MetaEditAPI.MENull();
		    try {
			    port.forName(meNull, this.Name, this.TypeName, "Autobuild");
		    } catch (Exception e) { 
			    DialogProvider.ShowMessageDialog("API error: " + e.Message, "API error");
		    }
	    }

        public void RunGenerator(MetaEditAPI.MetaEditAPI port, String generator)
        {
            port.forGraphRun(this.ToMEOop(), generator);
        }

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

        private void RemoveIniFile(String path)
        {
            IniParser h = new IniParser(path + "\\plugin.ini");
            if (h.GetSetting("runGenerated").Equals("true")) this.CompileAndExecute = true;
            Importer.RemoveIniFile(Settings.GetSettings().WorkingDir);
        }

	    /**
	     * Init graphs children recursively calling method for every children. Except
	     * those that are already initialized.
	     * @param port - Port for API calls.
	     * @param done - Array of graphs that are initialized.
	     * @throws Exception
	     */
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
