using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetaCase.GraphBrowser
{
    class PasswordVerifier : Verifier
    {
        string username;
        string managerAbPath;

        public PasswordVerifier(string managerAb, string username)
        {
            this.username = username;
            this.managerAbPath = managerAb;
        }

        public int verify(string input)
        {
            string sysadmin = "sysadmin;109859928";
			string user = "user;128988713";
			string [] users = GraphHandler.ReadFromManagerAb(managerAbPath, "users");
			if (username.Equals("user") && input.Equals("user")) {
				foreach(string s in users) {
					if ( s.Equals(user)) return 1;
				}
			}
			else if (username.Equals("sysadmin") && input.Equals("sysadmin")) {
				foreach(string s in users) {
					if ( s.Equals(sysadmin)) return 1;
				}
			}
			if ( username.Equals("user") && !input.Equals("user") ) return -1;
			if ( username.Equals("sysadmin") && !input.Equals("sysadmin") ) return -1;
			return 0;
        }
    }
}
