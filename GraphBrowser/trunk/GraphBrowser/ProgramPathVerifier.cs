using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MetaCase.GraphBrowser
{
    class ProgramPathVerifier : Verifier
    {
        public int verify(string input)
        {
            if (File.Exists(input) && input.Contains("mep") && input.Contains(".exe")) return 1;
            return -1;
        }
    }
}
