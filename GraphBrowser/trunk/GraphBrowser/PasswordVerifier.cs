using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetaCase.GraphBrowser
{
    class PasswordVerifier : Verifier
    {
        String username;
        String managerAbPath;

        public PasswordVerifier(String managerAb, String username)
        {
            this.username = username;
            this.managerAbPath = managerAb;
        }

        public int verify(string input)
        {
            String sysadmin = "sysadmin;109859928";
			String user = "user;128988713";
			String [] users = GraphHandler.ReadFromManagerAb(managerAbPath, "users");
			if (username.Equals("user") && input.Equals("user")) {
				foreach(String s in users) {
					if ( s.Equals(user)) return 1;
				}
			}
			else if (username.Equals("sysadmin") && input.Equals("sysadmin")) {
				foreach(String s in users) {
					if ( s.Equals(sysadmin)) return 1;
				}
			}
			if ( username.Equals("user") && !input.Equals("user") ) return -1;
			if ( username.Equals("sysadmin") && !input.Equals("sysadmin") ) return -1;
			return 0;
        }
    }
}
