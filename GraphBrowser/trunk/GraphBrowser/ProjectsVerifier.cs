using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetaCase.GraphBrowser
{
    class ProjectsVerifier : Verifier
    {
        String managerAbPath;
        String database;
        String workDir;

        public ProjectsVerifier(String managerAb, String Database, String WorkingDirPath)
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
            String[] projects = input.Split(new Char[] {';'});
            String[] allProjects = GraphHandler.ReadFromManagerAb(managerAbPath, "areas");
            List<String> projectsList = new List<String>(projects);
            List<String> allProjectsList = new List<String>(allProjects);
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
