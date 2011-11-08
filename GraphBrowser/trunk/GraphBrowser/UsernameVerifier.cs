using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetaCase.GraphBrowser
{
    class UsernameVerifier : Verifier
    {
        String managerAbPath;
        String workDir;
        String database;

        public UsernameVerifier(String path, String workingDir, String db)
        {
            this.managerAbPath = path;
            this.workDir = workingDir;
            this.database = db;
        }

        public int verify(string input)
        {
            DatabaseVerifier v = new DatabaseVerifier(this.workDir);
            if (v.verify(this.database) <= 0) return 0;
            String [] users = GraphHandler.ReadFromManagerAb(this.managerAbPath, "users");
			foreach (String user in users) {
                if (input.Equals(user.Split(new Char[] { ';' })[0])) return 1;
			}
			return -1;
        }
    }
}
