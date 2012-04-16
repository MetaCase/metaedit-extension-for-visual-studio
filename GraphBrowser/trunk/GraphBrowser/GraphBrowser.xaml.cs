using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;

namespace MetaCase.GraphBrowser
{
    /// <summary>
    /// Interaction logic for GraphBrowser.xaml
    /// </summary>
    public partial class GraphBrowser : UserControl
    {
        private SettingsWindow sWindow;

        /// <summary>
        /// Settings window instance. Uses only one instance at time.
        /// </summary>
        public SettingsWindow SettingsWindow
        {
            get
            {
                if (sWindow == null || sWindow.IsLoaded == false)
                {
                    sWindow = new SettingsWindow();
                }
                return sWindow;
            }
            set
            {
                sWindow = value;
            }

        }

        private static bool pShowGraphType;

        /// <summary>
        /// 
        /// </summary>
        public static bool ShowGraphType 
        {
            get
            { 
                return pShowGraphType;
            }
            set
            {
                pShowGraphType = value;
            }
        }

        private bool  IsCtrlPressed
        {
            get
            {
                return Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public GraphBrowser()
        {
            InitializeComponent();
            Launcher.connectionAlive = true;
            this.initializeTreeView();
            Settings s = Settings.GetSettings();
            setView();
        }

        /// <summary>
        /// Inits the treeview by calling MetaEdit+ and constructing the Graph tree.
        /// </summary>
        public void initializeTreeView()
        {
            GraphViewModel root = new GraphViewModel();
            root.IsNodeExpanded = true;
            Graph.ResetTypeNameTable();
            Graph[] gs = GraphHandler.Init();
            root.populate(gs, new List<Graph>()); 
            treeView1.ItemsSource = root.getChildren();
            treeView1.ContextMenu = Settings.GetSettings().is50 ? 
                                        treeView1.Resources["50Menu"] as System.Windows.Controls.ContextMenu: 
                                        treeView1.Resources["45Menu"] as System.Windows.Controls.ContextMenu;
            bool _api = this.IsAPI();
            this.SetToolBarButtonsEnabled(_api);
            this.setView(_api);
        }

        private void setView()
        {
            this.setView(this.IsAPI());
        }

        /// <summary>
        /// Sets the view according to the API connection.
        /// </summary>
        private void setView(bool _api)
        {
            GraphView.Visibility = _api ? 
                                      Visibility.Visible:
                                      Visibility.Collapsed;
            ErrorView.Visibility = !_api ?
                                      Visibility.Visible:
                                      Visibility.Collapsed;
        }

        private bool IsAPI()
        {
            return Launcher.IsApiOK();
        }

        private bool Is50()
        {
            return Settings.GetSettings().is50;
        }

        private void UpdateGraphView()
        {
            if (Launcher.initializeAPI(false)) {
                this.treeView1.ItemsSource = null;
                this.initializeTreeView();
            }
            this.setView();
        }

        /// <summary>
        /// Sets every toolbar button enabled or disabled after the conditions
        /// </summary>
        private void SetToolBarButtonsEnabled()
        {
            SetToolBarButtonsEnabled(this.IsAPI());
        }

        /// <summary>
        /// Sets the toolbar buttons enabled or disabled depending on the used MetaEdit+ version,
        /// API connection state and treeview's selection.
        /// </summary>
        /// <param name="_api">API connection state, <code>true</code> if connected.</param>
        private void SetToolBarButtonsEnabled(bool _api)
        {
            bool _is50 = this.Is50();
            bool _isAPI = _api;
            bool _isSelection = (treeView1.SelectedItem != null);

            ButtonGenerateFromList.IsEnabled = (_isAPI && _isSelection && _is50);
            ButtonRunAutobuild.IsEnabled = (_isAPI && _isSelection);
            ButtonOpenInMetaEdit.IsEnabled = (_isAPI && _isSelection);
            ButtonCreateGraph.IsEnabled = (_is50 && _isAPI);
            ButtonUpdateList.IsEnabled = (true);
            ButtonOpenSettings.IsEnabled = (true);
        }

        private void correctErrorSituation()
        {
            bool _api = this.IsAPI();
            if (_api)
            {
                this.initializeTreeView();
            }
            this.setView(_api);
        }

        private void ButtonOpen_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (treeView1.SelectedItem == null) return;
                GraphViewModel gvm = (GraphViewModel)treeView1.SelectedItem;
                MetaEditAPI.MetaEditAPI port = Launcher.Port;
                MEAPI.AllowSetForegroundWindow();
                port.open(gvm.getGraph().ToMEOop());
            }
            catch (Exception err)
            {
                DialogProvider.ShowMessageDialog("API error: " + err.Message, "API error");
                this.correctErrorSituation();
            }
            
        }

        /// <summary>
        /// Action for the "Run Autobuild" button. Runs Autobuild for the selected graph.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonRunAutobuild_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                GraphViewModel gvm = (GraphViewModel)treeView1.SelectedItem;
                if (gvm == null) return;
                gvm.getGraph().ExecuteGenerator("Autobuild");
            }
            catch (Exception err)
            {
                DialogProvider.ShowMessageDialog("API error: " + err.Message, "API error");
                this.correctErrorSituation();
            }
            
        }

        /// <summary>
        /// Run "Generate" action for the selected graph. Shows window containen all the possible generators.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonGenerate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                GraphViewModel gvm = (GraphViewModel)treeView1.SelectedItem;
                if (gvm == null) return;
                Settings s = Settings.GetSettings();
                if (s.is50)
                {
                    string[] _generators = Launcher.Port.generatorNames(gvm.getGraph().GetMEType()).Split(new Char[] { '\r' });
                    List<string> generatorList = new List<string>();
                    foreach (string _generator in _generators)
                    {
                        if (!_generator.StartsWith("_") && !_generator.StartsWith("!"))
                        {
                            generatorList.Add(_generator);
                        }
                    }
                    SelectionWindow sw = new SelectionWindow(generatorList, "Select the generator to run", false, false);
                    sw.Height = 300;
                    sw.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    sw.ShowDialog();
                    string generator = "";
                    if (sw.SelectedItems.Count > 0)
                    {
                        generator = sw.SelectedItems[0];
                        if (generator.Length > 0) gvm.getGraph().ExecuteGenerator(generator);
                    }
                }
                else
                {
                    gvm.getGraph().ExecuteGenerator("Autobuild");
                }
            }
            catch (Exception err)
            {
                DialogProvider.ShowMessageDialog("API error: " + err.Message, "API error");
                this.correctErrorSituation();
            }
        }

        /// <summary>
        /// Treeview update action
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonUpdate_Click(object sender, RoutedEventArgs e)
        {
            this.Cursor = Cursors.Wait;
            Launcher.connectionAlive = true;
            this.initializeTreeView();
            this.setView();
            this.Cursor = Cursors.Arrow;
        }

        /// <summary>
        /// Open settings dialog.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonSettings_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow.Show();
            SettingsWindow.Activate();
        }

        /// <summary>
        /// Opens create a graph -dialog in MetaEdit+
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonCreateGraph_Click(object sender, RoutedEventArgs e)
        {
            this.StartMEDialog(MEDialog.CREATE_NEW_GRAPH);
        }

        /// <summary>
        /// Open edit properties -dialog in MetaEdit+
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonEditProperties_Click(object sender, RoutedEventArgs e)
        {
            this.StartMEDialog(MEDialog.EDIT_GRAPH_PROPERTIES);
        }

        /// <summary>
        /// Open MetaEdit dialog in new thread
        /// </summary>
        /// <param name="DialogType"></param>
        private void StartMEDialog(int DialogType)
        {
            GraphViewModel gvm = null;
            MEDialog dialog = null;
            if (treeView1.SelectedItem != null)
            {
                gvm = (GraphViewModel)treeView1.SelectedItem;
                dialog = new MEDialog(DialogType, gvm.getGraph());
            }
            else
            {
                dialog = new MEDialog(DialogType, null);
            }
            Thread _thread = new Thread(new ThreadStart(dialog.Run));
            _thread.Start();
        }

        /// <summary>
        /// Launches MetaEdit+, or uses available API connection if found.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonStartMetaEdit_Click(object sender, RoutedEventArgs e)
        {
            bool runUpdate = true;
            if (IsAPI())
            {
                DialogProvider.ShowMessageDialog("Found an existing API connection.", "API connection found.");
                runUpdate = true;
            }
            else
            {
                runUpdate = Launcher.DoInitialLaunch();
            }
            if (runUpdate) UpdateGraphView();
        }

        /// <summary>
        /// Toggles graph typename on/off.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonShowTypeName_Clicked(object sender, RoutedEventArgs e)
        {
            // Read the boolean value
            ShowGraphType = ButtonShowTypeName.IsChecked.Value;
            this.treeView1.Items.Refresh();
            // Set focus to treeview
            this.treeView1.Focus();
        }

        private void treeView1_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
           SetToolBarButtonsEnabled();
        }

        private void treeView1_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ButtonOpen_Click(sender, e);
            e.Handled = true;
        }

        private void treeView1_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (IsCtrlPressed)
            {
                // set the last tree view item selected variable which may be used elsewhere as there is no other way I have found to obtain the TreeViewItem container (may be null)
                // TreeViewItem item = (TreeViewItem)(treeView1.ItemContainerGenerator.ContainerFromIndex(treeView1.Items.CurrentPosition));
                GraphViewModel SelectedGvm = (GraphViewModel)this.treeView1.SelectedItem;
                SelectedGvm.IsSelected = false;
                //this.treeView1.Focus();
            }
        }
    }
}