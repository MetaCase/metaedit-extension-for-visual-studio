using System;
using System.IO;
using System.Collections;

namespace MetaCase.GraphBrowser
{
    public class IniParser
    {
        private Hashtable values = new Hashtable();
        private string iniFilePath;
        public string key { get; set; }

        /// <summary>
        /// Opens the INI file at the given path and enumerates the values in the IniParser.
        /// </summary>
        /// <param name="iniPath">Full path to INI file.</param>
        public IniParser(string iniPath)
        {
            string strLine = null;
            string[] keyPair = new String[2];

            iniFilePath = iniPath;

            if (File.Exists(iniPath))
            {
                using (StreamReader reader = new StreamReader(iniPath))
                {
                    while ((strLine = reader.ReadLine()) != null)
                    {
                        strLine = strLine.Trim();
                        int EqualsIndex = strLine.IndexOf("=");
                        if (!strLine.Equals("") && !strLine.StartsWith("#") && EqualsIndex != -1)
                        {
                            values.Add(strLine.Substring(0, EqualsIndex), strLine.Substring(EqualsIndex+1));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Removes old values from ini file.
        /// </summary>
        public void FlushValues()
        {
            this.values = new Hashtable();
        }

        /// <summary>
        /// Returns the value for the given section, key pair.
        /// </summary>
        /// <param name="settingName">Key name.</param>
        public string GetSetting(string settingName)
        {
            return (string)values[settingName];
        }

        /// <summary>
        /// Adds or replaces a setting to the table to be saved.
        /// </summary>
        /// <param name="settingName">Key name to add.</param>
        /// <param name="settingValue">Value of key.</param>
        public void AddSetting(string settingName, string settingValue)
        {
            if (values.ContainsKey(settingName))
                values.Remove(settingName);

            values.Add(settingName, settingValue);
        }

        /// <summary>
        /// Adds or replaces a setting to the table to be saved with a null value.
        /// </summary>
        /// <param name="sectionName">Section to add under.</param>
        /// <param name="settingName">Key name to add.</param>
        public void AddSetting(string settingName)
        {
            AddSetting(settingName, null);
        }

        /// <summary>
        /// Remove a setting.
        /// </summary>
        /// <param name="sectionName">Section to add under.</param>
        /// <param name="settingName">Key name to add.</param>
        public void DeleteSetting(string settingName)
        {
            if (values.ContainsKey(settingName))
                values.Remove(settingName);
        }

        /// <summary>
        /// Save settings to new file.
        /// </summary>
        /// <param name="newFilePath">New file path.</param>
        public void SaveSettings(string newFilePath)
        {
            string tmpValue = "";
            string strToSave = "";
            foreach (string key in values.Keys)
            {
                tmpValue = (string)values[key];

                if (tmpValue != null)
                    tmpValue = "=" + tmpValue;

                strToSave += (key + tmpValue + "\r\n");
            }
            strToSave += "\r\n";

            TextWriter tw = new StreamWriter(newFilePath);
            tw.Write(strToSave);
            tw.Close();
        }

        /// <summary>
        /// Save settings back to ini file.
        /// </summary>
        public void SaveSettings()
        {
            SaveSettings(iniFilePath);
        }
    }
}
