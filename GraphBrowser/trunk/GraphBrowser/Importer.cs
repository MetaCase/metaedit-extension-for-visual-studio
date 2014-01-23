using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Build.BuildEngine;
using Microsoft.VisualStudio.Shell;
using EnvDTE;
using System.IO;

namespace MetaCase.GraphBrowser
{
    class Importer
    {
        public static DTE getDTE()
        {
            return Package.GetGlobalService(typeof(DTE)) as DTE;
        }

        public static string defaultProjectPath()
        {
            EnvDTE.DTE dte = getDTE();
            return (string)(dte.get_Properties("Environment", "ProjectsAndSolution").Item("ProjectsLocation").Value) + "\\";
        }
        
        /// <summary>
        /// Imports solution, builds, runs and opens it. 
        /// </summary>
        public static void ImportProject(string applicationName)
        {
            EnvDTE.DTE dte = getDTE();
            string defPrjPath = defaultProjectPath();
            string slnPath = defPrjPath + applicationName + "\\" + applicationName + ".sln";
            string prjPath = defPrjPath + applicationName + "\\" + applicationName + "\\" + applicationName + ".csproj";

            Engine engine = new Engine();

            try
            {
                // Build a project file
                bool success = engine.BuildProjectFile(prjPath);

                // Run if build succeeded
                if (success)
                {
                    string exe = defPrjPath + applicationName + "\\" + applicationName + "\\bin\\Debug\\" + applicationName + ".exe";
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
        /// <param name="path"></param>
        /// <param name="generatorName"></param>
        public static void WritePluginIniFile(string path, string generatorName)
        {
            IniParser ip = new IniParser(path + "//plugin.ini");
            ip.FlushValues();
            ip.AddSetting("IDE", "visualstudio");
            // The generator name will be the name of the generated project.
            ip.AddSetting("workspace", defaultProjectPath() + generatorName);
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
