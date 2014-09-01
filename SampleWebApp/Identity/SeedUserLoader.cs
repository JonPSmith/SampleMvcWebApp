using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace SampleWebApp.Identity
{
    public class SeedUsersLoader
    {

        private readonly string _databaseLoginPrefix;
        private readonly XElement _xmlData;

        public bool DataFileExists { get; private set; }

        public string UnauthenticatedDatabaseLogin { get; private set; }

        public string UnauthenticatedDatabasePassword { get; private set; }

        public SeedUsersLoader(string filepath, string databaseLoginPrefix)
        {
            _databaseLoginPrefix = databaseLoginPrefix;
            DataFileExists = File.Exists(filepath);
            if (!DataFileExists) return;

            _xmlData = XElement.Load(filepath);
            DecodeUnauthenticatedUser(_xmlData.Element("UnauthenticatedUser"));
        }

        public IEnumerable<SeedUserInfo> LoadSeedData()
        {
            //now decode and return
            return _xmlData.Elements("User").Select(userXml => new SeedUserInfo
            {
                DisplayName = userXml.Element("DisplayName").Value,
                Email = userXml.Element("Email").Value,
                OriginalPassword = userXml.Element("Password").Value,
                DatabaseLogin = FormLoginName(userXml.Element("DatabaseLogin").Value),
                DatabasePassword = userXml.Element("DatabasePassword").Value,
            });
        }

        /// <summary>
        /// This sets up the Unauthenticated properties
        /// </summary>
        /// <param name="xml"></param>
        private void DecodeUnauthenticatedUser(XElement xml)
        {
            UnauthenticatedDatabaseLogin = FormLoginName(xml.Element("DatabaseLogin").Value);
            UnauthenticatedDatabasePassword = xml.Element("DatabasePassword").Value;
        }

        private string FormLoginName(string userName)
        {
            return _databaseLoginPrefix + userName;
        }

    }
}