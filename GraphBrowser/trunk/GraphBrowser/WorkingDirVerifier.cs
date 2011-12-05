using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MetaCase.GraphBrowser
{
    class WorkingDirVerifier : Verifier
    {
        public int verify(string input)
        {
            if (input != "" && Directory.Exists(input)) {
				string[] files = Directory.GetFiles(input);
				foreach(string file in files) {
					if (file.Contains("artbase.roo")) return 1;
				}
			}
		    return -1;
        }
    }
}
