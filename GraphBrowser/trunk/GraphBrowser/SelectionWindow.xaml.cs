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
using System.Windows.Shapes;
using System.Collections;

namespace MetaCase.GraphBrowser
{
    /// <summary>
    /// Interaction logic for ProjectSelectionWindow.xaml
    /// </summary>
    public partial class SelectionWindow : Window
    {
        public List<string> SelectedItems = new List<string>();
        private Button SelectAllButton;

        /// <summary>
        /// Selection window logic. Reads selected items to List when the window is closed.
        /// </summary>
        /// <param name="Items">Items to be shown in list.</param>
        /// <param name="Title">Title for the window.</param>
        /// <param name="CanSelectAll">Value for allowing the "select all" button.</param>
        /// <param name="MultiSelection">Value for allowing multiselection or not.</param>
        public SelectionWindow(List<string> Items, string Title, bool CanSelectAll, bool MultiSelection)
        {
            InitializeComponent();
            this.HeaderText.Text = Title;
            Items.Sort(delegate(string s1, string s2) { return s1.CompareTo(s2); });
            this.ItemsListBox.ItemsSource = Items;
            if (CanSelectAll) //this.SelectAllButton.Visibility = System.Windows.Visibility.Hidden;
            {
                SelectAllButton = new Button();
                SelectAllButton.Click += new RoutedEventHandler(SelectAllButton_Click);
                SelectAllButton.Content = "Select All";
                SelectAllButton.Margin = new Thickness(2);
                SelectAllButton.Width = 65;
                SelectAllButton.Height = 25;
                SelectAllButton.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                ButtonPanel.Children.Insert(0, SelectAllButton);
            }
            if (MultiSelection) this.ItemsListBox.SelectionMode = SelectionMode.Multiple;
        }

        public void SelectAllButton_Click(object sender, RoutedEventArgs e)
        {
            this.ItemsListBox.SelectAll();
            this.ItemsListBox.SelectedIndex = 0;
        }

        private void SelectButton_Click(object sender, RoutedEventArgs e)
        {
            IList selectedItems = this.ItemsListBox.SelectedItems;
            foreach (string Item in selectedItems)
            {
                this.SelectedItems.Add(Item);
            }
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
