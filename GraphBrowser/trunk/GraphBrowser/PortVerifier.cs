using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetaCase.GraphBrowser
{
    class PortVerifier : Verifier
    {
        public int verify(string input)
        {
            int port = -1;
            Int32.TryParse(input, out port);
            if (0 <= port && port <= 1023)
            {
                return 0;
            }
            else if (1024 <= port && port <= 65535)
            {
                return 1;
            }
            else
            {
                return -1;
            }
        }
    }
}
