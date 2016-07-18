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
        /// <summary>
        /// Gets a new, never saved, never signed licenses. 
        /// </summary>
        /// <returns></returns>
        internal static License GetTestNewUnsignedLicense()
        {
            License license = new License();
            license.AddOrChangeAttribute("Company", "Test Co.");
            license.AddOrChangeAttribute("LicensedOn", DateTime.Now.ToShortDateString());

            return license;
        }

        /// <summary>
        /// A valid, signed license that has been saved to disk. 
        /// </summary>
        /// <returns></returns>
        internal static string GetTestSignedLicenseString()
        {
            string licenseText = File.ReadAllText("testSignedLicense.lic");
            return licenseText;
        }

        /// <summary>
        /// A saved license that has had it's signature removed. 
        /// </summary>
        /// <returns></returns>
        internal static string GetTestUnsignedLicenseString()
        {
            string licenseText = File.ReadAllText("testUnSignedLicense.lic");
            return licenseText;
        }

        /// <summary>
        /// A saved license that was then altered (date changed to 2017)
        /// </summary>
        /// <returns></returns>
        internal static string GetTestAlteredLicenseString()
        {
            string licenseText = File.ReadAllText("testAlteredLicense.lic");
            return licenseText;
        }

        /// <summary>
        /// Gets an invalid license that is just junk XML and not actually a license at all. 
        /// </summary>
        /// <returns></returns>
        internal static string GetTestInvalidLicenseString()
        {
            string licenseText = File.ReadAllText("testInvalidLicense.lic");
            return licenseText;
        }

        /// <summary>
        /// Gets an invalid string that is not even xml. 
        /// </summary>
        /// <returns></returns>
        internal static string GetNonXml()
        {
            return @"I've seen things you people wouldn't believe. Attack ships on fire off the shoulder of Orion.
                    I've watched c-beams glitter in the dark near the Tannhäuser Gate. All those ... moments will be lost in time,
                    like tears...in rain. ";
        }
    }
}
