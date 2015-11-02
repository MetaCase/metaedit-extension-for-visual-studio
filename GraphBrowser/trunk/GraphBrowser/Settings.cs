using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;
using System.Resources;
using Microsoft.VisualStudio.Shell;
using Microsoft.Win32;
using System.Reflection;
using EnvDTE;

namespace MetaCase.GraphBrowser
{
    public class Settings
    {
        /// <summary>
        /// 
        /// </summary>
        public string ProgramPath   { get; set; }
        public string WorkingDir    { get; set; }
        public string Database      { get; set; }
        public string Username      { get; set; }
        public string Password      { get; set; }
        public string[] Projects    { get; set; }
        public string Host          { get; set; }
        public int Port             { get; set; }
        public bool Logging         { get; set; }
        public MEVersion Version    { get; set; }
        public bool Initialized     { get; set; }
        private static List<KeyValuePair<string, string>> UnknownIniKeys = new List<KeyValuePair<string, string>>();
        public static Settings singleton;
        
        public Settings()
        {
            if (!CheckIfMerExists())
            {
                this.CalculateValues();
                this.Initialized = true;
            }
            else
            {
                this.ReadFromConfigFile();
                this.Initialized = true;
            }
        }

        public static Settings GetSettings()
        {
            if (singleton == null) singleton = new Settings();
            return singleton;
        }

        public void SaveSettings()
        {
            this.WriteToConfigFile();
        }

        public string merFilePath()
        {
            return Importer.defaultProjectPath() + "default.mer";
        }

        /// <summary>
        /// Checck ig mer file exists in file system.
        /// </summary>
        /// <returns>True if mer file exists</returns>
        public bool CheckIfMerExists()
        {
            return File.Exists(this.merFilePath());
        }

        private MEVersion ParseVersion(string path)
        {
            string versionString = path.Substring(path.IndexOf("MetaEdit+", 0) + 10);
            char[] splitChars = {'.', ' ', '\\'};
            string[] tokens = versionString.Split(splitChars);
            return new MEVersion(versionString = tokens[0] + "." + tokens[1]);
        }

        /// <summary>
        /// Reads settings from mer file.
        /// </summary>
        private void ReadFromConfigFile()
        {
            try
            {
                IniParser reader = new IniParser(this.merFilePath());
                this.ProgramPath = reader.GetSetting("programPath");
                this.WorkingDir  = reader.GetSetting("workingDir");
                this.Database    = reader.GetSetting("database");
                this.Username    = reader.GetSetting("username");
                this.Password    = reader.GetSetting("password");
                this.Projects    = reader.GetSetting("projects").Split(new Char[] { ';' });
                this.Host        = reader.GetSetting("hostname");
                int tempPort;
                int.TryParse(reader.GetSetting("port"), out tempPort);
                this.Port        = tempPort;
                this.Logging     = reader.GetSetting("logging").Equals("true");
                this.Version     = this.ParseVersion(this.ProgramPath);
            }
            catch (Exception ex)
            {
                DialogProvider.ShowMessageDialog("Error reading .mer file. " + ex.Message, "Error reading .mer file");
            }
        }

        /// <summary>
        /// Writes settings to mer file.
        /// </summary>
        private void WriteToConfigFile()
        {
            try
            {
                IniParser writer = new IniParser(this.merFilePath());

                writer.AddSetting("programPath", this.ProgramPath);
                writer.AddSetting("workingDir", this.WorkingDir);
                writer.AddSetting("database", this.Database);
                writer.AddSetting("username", this.Username);
                writer.AddSetting("password", this.Password);
                string projects = "";
                foreach (string s in this.Projects)
                {
                    projects += ";" + s;
                }
                // Delete the first character which is the delimiter (';')
                projects = projects.Substring(1, projects.Length - 1);
                writer.AddSetting("projects", projects);
                writer.AddSetting("hostname", this.Host);
                writer.AddSetting("port", this.Port.ToString());
                writer.AddSetting("logging", this.Logging.ToString());
                writer.SaveSettings();

                this.Version = this.ParseVersion(this.ProgramPath);
            }
            catch (Exception ex)
            {
                DialogProvider.ShowMessageDialog("Error writing .mer file. " + ex.Message, "Error writing .mer file");
            }
        }

        private string ComposeProgramPath(MEVersion version, string programFilesPath, string pathAddendum, string exeAddendum)
        {
            return programFilesPath + "\\MetaEdit+ " + version.VersionString() + pathAddendum + "\\mep" + version.ShortVersionString() + exeAddendum + ".exe";
        }

        private bool checkExe(MEVersion version, string programFilesPath, string pathAddendum, string exeAddendum)
        {
            string filePath = this.ComposeProgramPath(version, programFilesPath, pathAddendum, exeAddendum);
            return File.Exists(filePath);
        }

       	private void CalculateValues() 
        {
		    this.Database = "demo";
		    this.Username = "user";
		    this.Password = "user";
		    this.Projects = new string [] { "Digital Watch" };
		    this.Port = 6390;
		    this.Host = "localhost";
		    this.Logging = false;
            this.Version = new MEVersion("0.0");

            string programFilesPath = "";
		
		    // Search for Program File (x86) folder from env. variable.
            IDictionary variables = System.Environment.GetEnvironmentVariables();  
            if (variables.Contains("ProgramFiles(x86)")) programFilesPath = (string)variables["ProgramFiles(x86)"];
            else programFilesPath = (string)variables["ProgramFiles"];

            string[] programFilesDirectories = Directory.GetDirectories(programFilesPath);
            bool isEval = false;
            bool isClient = false;
            string pathAddendum = "";
            string exeAddendum = "";

            foreach (string dir in programFilesDirectories)
            {
                if(dir.Contains("MetaEdit+") && !dir.Contains("Server"))
                {
                    MEVersion version = this.ParseVersion(dir);
                    isEval = dir.Contains("Evaluation");
                    isClient = dir.Contains("Client");
                    if (version.IsEqualWith(Version))
                    {
                        if (pathAddendum != "" && !isEval && !isClient && checkExe(version, programFilesPath, "", ""))
                        {
                            pathAddendum = "";
                            exeAddendum = "";
                        }
                        if (pathAddendum == " Client" && isEval  && checkExe(version, programFilesPath, " Evaluation", "eval"))
                        {
                            pathAddendum = " Evaluation";
                            exeAddendum = "eval";
                        }
                    }
                    if (version.IsGreaterThan(this.Version))
                    { 
                        string tempPathAddendum = "";
                        string tempExeAddendum = "";
                        if (isEval)
                        {
                            tempPathAddendum = " Evaluation";
                            tempExeAddendum = "eval";
                        }
                        if (isClient)
                        {
                            tempPathAddendum = " Client";
                            tempExeAddendum = "m";
                        }
                        if(checkExe(version, programFilesPath, tempPathAddendum, tempExeAddendum))
                        {
                            this.Version = version;
                            pathAddendum = tempPathAddendum;
                            exeAddendum = tempExeAddendum;
                        }
                    }
                }
            }

            this.WorkingDir = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal) + "\\MetaEdit+ " + this.Version.VersionString();
            this.ProgramPath = this.ComposeProgramPath(this.Version, programFilesPath, pathAddendum, exeAddendum);
        }
    }
}
