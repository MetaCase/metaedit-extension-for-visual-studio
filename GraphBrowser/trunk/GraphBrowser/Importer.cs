using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Build.BuildEngine;
using EnvDTE;
using System.IO;

namespace MetaCase.GraphBrowser
{
    class Importer
    {
        /// <summary>
        /// Imports solution, opens, builds and runs it. 
        /// </summary>
        public static void ImportProject(string applicationName)
        {
            EnvDTE.DTE dte = (EnvDTE.DTE)System.Runtime.InteropServices.Marshal.GetActiveObject("VisualStudio.DTE.10.0");
            //Get visual studio version e.g. dte.FullName;
            string slnPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Visual Studio 2010\\Projects\\" + applicationName + "\\" + applicationName + ".sln";
            string prjPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Visual Studio 2010\\Projects\\" + applicationName + "\\" + applicationName + "\\" + applicationName + ".csproj";

            Engine engine = new Engine();

            try
            {
                // Build a project file
                bool success = engine.BuildProjectFile(prjPath);


                // Run if build succeeded
                if (success)
                {
                    string exe = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Visual Studio 2010\\Projects\\" + applicationName + "\\" + applicationName
                    + "\\bin\\Debug\\" + applicationName + ".exe";
                    if (File.Exists(exe))
                        System.Diagnostics.Process.Start(exe);
                }

            }
            catch (Exception)
            {

            }
            finally
            {
                // Open the solution in Visual Studio
                dte.Solution.Open(slnPath);
            }
        }

        /// <summary>
        /// Writes ini file for MetaEdit+ generator.
        /// </summary>
        public static void WritePluginIniFile(string path, string generatorName)
        {
            IniParser ip = new IniParser(path + "//plugin.ini");
            ip.FlushValues();
            ip.AddSetting("IDE", "visualstudio");
            ip.AddSetting("workspace", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Visual Studio 2010\\Projects\\" + generatorName);
            // If running the Autobuild generator set "runGenerated" to "true"
            ip.AddSetting("runGenerated", generatorName.Equals("Autobuild") ? "true" : "false");
            ip.SaveSettings();
        }

        /// <summary>
        /// Removes the plugin.ini file.
        /// </summary>
        /// <param name="path">Folder path that contains the file</param>
        public static void RemoveIniFile(string path)
        {
            File.Delete(path + "//plugin.ini");
        }
    }
}
