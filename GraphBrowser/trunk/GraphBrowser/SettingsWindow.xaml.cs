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
using System.Windows.Forms;
using System.IO;

namespace MetaCase.GraphBrowser
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        private string managerAbPath;

        public SettingsWindow()
        {
            InitializeComponent();
            this.LoadSettings();

        }

        /// <summary>
        /// Loads parameters from mer file to textboxes
        /// </summary>
        private void LoadSettings()
        {
            Settings s = Settings.GetSettings();
            this.ProgramTextBox.Text = s.ProgramPath;
            this.WorkingDirTextBox.Text = s.WorkingDir;
            this.DatabaseTextBox.Text = s.Database;
            this.UsernameTextBox.Text = s.Username;
            this.PasswordTextBox.Password = s.Password;
            string projects = "";
            foreach (string project in s.Projects)
            {
                projects += project + ";";
            }
            this.ProjectsTextBox.Text = projects.Substring(0, projects.Length - 1);
            this.PortTextBox.Text = s.Port.ToString();
        }

        private void SaveSettings()
        {
            Settings s = Settings.GetSettings();
            s.ProgramPath = this.ProgramTextBox.Text;
            s.WorkingDir = this.WorkingDirTextBox.Text;
            s.Database = this.DatabaseTextBox.Text;
            s.Username = this.UsernameTextBox.Text;
            s.Password = this.PasswordTextBox.Password;
            s.Projects = this.ProjectsTextBox.Text.Split(new Char[] { ';' });
            int port;
            int.TryParse(this.PortTextBox.Text, out port);
            s.Port = port;
            s.SaveSettings();
        }

        public bool VerifyAllFields()
        {
            bool allOk = true;
            if (!VerifyField(new ProgramPathVerifier(), ProgramTextBox.Text, ref ProgramVerifierImage)) allOk = false;
            if (!VerifyField(new WorkingDirVerifier(), WorkingDirTextBox.Text, ref WorkingDirVerifierImage)) allOk = false;
            if (!VerifyField(new DatabaseVerifier(WorkingDirTextBox.Text), DatabaseTextBox.Text, ref DatabaseVerifierImage)) allOk = false;
            if (!VerifyField(new UsernameVerifier(this.GetManagerAbPath(), WorkingDirTextBox.Text, DatabaseTextBox.Text), UsernameTextBox.Text, ref UsernameVerifierImage)) allOk = false;
            if (!VerifyField(new PasswordVerifier(this.GetManagerAbPath(), UsernameTextBox.Text), PasswordTextBox.Password, ref PasswordVerifierImage)) allOk = false;
            if (!VerifyField(new ProjectsVerifier(this.GetManagerAbPath(), DatabaseTextBox.Text, WorkingDirTextBox.Text), ProjectsTextBox.Text, ref ProjectsVerifierImage)) allOk = false;
            if (!VerifyField(new PortVerifier(), PortTextBox.Text, ref PortVerifierImage)) allOk = false;
            // if ( !verifyField(new HostnameVerifier(), hostnameField, hostnameIconLabel) ) allOk = false;
            return allOk;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!VerifyAllFields())
            {
                if (DialogProvider.ShowYesNoMessageDialog("Are you sure you want to save the settings?",
                        "Confirm Save Settings"))
                {
                    SaveSettings();
                }
            }
            else SaveSettings();
            this.Close();
        }

        /// <summary>
        /// Closes the window when Cancel button is pressed.
        /// </summary>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ProgramSearchButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            DialogResult result = dialog.ShowDialog();
            dialog.DefaultExt = "exe";
            if (result.ToString() == "OK") // Test result.
            {
                ProgramTextBox.Text = dialog.FileName;
            }
        }

        private void WorkingDirSearchButton_Click(object sender, RoutedEventArgs e)
        {
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                dialog.Description = "Select a folder";
                DialogResult result = dialog.ShowDialog();
                if (result.ToString() == "OK")
                {
                    WorkingDirTextBox.Text = dialog.SelectedPath;
                }
            }
        }

        private bool VerifyField(Verifier verifier, string input, ref Image iconImage)
        {
            int verified = verifier.verify(input);
            switch (verified)
            {
                case (1):
                    iconImage.Source = new BitmapImage(new Uri("pack://application:,,,/GraphBrowser;component/Resources/ok_icon.png")); 
                    return true;
                case (0):
                    iconImage.Source = new BitmapImage(new Uri("pack://application:,,,/GraphBrowser;component/Resources/question_icon.png"));
                    return false;
                case (-1):
                    iconImage.Source = new BitmapImage(new Uri("pack://application:,,,/GraphBrowser;component/Resources/error_icon.png"));
                    return false;
            }
            return (verified > 0);
        }

        public string GetManagerAbPath()
        {
            SetManagerAbPath();
            return this.managerAbPath;
        }

        public void SetManagerAbPath()
        {
            string path = WorkingDirTextBox.Text + "\\" + DatabaseTextBox.Text + "\\" + "manager.ab";
            if (File.Exists(path))
            {
                SetManagerAbPath(path);
            }
            else
            {
                SetManagerAbPath("");
            }
        }

        private void SetManagerAbPath(string managerAbPath)
        {
            this.managerAbPath = managerAbPath;
        }

        private void ProgramTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            VerifyField(new ProgramPathVerifier(), ProgramTextBox.Text, ref ProgramVerifierImage);
        }

        private void WorkingDirTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            VerifyField(new WorkingDirVerifier(), WorkingDirTextBox.Text, ref WorkingDirVerifierImage);
            VerifyField(new DatabaseVerifier(WorkingDirTextBox.Text), DatabaseTextBox.Text, ref DatabaseVerifierImage);
            VerifyField(new UsernameVerifier(this.GetManagerAbPath(), WorkingDirTextBox.Text, DatabaseTextBox.Text), UsernameTextBox.Text, ref UsernameVerifierImage);
            VerifyField(new PasswordVerifier(this.GetManagerAbPath(), UsernameTextBox.Text), PasswordTextBox.Password, ref PasswordVerifierImage);
            VerifyField(new ProjectsVerifier(this.GetManagerAbPath(), DatabaseTextBox.Text, WorkingDirTextBox.Text), ProjectsTextBox.Text, ref ProjectsVerifierImage);
        }

        private void DatabaseTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            VerifyField(new DatabaseVerifier(WorkingDirTextBox.Text), DatabaseTextBox.Text, ref DatabaseVerifierImage);
            VerifyField(new UsernameVerifier(this.GetManagerAbPath(), WorkingDirTextBox.Text, DatabaseTextBox.Text), UsernameTextBox.Text, ref UsernameVerifierImage);
            VerifyField(new PasswordVerifier(this.GetManagerAbPath(), UsernameTextBox.Text), PasswordTextBox.Password, ref PasswordVerifierImage);
            VerifyField(new ProjectsVerifier(this.GetManagerAbPath(), DatabaseTextBox.Text, WorkingDirTextBox.Text), ProjectsTextBox.Text, ref ProjectsVerifierImage);
        }

        private void UsernameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            VerifyField(new UsernameVerifier(this.GetManagerAbPath(), WorkingDirTextBox.Text, DatabaseTextBox.Text), UsernameTextBox.Text, ref UsernameVerifierImage);
            VerifyField(new PasswordVerifier(this.GetManagerAbPath(), UsernameTextBox.Text), PasswordTextBox.Password, ref PasswordVerifierImage);
        }

        private void PasswordTextBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            VerifyField(new UsernameVerifier(this.GetManagerAbPath(), WorkingDirTextBox.Text, DatabaseTextBox.Text), UsernameTextBox.Text, ref UsernameVerifierImage);
            VerifyField(new PasswordVerifier(this.GetManagerAbPath(), UsernameTextBox.Text), PasswordTextBox.Password, ref PasswordVerifierImage);
        }

        private void ProjectsTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            VerifyField(new ProjectsVerifier(this.GetManagerAbPath(), DatabaseTextBox.Text, WorkingDirTextBox.Text), ProjectsTextBox.Text, ref ProjectsVerifierImage);
        }

        private void PortTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            VerifyField(new PortVerifier(), PortTextBox.Text, ref PortVerifierImage);
        }

        private void ProjectsSearchButton_Click(object sender, RoutedEventArgs e)
        {
            SelectionWindow w = new SelectionWindow(new List<String>(GraphHandler.ReadFromManagerAb(managerAbPath, "areas")),"Select project(s) to open", true, true);
            w.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            w.ShowDialog();
            String projects = "";
            foreach (string project in w.SelectedItems)
            {
                projects += project + ";";
            }
            if (projects != "") ProjectsTextBox.Text = projects.Substring(0, projects.Length - 1);
        }
    }
}
