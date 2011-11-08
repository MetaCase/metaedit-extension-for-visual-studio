﻿using System;
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

        public GraphBrowser()
        {
            InitializeComponent();
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
            Graph.ResetTypeNameTable();
            Graph[] gs = GraphHandler.Init();
            root.populate(gs, new List<Graph>()); 
            treeView1.ItemsSource = root.getChildren();
            treeView1.ContextMenu = Settings.GetSettings().is50 ? treeView1.Resources["50Menu"] as System.Windows.Controls.ContextMenu : treeView1.Resources["45Menu"] as System.Windows.Controls.ContextMenu;
            this.SetToolBarButtonsEnabled();
            this.setView();
        }

        /// <summary>
        /// Sets the view according to the API connection.
        /// </summary>
        private void setView()
        {
            bool api = this.IsAPI();
            GraphView.Visibility = api ? Visibility.Visible : Visibility.Collapsed;
            ErrorView.Visibility = !api ? Visibility.Visible : Visibility.Collapsed;
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
            Launcher.initializeAPI();
            this.treeView1.ItemsSource = null;
            this.initializeTreeView();
            this.setView();
        }

        /// <summary>
        /// Sets every toolbar button enabled or disabled after the conditions
        /// </summary>
        private void SetToolBarButtonsEnabled()
        {
            bool _is50 = this.Is50();
            bool _isAPI = this.IsAPI();
            bool _isSelection = (treeView1.SelectedItem != null);

            
            ButtonGenerateFromList.IsEnabled = (_isAPI && _isSelection && _is50);
            ButtonRunAutobuild.IsEnabled = (_isAPI && _isSelection);
            ButtonOpenInMetaEdit.IsEnabled = (_isAPI && _isSelection);
            ButtonCreateGraph.IsEnabled = (_is50 && _isAPI);
            //ButtonEditProperties.IsEnabled = (_is50 && _isAPI && _isSelection);
            ButtonUpdateList.IsEnabled = (true);
            ButtonOpenSettings.IsEnabled = (true);
	    
        }

        private void ButtonOpen_Click(object sender, RoutedEventArgs e)
        {
            if (treeView1.SelectedItem == null) return;
            GraphViewModel gvm = (GraphViewModel)treeView1.SelectedItem;
            MetaEditAPIPortTypeClient port = Launcher.Port;
            port.open(gvm.getGraph().ToMEOop());
        }

        private void ButtonRunAutobuild_Click(object sender, RoutedEventArgs e)
        {
            GraphViewModel gvm = (GraphViewModel)treeView1.SelectedItem;
            if (gvm == null) return;
            gvm.getGraph().RunGenerator("Autobuild", true);
        }

        private void ButtonGenerate_Click(object sender, RoutedEventArgs e)
        {
            GraphViewModel gvm = (GraphViewModel)treeView1.SelectedItem;
            if (gvm == null) return;
            Settings s = Settings.GetSettings();
            if (s.is50)
            {
                String[] _generators = Launcher.Port.generatorNames(gvm.getGraph().GetMEType()).Split(new Char[] { '\r' });
                List<String> generatorList = new List<string>();
                foreach (String _generator in _generators)
                {
                    if (!_generator.StartsWith("_") && !_generator.StartsWith("!"))
                    {
                        generatorList.Add(_generator);
                    }
                }
                SelectionWindow sw = new SelectionWindow(generatorList, "Choose the generator you want to run for the graph", false, false);
                sw.Height = 300;
                sw.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                sw.ShowDialog();
                String generator = "";
                if (sw.SelectedItems.Count > 0)
                {
                    generator = sw.SelectedItems[0];
                    if (generator.Length > 0) gvm.getGraph().RunGenerator(generator, false);
                }
            }
            else
            {
                gvm.getGraph().RunGenerator("Autobuild", true);
            }
        }

        private void ButtonUpdate_Click(object sender, RoutedEventArgs e)
        {
            this.Cursor = Cursors.Wait;
            this.initializeTreeView();
            this.setView();
            this.Cursor = Cursors.Arrow;
        }

        private void ButtonSettings_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow.Show();
            SettingsWindow.Activate();
        }

        private void ButtonCreateGraph_Click(object sender, RoutedEventArgs e)
        {
            this.StartMEDialog(MEDialog.CREATE_NEW_GRAPH);
        }

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
            GraphViewModel gvm = (GraphViewModel)treeView1.SelectedItem;
            MEDialog dialog = new MEDialog(DialogType, gvm.getGraph());
            Thread _thread = new Thread(new ThreadStart(dialog.Run));
            _thread.Start();
        }


        private void ButtonStartMetaEdit_Click(object sender, RoutedEventArgs e)
        {
            bool runUpdate = false;
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

        private void treeView1_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            SetToolBarButtonsEnabled();
        }

        private void TreeViewItem_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ButtonOpen_Click(sender, e);
            e.Handled = true;
        }
    }
}