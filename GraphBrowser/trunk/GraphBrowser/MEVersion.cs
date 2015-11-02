using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetaCase.GraphBrowser
{
    public class MEVersion
    {
        public string Major { get; set; }
        public string Minor { get; set; }

        public MEVersion(string versionString)
        {
            this.SetVersion(versionString);
        }

        public void SetVersion(string versionString)
        {
            char[] splitChars = {'.'};
            string[] tokens = versionString.Split(splitChars);
            this.Major = tokens[0];
            this.Minor = tokens[1];
        }

        public void SetVersion(float versionNumber)
        {
            this.SetVersion(versionNumber.ToString());
        }

        public string GetMajor()
        {
            return this.Major;
        }

        public string GetMinor()
        {
            return this.Minor;
        }
        
        public string VersionString()
        {
            return this.Major + "." + this.Minor;
        }

        public string ShortVersionString()
        {
            return this.Major + this.Minor;
        }

        public bool IsEqualWith(MEVersion version)
        {
            return (this.Major == version.GetMajor() && this.Minor == version.GetMinor());
        }

        public bool IsGreaterThan(MEVersion version)
        {
            if(Int32.Parse(this.Major) > Int32.Parse(version.GetMajor())) return true;
            if(Int32.Parse(this.Minor) > Int32.Parse(version.GetMinor())) return true;
            return false;
        }

        public bool IsEqualOrGreaterThan(MEVersion version)
        {
            return (this.IsEqualWith(version) || this.IsGreaterThan(version));
        }

        public bool IsEqualOrGreaterThan(string versionString)
        {
            MEVersion tempVersion = new MEVersion(versionString);
            return this.IsEqualOrGreaterThan(tempVersion);
        }
    }
}
