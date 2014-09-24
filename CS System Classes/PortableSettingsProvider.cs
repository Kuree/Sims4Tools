/***************************************************************************
 *  Copyright (C) 2009, 2010 by Peter L Jones                              *
 *  pljones@users.sf.net                                                   *
 *                                                                         *
 *  This file is part of the Sims 3 Package Interface (s3pi)               *
 *                                                                         *
 *  s3pi is free software: you can redistribute it and/or modify           *
 *  it under the terms of the GNU General Public License as published by   *
 *  the Free Software Foundation, either version 3 of the License, or      *
 *  (at your option) any later version.                                    *
 *                                                                         *
 *  s3pi is distributed in the hope that it will be useful,                *
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of         *
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the          *
 *  GNU General Public License for more details.                           *
 *                                                                         *
 *  You should have received a copy of the GNU General Public License      *
 *  along with s3pi.  If not, see <http://www.gnu.org/licenses/>.          *
 ***************************************************************************/
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;

namespace System.Configuration
{
    /// <summary>
    /// Provides persistence for application settings classes without the random folder naming of <see cref="LocalFileSettingsProvider"/>.
    /// </summary>
    public class PortableSettingsProvider : SettingsProvider//, IApplicationSettingsProvider
    {
        #region Template XML
        /* Define some static strings later used in our XML creation */
        
        // XML Root node
        private const string XMLROOT = "configuration";

        // Configuration declaration node
        private const string CONFIGNODE = "configSections";

        // Configuration section group declaration node
        private const string GROUPNODE = "sectionGroup";

        // User section node
        private const string USERNODE = "userSettings";

        // Application Specific Node
        private static string APPNODE = ExecutableName + ".Properties.Settings";

        private static System.Xml.XmlDocument _xmlDocTemplate
        {
            get
            {
                System.Xml.XmlDocument _xmlDoc = new XmlDocument();
                // XML Declaration
                // <?xml version="1.0" encoding="utf-8"?>
                XmlDeclaration dec = _xmlDoc.CreateXmlDeclaration("1.0", "utf-8", null);
                _xmlDoc.AppendChild(dec);

                // Create root node and append to the document
                // <configuration>
                XmlElement rootNode = _xmlDoc.CreateElement(XMLROOT);
                _xmlDoc.AppendChild(rootNode);

                // Create Configuration Sections node and add as the first node under the root
                // <configSections>
                XmlElement configNode = _xmlDoc.CreateElement(CONFIGNODE);
                _xmlDoc.DocumentElement.PrependChild(configNode);

                // Create the user settings section group declaration and append to the config node above
                // <sectionGroup name="userSettings"...>
                XmlElement groupNode = _xmlDoc.CreateElement(GROUPNODE);
                groupNode.SetAttribute("name", USERNODE);
                groupNode.SetAttribute("type", "System.Configuration.UserSettingsGroup");
                configNode.AppendChild(groupNode);

                // Create the Application section declaration and append to the groupNode above
                // <section name="AppName.Properties.Settings"...>
                XmlElement newSection = _xmlDoc.CreateElement("section");
                newSection.SetAttribute("name", APPNODE);
                newSection.SetAttribute("type", "System.Configuration.ClientSettingsSection");
                groupNode.AppendChild(newSection);

                // Create the userSettings node and append to the root node
                // <userSettings>
                XmlElement userNode = _xmlDoc.CreateElement(USERNODE);
                _xmlDoc.DocumentElement.AppendChild(userNode);

                // Create the Application settings node and append to the userNode above
                // <AppName.Properties.Settings>
                XmlElement appNode = _xmlDoc.CreateElement(APPNODE);
                userNode.AppendChild(appNode);

                return _xmlDoc;
            }
        }
        #endregion
        
        /// <summary>
        /// Initializes the provider.
        /// </summary>
        /// <param name="name">The friendly name of the provider.</param>
        /// <param name="config">A collection of the name/value pairs representing the provider-specific attributes specified in the configuration for this provider.</param>
        /// <exception cref="ArgumentNullException">The name of the provider is null.</exception>
        /// <exception cref="ArgumentException">The name of the provider has a length of zero.</exception>
        /// <exception cref="InvalidOperationException">An attempt is made to call System.Configuration.Provider.ProviderBase.Initialize(System.String,System.Collections.Specialized.NameValueCollection) on a provider after the provider has already been initialized.</exception>
        public override void Initialize(string name, NameValueCollection config)
        {
            base.Initialize(this.ApplicationName, config);
        }

        private static Assembly _mainAssembly = null;
        private static Assembly MainAssembly
        {
            get
            {
                if (_mainAssembly == null)
                    _mainAssembly = Assembly.GetEntryAssembly() // This is what we really want
                        ?? Assembly.GetCallingAssembly() // Not ideal
                        ?? Assembly.GetExecutingAssembly() // Any port in a storm
                        ?? typeof(PortableSettingsProvider).Assembly; // Should be same as above
                ;
                return _mainAssembly;
            }
        }

        /// <summary>
        /// The path to the process executable.
        /// </summary>
        public static string ExecutablePath { get { return MainAssembly.Location; } }

        /// <summary>
        /// The name of the process, stripped of path and extension.
        /// </summary>
        public static string ExecutableName { get { return Path.GetFileNameWithoutExtension(ExecutablePath); } }

        /// <summary>
        /// Return the path of the ini file named <paramref name="suffix"/>.
        /// </summary>
        /// <param name="suffix">Name of the ini file.</param>
        /// <returns>The path of the ini file named <paramref name="suffix"/>.</returns>
        public static string GetApplicationIniFile(string suffix) { return Path.Combine(Path.GetDirectoryName(ExecutablePath), ExecutableName + suffix + ".ini"); }

        private static string _ApplicationName = null;
        /// <summary>
        /// Return the executing assembly name without extension.
        /// </summary>
        public override string ApplicationName
        {
            get { if (_ApplicationName == null) _ApplicationName = ExecutableName; return _ApplicationName; }
            set { if (_ApplicationName == null) _ApplicationName = value; }
        }

        static string ApplicationData { get { return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData); } }

        static string _Company = null;
        static string Company
        {
            get
            {
                if (_Company == null)
                {
                    object[] conames = MainAssembly.GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                    _Company = conames.Length == 1 ? ((AssemblyCompanyAttribute)conames[0]).Company : "noCompany";
                }
                return _Company;
            }
        }

        static string _Product = null;
        static string Product
        {
            get
            {
                if (_Product == null)
                {
                    object[] prdnames = MainAssembly.GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                    _Product = prdnames.Length == 1 ? ((AssemblyProductAttribute)prdnames[0]).Product : "noProduct";
                }
                return _Product;
            }
        }

        static string _ProgramApplicationData = null;
        /// <summary>
        /// Get the folder where data for the application product is held.
        /// </summary>
        public static string ProgramApplicationData
        {
            get
            {
                if (_ProgramApplicationData == null)
                    _ProgramApplicationData = Path.Combine(Path.Combine(ApplicationData, Company), Product);
                return _ProgramApplicationData;
            }
        }

        static string _UserConfigurationData = null;
        /// <summary>
        /// Provide the application settings filename.
        /// </summary>
        /// <returns>The application settings filename.</returns>
        public static string UserConfigurationData
        {
            get
            {
                if (_UserConfigurationData == null)
                    _UserConfigurationData = Path.Combine(ProgramApplicationData, ExecutableName + ".user.config");
                return _UserConfigurationData;
            }
        }

        /// <summary>
        /// Retrieve settings from the configuration file.
        /// </summary>
        /// <param name="sContext">Provides contextual information that the provider can use when persisting settings.</param>
        /// <param name="settingsColl">Contains a collection of <see cref="SettingsProperty"/> objects.</param>
        /// <returns>A collection of settings property values that map <see cref="SettingsProperty"/> objects to <see cref="SettingsPropertyValue"/> objects.</returns>
        public override SettingsPropertyValueCollection GetPropertyValues(SettingsContext sContext, SettingsPropertyCollection settingsColl)
        {
            // Create a collection of values to return
            SettingsPropertyValueCollection retValues = new SettingsPropertyValueCollection();

            // Create a temporary SettingsPropertyValue to reuse
            SettingsPropertyValue setVal;

            // Loop through the list of settings that the application has requested and add them
            // to our collection of return values.
            foreach (SettingsProperty sProp in settingsColl)
            {
                setVal = new SettingsPropertyValue(sProp);
                setVal.IsDirty = false;
                setVal.SerializedValue = GetSetting(sProp);
                retValues.Add(setVal);
            }
            return retValues;
        }

        /// <summary>
        /// Save any of the applications settings that have changed (flagged as "dirty").
        /// </summary>
        /// <param name="sContext">Provides contextual information that the provider can use when persisting settings.</param>
        /// <param name="settingsColl">Contains a collection of <see cref="SettingsProperty"/> objects.</param>
        /// <exception cref="System.Xml.XmlException">The operation would not result in a well formed XML document (for example, no document element or duplicate XML declarations).</exception>
        public override void SetPropertyValues(SettingsContext sContext, SettingsPropertyValueCollection settingsColl)
        {
            // Set the values in XML
            foreach (SettingsPropertyValue spVal in settingsColl)
                if (spVal != null && spVal.SerializedValue != null) SetSetting(spVal);

            if (!Directory.Exists(ProgramApplicationData))
                Directory.CreateDirectory(ProgramApplicationData);
            XMLConfig.Save(UserConfigurationData);
        }

        private XmlDocument _XMLConfig = null;
        private XmlDocument XMLConfig
        {
            get
            {
                if (_XMLConfig == null)
                {
                    if (File.Exists(UserConfigurationData))
                    {
                        try
                        {
                            _XMLConfig = new XmlDocument();
                            _XMLConfig.Load(UserConfigurationData);
                        }
                        catch
                        {
                            _XMLConfig = (XmlDocument)_xmlDocTemplate.Clone();
                        }
                    }
                    else
                    {
                        _XMLConfig = (XmlDocument)_xmlDocTemplate.Clone();
                    }
                }
                return _XMLConfig;
            }
        }

        // Retrieve values from the configuration file, or if the setting does not exist in the file, 
        // retrieve the value from the application's default configuration
        private object GetSetting(SettingsProperty setProp)
        {
            object retVal;

            // Search for the specific settings node we are looking for in the configuration file.
            XmlNode SettingNode = XMLConfig.SelectSingleNode("//setting[@name='" + setProp.Name + "']");
            SettingNode = SettingNode == null ? null : SettingNode.FirstChild;

            // If it exists, return the InnerText or InnerXML of its first child node, depending on the setting type.
            if (SettingNode != null)
            {

                switch (setProp.SerializeAs)
                {
                    case SettingsSerializeAs.String:
                        return SettingNode.InnerText;
                    case SettingsSerializeAs.Xml:
                        string xmlData = SettingNode.InnerXml;
                        return @"" + xmlData;
                    case SettingsSerializeAs.Binary:
                    default:
                        throw new NotSupportedException();
                        //break;
                }

            }
            else
            {
                // Check to see if a default value is defined by the application.
                if ((setProp.DefaultValue != null))
                {
                    // If so, return that value, using the same rules for settings stored as Strings and XML as above
                    switch (setProp.SerializeAs)
                    {
                        case SettingsSerializeAs.String:
                            retVal = setProp.DefaultValue.ToString();
                            break;
                        case SettingsSerializeAs.Xml:
                            retVal = setProp.DefaultValue.ToString().Replace(@"<?xml version=""1.0"" encoding=""utf-16""?>", "");
                            break;
                        /*string xmlData = setProp.DefaultValue.ToString();
                        XmlSerializer xs = new XmlSerializer(typeof(string[]));
                        StringCollection sc = new StringCollection();
                        object property = xs.Deserialize(new XmlTextReader(xmlData, XmlNodeType.Element, null));
                        if (setProp.PropertyType == typeof(System.Collections.Specialized.StringCollection))
                        {
                            string[] data = (string[])property;
                            sc.AddRange(data);
                            return sc;
                        }
                        throw new NotSupportedException();**/
                        //retVal = "";
                        //break;
                        case SettingsSerializeAs.Binary:
                        default:
                            throw new NotSupportedException();
                        //retVal = "";
                        //break;
                    }
                }
                else
                {
                    retVal = "";
                }
            }
            return retVal;
        }

        private void SetSetting(SettingsPropertyValue setProp)
        {
            // Search for the specific settings node we are looking for in the configuration file.
            XmlNode SettingNode = XMLConfig.SelectSingleNode("//setting[@name='" + setProp.Name + "']");
            SettingNode = SettingNode == null ? null : SettingNode.FirstChild;

            // If we have a pointer to an actual XML node, update the value stored there
            if (SettingNode != null)
            {

                switch (setProp.Property.SerializeAs)
                {
                    case SettingsSerializeAs.String:
                        SettingNode.InnerText = setProp.SerializedValue.ToString();
                        break;
                    case SettingsSerializeAs.Xml:
                        // Write the object to the config serialized as Xml - we must remove the Xml declaration when writing
                        // the value, otherwise .Net's configuration system complains about the additional declaration.
                        SettingNode.InnerXml = setProp.SerializedValue.ToString().Replace(@"<?xml version=""1.0"" encoding=""utf-16""?>", "");
                        break;
                    case SettingsSerializeAs.Binary:
                    default:
                        throw new NotSupportedException();
                    //break;
                }

            }
            else
            {
                // If the value did not already exist in this settings file, create a new entry for this setting

                // Search for the application settings node (<Appname.Properties.Settings>) and store it.
                XmlNode tmpNode = XMLConfig.SelectSingleNode("//" + APPNODE);

                // Create a new settings node and assign its name as well as how it will be serialized
                XmlElement newSetting = _XMLConfig.CreateElement("setting");

                // Create an element under our named settings node, and assign it the value we are trying to save
                XmlElement valueElement = _XMLConfig.CreateElement("value");
                newSetting.SetAttribute("name", setProp.Name);

                switch (setProp.Property.SerializeAs)
                {
                    case SettingsSerializeAs.String:
                        newSetting.SetAttribute("serializeAs", "String");
                        valueElement.InnerText = setProp.SerializedValue.ToString();
                        break;
                    case SettingsSerializeAs.Xml:
                        newSetting.SetAttribute("serializeAs", "Xml");
                        // Write the object to the config serialized as Xml - we must remove the Xml declaration when writing
                        // the value, otherwise .Net's configuration system complains about the additional declaration
                        valueElement.InnerXml = setProp.SerializedValue.ToString().Replace(@"<?xml version=""1.0"" encoding=""utf-16""?>", "");
                        break;
                    case SettingsSerializeAs.Binary:
                    default:
                        throw new NotSupportedException();
                    //break;
                }

                // Append this node to the application settings node (<Appname.Properties.Settings>)
                tmpNode.AppendChild(newSetting);

                //Append this new element under the setting node we created above
                newSetting.AppendChild(valueElement);
            }
        }
    }
}