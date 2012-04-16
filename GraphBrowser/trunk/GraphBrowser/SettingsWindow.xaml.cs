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

        ///<summary>
        ///Reads usernames and passwords or projects from manager.ab file depending on the section parameter.
        ///</summary>
        ///<param name="path">path to manager.ab file.</param>
        ///<param name="section">
        ///section if "areas" reads the project names. If "users" reads usernames and passwords and returns 
        ///them as single string separated with ';'. (eg. "root;root")
        ///</param>
        ///<returns>
        ///Array containing the strings.
        ///</returns>
        public static string[] ReadFromManagerAb(string path, string section)
        {
            List<string> list = new List<string>();

            string line;
            // if path is null or does not exist return.
            if (!File.Exists(path.ToString())) return list.ToArray();
            // Read the file and display it line by line.
            using (StreamReader reader = new StreamReader(path.ToString()))
            {
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.Contains("[" + section + "]"))
                    {
                        while (!(line = reader.ReadLine()).StartsWith("["))
                        {
                            if (line.Length > 1)
                            {
                                switch (section)
                                {
                                    case "areas":
                                        list.Add(ParseProjectFromLine(line));
                                        break;
                                    case "users":
                                        list.Add(ParseNameAndPasswordFromLine(line));
                                        break;
                                }
                            }
                        }
                    }
                }
            }
            list.RemoveAll(item => item == null);
            return list.ToArray();
        }

        ///<summary>
        ///Parses project name from manager.ab file line.
        ///</summary>
        ///<param name="line">line to read from manager.ab file</param>
        ///<returns>Project name</returns>
        private static string ParseProjectFromLine(string line)
        {
            string[] inValidProjects = { "Administration-Common", "Administration-System" };
            string project = line.Split(new Char[] { ';' })[1];
            for (int i = 0; i < inValidProjects.Length; i++)
            {
                if (project.Equals(inValidProjects[i])) return null;
            }
            return project;
        }

        ///<summary>
        ///Parses name and password from manager.ab file line
        ///</summary>
        ///<param name="line">line to read from manager.ab file [users] section.</param>
        ///<returns>name and password in string.</returns>
        private static string ParseNameAndPasswordFromLine(string line)
        {
            string[] splitted = line.Split(new Char[] { ';' });
            return splitted[1] + ";" + splitted[2];
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
            SelectionWindow w = new SelectionWindow(new List<string>(ReadFromManagerAb(managerAbPath, "areas")),"Select project(s) to open", true, true);
            w.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            w.ShowDialog();
            string projects = "";
            foreach (string project in w.SelectedItems)
            {
                projects += project + ";";
            }
            if (projects != "") ProjectsTextBox.Text = projects.Substring(0, projects.Length - 1);
        }
    }
}
