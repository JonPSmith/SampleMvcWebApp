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

        public bool DataFileExists { get; private set; }

        public SeedUsersLoader(string filepath)
        {
            _filepath = filepath;
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
                DatabaseLogin = userXml.Element("DatabaseLogin").Value,
                DatabasePassword = userXml.Element("DatabasePassword").Value,
            });
        }

        /// <summary>
        /// This sets up the Unauthenticated sql user, which also enables SqlSecurity
        /// </summary>
        /// <param name="xml"></param>
        private static void DecodeUnauthenticatedUser(XElement xml)
        {
            SqlSecure.SetupUnauthenticatedDatabaseUser(xml.Element("DatabaseLogin").Value, xml.Element("DatabasePassword").Value);
        }

    }
}