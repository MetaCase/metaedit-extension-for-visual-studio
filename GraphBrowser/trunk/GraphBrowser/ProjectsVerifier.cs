using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetaCase.GraphBrowser
{
    class ProjectsVerifier : Verifier
    {
        string managerAbPath;
        string database;
        string workDir;

        public ProjectsVerifier(string managerAb, string Database, string WorkingDirPath)
        {
            this.managerAbPath = managerAb;
            this.database = Database;
            this.workDir = WorkingDirPath;
        }

        public int verify(string input)
        {
            DatabaseVerifier v = new DatabaseVerifier(this.workDir);
            if (v.verify(this.database) < 0)
            {
                return 0;
            }
            if (input.Equals("")) return 0;
            string[] projects = input.Split(new Char[] {';'});
            string[] allProjects = SettingsWindow.ReadFromManagerAb(managerAbPath, "areas");
            List<string> projectsList = new List<string>(projects);
            List<string> allProjectsList = new List<string>(allProjects);
            if (projects.Length > 0)
            {
                for (int i = projectsList.Count - 1; i >= 0; i--)
                {
                    if (allProjectsList.Contains(projectsList[i])) projectsList.Remove(projectsList[i]);
                }
            }
            if (projectsList.Count > 0) return -1;
            return 1;
        }
    }
}
