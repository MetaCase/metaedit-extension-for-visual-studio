﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetaCase.GraphBrowser
{
    class UsernameVerifier : Verifier
    {
        string managerAbPath;
        string workDir;
        string database;

        public UsernameVerifier(string path, string workingDir, string db)
        {
            this.managerAbPath = path;
            this.workDir = workingDir;
            this.database = db;
        }

        public int verify(string input)
        {
            DatabaseVerifier v = new DatabaseVerifier(this.workDir);
            if (v.verify(this.database) <= 0) return 0;
            string [] users = SettingsWindow.ReadFromManagerAb(this.managerAbPath, "users");
			foreach (string user in users) {
                if (input.Equals(user.Split(new Char[] { ';' })[0])) return 1;
			}
			return -1;
        }
    }
}
