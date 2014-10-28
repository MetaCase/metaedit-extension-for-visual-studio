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
        public bool is50            { get; set; }
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

                if (this.ProgramPath.Contains("50")) this.is50 = true;
                else this.is50 = false;
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

                if (this.ProgramPath.Contains("50")) this.is50 = true;
                else this.is50 = false;
            }
            catch (Exception ex)
            {
                DialogProvider.ShowMessageDialog("Error writing .mer file. " + ex.Message, "Error writing .mer file");
            }
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

            string protoWorkingDir = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal) + "\\MetaEdit+ ";
		    string tempProgramPath = "";
		
		    // Search for Program File (x86) folder from env. variable.
            IDictionary variables = System.Environment.GetEnvironmentVariables();  
            if (variables.Contains("ProgramFiles(x86)")) tempProgramPath = (string)variables["ProgramFiles(x86)"];
            else tempProgramPath = (string)variables["ProgramFiles"];
	        
	        this.WorkingDir = protoWorkingDir + "5.1";

            this.ProgramPath = tempProgramPath + "\\MetaEdit+ 5.1\\mep51.exe";

            if (!File.Exists(this.ProgramPath))
            {
                // Try with MetaEdit+ 5.1 evaluation version.
                this.ProgramPath = tempProgramPath + "\\MetaEdit+ 5.1 Evaluation\\mep51eval.exe";
            }

            if (!File.Exists(this.ProgramPath))
            {
                // No MetaEdit+ 5.1 found, make the working directory for version 5.0
                this.WorkingDir = protoWorkingDir + "5.0";
                // Try with MetaEdit+ 5.0
                this.ProgramPath = tempProgramPath + "\\MetaEdit+ 5.0\\mep50.exe";
            }

	        if (!File.Exists(this.ProgramPath)) {
	    	    // Try with MetaEdit+ 5.0 evaluation version.
                this.ProgramPath = tempProgramPath + "\\MetaEdit+ 5.0 Evaluation\\mep50eval.exe";
	        }

            if (!File.Exists(this.ProgramPath))
            {
                // No MetaEdit+ 5.0 found, make the working directory for version 4.5
                this.WorkingDir = protoWorkingDir + "4.5";
	    	    // Try with MetaEdit+ 4.5
                this.ProgramPath = tempProgramPath + "\\MetaEdit+ 4.5\\mep45.exe";    	    
	        }
	    
	        // if no mep45.exe found it MUST be the 4.5 evaluation version ;)
            if (!File.Exists(this.ProgramPath))
            {
                this.ProgramPath = tempProgramPath + "\\MetaEdit+ 4.5 Evaluation\\mep45eval.exe";
	        }
	    }
    }
}
