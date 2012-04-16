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
        
        /// <summary>
        /// Graph name
        /// </summary>
        public string Name              { get; set; }

        /// <summary>
        /// Graph super type name
        /// </summary>
        public string Type              { get; set; }

        /// <summary>
        /// Graph type name
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
        /// Boolean value for children graphs
        /// </summary>
        public bool isChild             { get; set; }

        private bool CompileAndExecute  { get; set; }

        /// <summary>
        /// Constuctor
        /// </summary>
        /// <param name="name">Name of the graph</param>
        /// <param name="type">Type of the graph</param>
        /// <param name="typeName">Internal name of the graph type</param>
        /// <param name="areaID">Graphs area Id</param>
        /// <param name="objectID">Graphs object Id</param>
       	public Graph(string name, string type, string typeName, int areaID, int objectID) 
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
            string _typeName;
            if (GetTypeNameTable().ContainsKey(_graphType.name))
            {
                _typeName = (string)GetTypeNameTable()[_graphType.name];
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

        /// <summary>
        /// Getter for type name table
        /// </summary>
        /// <returns>TypeNameTable</returns>
        public static Hashtable GetTypeNameTable()
        {
            return TypeNameTable;
        }

        /// <summary>
        /// Set TypeNameTable empty table
        /// </summary>
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
        /// to import project with same name as the graph to workspace. Used for MetaEdit+ 5.0 API
        ///</summary>
        ///<param name="generator">generator name of the generator to be run.</param>
        ///<param name="autobuild"></param>
	    public void ExecuteGenerator(string generator) {
            this.WritePluginIniFile();
            MetaEditAPI.MetaEditAPI port = Launcher.Port;
            
            // Run generator
            this.RunGenerator(port, generator); // Since this method doesn't return anything the RunSuccess value can not be used in every case and is not reliable.
                                                    // But at least at some cases we get result that sets the RunSuccess false and then are the following tasks not executed.
            this.ReadAndRemoveIniFile(Settings.GetSettings().WorkingDir);
            this.ImportProject();
	    }


        /// <summary>
        /// Runs the named generator
        /// </summary>
        /// <param name="port">Connection to MetaEdit+ API server.</param>
        /// <param name="generator">Name of the generator</param>
        /// <returns>return value from MetaEdit+ API server.</returns>
        public void RunGenerator(MetaEditAPI.MetaEditAPI port, String generator)
        {
            if (Settings.GetSettings().is50)
            {
                MEAPI.AllowSetForegroundWindow();
                port.forGraphRun(this.ToMEOop(), generator);
            }
            else
            {
                MetaEditAPI.MENull meNull = new MetaEditAPI.MENull();
                try
                {
                    MEAPI.AllowSetForegroundWindow();
                    port.forName(meNull, this.Name, this.TypeName, "Autobuild");
                }
                catch (Exception e)
                {
                    DialogProvider.ShowMessageDialog("API error: " + e.Message, "API error");
                }
            } 
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
            if (h.GetSetting("runGenerated").Equals("true")) this.CompileAndExecute = true;
            Importer.RemoveIniFile(Settings.GetSettings().WorkingDir);
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
