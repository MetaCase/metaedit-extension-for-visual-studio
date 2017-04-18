# Plugin functions

This is a usage guide page for functions of the {"MetaEdit+ extension for Visual Studio."}

## Commands

The extension provides commands that may be used from buttons in the views toolbar or by selecting a graph from the tree and clicking the mouse right button.

 ![](AdvancedInstructions_http://metaedit-plugin-for-eclipse.googlecode.com/svn/trunk/com.metacase.graphbrowser/icons/run_generator_icon.png) **Run Autobuild** runs the generator named "Autobuild" for the selected graph. If no graph is selected, no action is taken.

 ![](AdvancedInstructions_http://metaedit-plugin-for-eclipse.googlecode.com/svn/trunk/com.metacase.graphbrowser/icons/select_generator_to_run_icon.png) **Select Generator to Run** allows you to select and run a specific generator for the selected graph. If no graph is selected, no action is taken. Since a graph may have more than one generator the extension retrieves all available generators from {"MetaEdit+ and shows them in a list dialog for selection. Depending on the selected generator this command may start several automated steps, like produce the code, importing it to Visual Studio and running the generated solution. See below for further information."}

 ![](AdvancedInstructions_http://metaedit-plugin-for-eclipse.googlecode.com/svn/trunk/com.metacase.graphbrowser/icons/open_graph_in_metaedit_icon.png) **Open Graph in MetaEdit+** opens the same graph in {"MetaEdit+ that is selected in the tree view. If no graph is selected, no action is taken. This way user gets to edit the graphs quickly in MetaEdit+."}

 ![](AdvancedInstructions_http://metaedit-plugin-for-eclipse.googlecode.com/svn/trunk/com.metacase.graphbrowser/icons/update_graph_list_icon.png) **Update Graph List** updates the treeview. Before asking the graphs from {"MetaEdit+, it tests the API connection and tries to launch MetaEdit+ as in the extension startup. This command is useful for both updating the graph list and initializing the whole treeview if the extension was started without API connection."}

 ![](AdvancedInstructions_http://metaedit-plugin-for-eclipse.googlecode.com/svn/trunk/com.metacase.graphbrowser/icons/settings_icon.png) **Open Settings** opens a dialog that contains the {"MetaEdit+ launch parameters. The parameters are saved in 'default.mer' file in the Visual Studio Projects folder and are read when launching MetaEdit+ or when initalizing the settings dialog. The dialog provides a three step verifier for each text field. green when content is correct, yellow if the entry could not be verified, or red when entry is not correct."}

 ![](AdvancedInstructions_http://metaedit-plugin-for-eclipse.googlecode.com/svn/trunk/com.metacase.graphbrowser/icons/create_graph_icon.png) **Create a new Graph** will open a {"MetaEdit+ dialog for creating a new graph. If no graph is selected, a dialog opens to ask which type of graph will be created. If graph is selected the dialog will automatically have the same type as the selected graph."}

![](AdvancedInstructions_http://metaedit-plugin-for-eclipse.googlecode.com/svn/trunk/com.metacase.graphbrowser/icons/folder_explore.png) **Show/hide graph type** toggles the graph typename on or off.

In the popup menu (mouse right click) there is a command that is not shown in the view toolbar. This command is only for {"MetaEdit+ 5.0 version."}

 ![](AdvancedInstructions_http://metaedit-plugin-for-eclipse.googlecode.com/svn/trunk/com.metacase.graphbrowser/icons/edit_properties_icon.png) **Edit Graph Properties** opens graph properties dialog in {"MetaEdit+ and lets user to edit the graph's properties. This function always depends on selected graph so the command is shown in the popup menu only when there is a selection in the tree view."}