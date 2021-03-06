﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace Tcdev.Dsm.Install
{
    /// <summary>
    /// Updater detects new versions and handles download
    /// </summary>
    public class Installer
    {
        string _downloadedFile;

        public Installer(string versionFileUrl, string installerUrl)
        {
            _webSiteVersionFileUrl = versionFileUrl;
            _webSiteInstallerUrl = installerUrl;
        }

        string _webSiteVersionFileUrl;
        string _webSiteInstallerUrl;

        public bool NewVersion()
        {
            bool isNew = false;
            WebRequest req = WebRequest.Create(_webSiteVersionFileUrl);
            req.Timeout = 3000;

            try
            {
                string webSiteVersion;
                using (WebResponse reply = req.GetResponse())
                {
                    using (StreamReader reader = new StreamReader(reply.GetResponseStream()))
                    {
                        webSiteVersion = reader.ReadLine();
                    }
                }

                if (webSiteVersion != null)
                {
                    isNew = CompareVersion(webSiteVersion, Assembly.GetExecutingAssembly().GetName().Version);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            return isNew;
        }

        public bool CompareVersion(string webSiteVersion, Version assemblyVersion)
        {
            string[] data = webSiteVersion.Split('.');
            if (data.Length < 2)
                throw new DsmException("WebSite version text file invalid: " + webSiteVersion);

            int newMajor = int.Parse(data[0], CultureInfo.InvariantCulture);
            int newMinor = int.Parse(data[1], CultureInfo.InvariantCulture);
            int newBuild = data.Length > 2 ? int.Parse(data[2], CultureInfo.InvariantCulture) : 0;

            Version latest = new Version(newMajor, newMinor, newBuild);

            return latest > assemblyVersion;
        }

        public void Run()
        {
            if (string.IsNullOrEmpty(_downloadedFile))
                throw new DsmException("No installer downloaded");

            Process process = new Process();
            process.StartInfo = new ProcessStartInfo(
                     "msiexec", " /i " + _downloadedFile + " REINSTALL=ALL REINSTALLMODE=vomus");
            process.Start();
        }

        public void Load()
        {
            WebRequest req = WebRequest.Create(_webSiteInstallerUrl);
            using (WebResponse reply = req.GetResponse())
            {
                using (Stream reader = reply.GetResponseStream())
                {
                    _downloadedFile = Path.Combine(System.IO.Path.GetTempPath(), "DsmInstaller.msi");
                    using (FileStream fs = new FileStream(_downloadedFile, FileMode.Create, FileAccess.Write))
                    {
                        Byte[] buffer = new Byte[32 * 1024];
                        int read = reader.Read(buffer, 0, buffer.Length);

                        while (read > 0)
                        {
                            fs.Write(buffer, 0, read);
                            fs.Flush();
                            read = reader.Read(buffer, 0, buffer.Length);
                        }
                    }
                }
            }
        }
    }
}