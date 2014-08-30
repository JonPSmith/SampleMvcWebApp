using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using DataLayer.Security;

namespace SampleWebApp.Identity
{
    public class SeedUsersLoader
    {

        private readonly string _filepath;
        private readonly string _databaseLoginPrefix;

        public bool DataFileExists { get; private set; }

        public SeedUsersLoader(string filepath, string databaseLoginPrefix)
        {
            _filepath = filepath;
            _databaseLoginPrefix = databaseLoginPrefix;
            DataFileExists = File.Exists(_filepath);
        }

        public IEnumerable<SeedUserInfo> LoadSeedData()
        {
            var xmlData = XElement.Load(_filepath);

            DecodeUnauthenticatedUser(xmlData.Element("UnauthenticatedUser"));

            //now decode and return
            return xmlData.Elements("User").Select(userXml => new SeedUserInfo
            {
                DisplayName = userXml.Element("DisplayName").Value,
                Email = userXml.Element("Email").Value,
                OriginalPassword = userXml.Element("Password").Value,
                DatabaseLogin = FormLoginName(userXml.Element("DatabaseLogin").Value),
                DatabasePassword = userXml.Element("DatabasePassword").Value,
            });
        }

        /// <summary>
        /// This sets up the Unauthenticated sql user, which also enables SqlSecurity
        /// </summary>
        /// <param name="xml"></param>
        private void DecodeUnauthenticatedUser(XElement xml)
        {
            SqlSecure.SetupUnauthenticatedDatabaseUser(FormLoginName(xml.Element("DatabaseLogin").Value), xml.Element("DatabasePassword").Value);
        }

        private string FormLoginName(string userName)
        {
            return _databaseLoginPrefix + userName;
        }

    }
}