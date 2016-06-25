using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetLicense.UnitTests
{
    internal static class TestLicenses
    {
        internal static License GetTestUnsignedLicense()
        {
            License license = new License();
            license.AddOrChangeAttribute("Company", "Test Co.");
            license.AddOrChangeAttribute("LicensedOn", DateTime.Now.ToShortDateString());

            return license;
        }

        internal static string GetTestSignedLicenseString()
        {
            string licenseText = File.ReadAllText("testSignedLicense.lic");
            return licenseText;
        }
    }
}
