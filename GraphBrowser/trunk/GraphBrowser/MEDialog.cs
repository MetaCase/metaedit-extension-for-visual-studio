using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using MetaEditAPI;

namespace MetaCase.GraphBrowser
{
   class MEDialog {
	
    	// Dialog types
	    public const int CREATE_NEW_GRAPH = 1;
	    //public static final int CREATE_NEW_GRAPH_OF_SAME_TYPE = 2;
	    public const int EDIT_GRAPH_PROPERTIES = 3;
	    private int dialogType;
	    private Graph selectedGraph;
	
       ///<summary>
       /// Constructor.
       ///</summary>
       ///<param name="dialogType">Dialog type. Use CREATE_NEW_GRAPH or EDIT_GRAPH_PROPERTIES</param>
       ///<param name="selectedGraph">graph that is selected in the treeview or null</param>
	   public MEDialog(int dialogType, Graph selectedGraph) {
	       this.dialogType = dialogType;
	       this.selectedGraph = selectedGraph;
	   }
	
	   ///<summary>
       /// Runs MetaEdit+ dialog.
       ///</summary>
       public void Run() {
	        MetaEditAPI.MetaEditAPI port = Launcher.Port;
	        switch (this.dialogType) {
	    	    case CREATE_NEW_GRAPH:
	    	        // Opens "Create Graph" dialog in MetaEdit+
                    MetaEditAPI.METype m = null;
	    	        if (selectedGraph == null) {
	    		        m = new MetaEditAPI.METype();
	    		        m.name = ("Graph");
	    		    }
	    		    else {
	    		        m = selectedGraph.GetMEType(); 
	    		    }
	    		    port.createGraphDialog(m);
	    	        break;
	    	    case EDIT_GRAPH_PROPERTIES:
	    	        // Opens "Properties" dialog for the selected graph in MetaEdit+
                    if (selectedGraph != null)
                    {
                        port.propertyDialog(this.selectedGraph.ToMEOop());
                    }
	    	        break;
	        }
	    }
    }
}
