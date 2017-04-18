# Plugin functions

This is a usage guide page for functions of the MetaEdit+ extension for Visual Studio.

## Commands

The extension provides commands that may be used from buttons in the views toolbar or by selecting a graph from the tree and clicking the mouse right button.

 ![](https://cloud.githubusercontent.com/assets/9478151/10095473/c3cba6f8-6372-11e5-975d-ba53f87bde72.png) **Run Autobuild** runs the generator named "Autobuild" for the selected graph. If no graph is selected, no action is taken.

 ![](https://cloud.githubusercontent.com/assets/9478151/10095472/c3c93fc6-6372-11e5-8080-bc50ec055704.png) **Select Generator to Run** allows you to select and run a specific generator for the selected graph. If no graph is selected, no action is taken. Since a graph may have more than one generator the extension retrieves all available generators from MetaEdit+ and shows them in a list dialog for selection. Depending on the selected generator this command may start several automated steps, like produce the code, importing it to Visual Studio and running the generated solution. See below for further information.

 ![](https://cloud.githubusercontent.com/assets/9478151/10095465/c3a4da6e-6372-11e5-8166-375abb40183a.png) **Open Graph in MetaEdit+** opens the same graph in MetaEdit+ that is selected in the tree view. If no graph is selected, no action is taken. This way user gets to edit the graphs quickly in MetaEdit+.

 ![](https://cloud.githubusercontent.com/assets/9478151/10095476/c3d2136c-6372-11e5-8a9a-17e39cf77c6c.png) **Update Graph List** updates the treeview. Before asking the graphs from MetaEdit+, it tests the API connection and tries to launch MetaEdit+ as in the extension startup. This command is useful for both updating the graph list and initializing the whole treeview if the extension was started without API connection.

 ![](https://cloud.githubusercontent.com/assets/9478151/10095474/c3cc2998-6372-11e5-81ed-1cf37c80352f.png) **Open Settings** opens a dialog that contains the MetaEdit+ launch parameters. The parameters are saved in 'default.mer' file in the Visual Studio Projects folder and are read when launching MetaEdit+ or when initalizing the settings dialog. The dialog provides a three step verifier for each text field. green when content is correct, yellow if the entry could not be verified, or red when entry is not correct.

 ![](https://cloud.githubusercontent.com/assets/9478151/10095463/c36ed702-6372-11e5-98e6-58418da8b0e4.png) **Create a new Graph** will open a MetaEdit+ dialog for creating a new graph. If no graph is selected, a dialog opens to ask which type of graph will be created. If graph is selected the dialog will automatically have the same type as the selected graph.

![](https://cloud.githubusercontent.com/assets/9478151/10095466/c3ad77d2-6372-11e5-893d-fdafe3053acb.png) **Show/hide graph type** toggles the graph typename on or off.

In the popup menu (mouse right click) there is a command that is not shown in the view toolbar. This command is only for MetaEdit+ 5.0 and later.

 ![](https://cloud.githubusercontent.com/assets/9478151/10095470/c3b737c2-6372-11e5-9fc4-2b5933544d33.png) **Edit Graph Properties** opens graph properties dialog in MetaEdit+ and lets user to edit the graph's properties. This function always depends on selected graph so the command is shown in the popup menu only when there is a selection in the tree view.
