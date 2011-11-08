﻿using System;
using System.IO;
using System.Collections;

namespace MetaCase.GraphBrowser
{
    public class IniParser
    {
        private Hashtable values = new Hashtable();
        private String iniFilePath;
        public String key { get; set; }

        /// <summary>
        /// Opens the INI file at the given path and enumerates the values in the IniParser.
        /// </summary>
        /// <param name="iniPath">Full path to INI file.</param>
        public IniParser(String iniPath)
        {
            TextReader iniFile = null;
            String strLine = null;
            String[] keyPair = null;

            iniFilePath = iniPath;

            if (File.Exists(iniPath))
            {
                try
                {
                    iniFile = new StreamReader(iniPath);
                    strLine = iniFile.ReadLine();
                    while (strLine != null)
                    {
                        strLine = strLine.Trim();
                        if (strLine != "" && !strLine.StartsWith("#"))
                        {
                            keyPair = strLine.Split(new char[] { '=' }, 2);
                            values.Add(keyPair[0], keyPair[1]);
                            
                        }
                        strLine = iniFile.ReadLine();
                    }
                }
                finally
                {
                    if (iniFile != null)
                        iniFile.Close();
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
        public String GetSetting(String settingName)
        {
            return (String)values[settingName];
        }

        /// <summary>
        /// Adds or replaces a setting to the table to be saved.
        /// </summary>
        /// <param name="settingName">Key name to add.</param>
        /// <param name="settingValue">Value of key.</param>
        public void AddSetting(String settingName, String settingValue)
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
        public void AddSetting(String settingName)
        {
            AddSetting(settingName, null);
        }

        /// <summary>
        /// Remove a setting.
        /// </summary>
        /// <param name="sectionName">Section to add under.</param>
        /// <param name="settingName">Key name to add.</param>
        public void DeleteSetting(String settingName)
        {
            if (values.ContainsKey(settingName))
                values.Remove(settingName);
        }

        /// <summary>
        /// Save settings to new file.
        /// </summary>
        /// <param name="newFilePath">New file path.</param>
        public void SaveSettings(String newFilePath)
        {
            String tmpValue = "";
            String strToSave = "";
            foreach (String key in values.Keys)
            {
                tmpValue = (String)values[key];

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