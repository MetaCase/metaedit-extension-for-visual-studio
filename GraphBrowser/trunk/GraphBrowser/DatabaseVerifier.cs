using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MetaCase.GraphBrowser
{
    /// <summary>
    /// Verifier class for verifying the database input.
    /// </summary>
    class DatabaseVerifier : Verifier
    {
        String workingDirPath;

        /// <summary>
        /// Constuctor
        /// </summary>
        /// <param name="path">MetaEdit+ working directory path</param>
        public DatabaseVerifier(String path)
        {
			this.workingDirPath = path;
		}

        /// <summary>
        /// Verifier that checks if database can be found from given Working directory
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public int verify(string input)
        {
            WorkingDirVerifier v = new WorkingDirVerifier();
            if (v.verify(this.workingDirPath) < 0) return 0;

            if (Directory.Exists(workingDirPath + "\\" + input))
            {
                string[] files = Directory.GetFiles(workingDirPath + "\\" + input);
				foreach(String file in files) {
					if (file.Contains("manager.ab")) return 1;
				}
			}
			return -1;
		}
    }
}
