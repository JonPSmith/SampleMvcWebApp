#region licence
// The MIT License (MIT)
// 
// Filename: SeedUserLoader.cs
// Date Created: 2014/08/22
// 
// Copyright (c) 2014 Jon Smith (www.selectiveanalytics.com & www.thereformedprogrammer.net)
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
#endregion
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