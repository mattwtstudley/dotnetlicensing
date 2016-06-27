using System;
using DotNetLicense;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DotNetLicense.UnitTests
{
    [TestClass]
    public class LicenseManagerTests
    {
        private string _classTestDirName;

        [TestInitialize]
        public void Setup()
        {
            _classTestDirName = "dotnetlicensingworkdir";
            Directory.CreateDirectory(_classTestDirName);
        }

        [TestCleanup]
        public void Cleanup()
        {
            Directory.Delete(this._classTestDirName, true);
        }

        [TestMethod]
        public void CreateNewKeyPair()
        {
            LicenseManager manager = new LicenseManager();
            manager.CreateKeyPairs(_classTestDirName, "testNewPair");

            string expectedPublicKeyPath = string.Format("{0}\\testNewPair_public.key",_classTestDirName);
            string expectedPrivateKeyPath = string.Format("{0}\\testNewPair_private.key",_classTestDirName);
            Assert.IsTrue(File.Exists(expectedPublicKeyPath));
            Assert.IsTrue(File.Exists(expectedPrivateKeyPath));   
        }

        [TestMethod]
        public void LoadPrivateKeyFromString()
        {
            LicenseManager manager = new LicenseManager();
            manager.LoadPrivateKeyFromString(TestKeys.PrivateKey);
            Assert.IsNotNull(manager.PrivateKey);
        }

        [TestMethod]
        public void LoadPublicKeyFromString()
        {
            LicenseManager manager = new LicenseManager();
            manager.LoadPublicKeyFromString(TestKeys.PublicKey);
            Assert.IsNotNull(manager.PublicKey);
        }

        [TestMethod]
        public void CreateNewLicense()
        {
            LicenseManager manager = new LicenseManager();
            manager.LoadPrivateKeyFromString(TestKeys.PrivateKey);
            string expectedFilePath = string.Format("{0}\\testLicense.lic", _classTestDirName);
            manager.SignAndSaveNewLicense(TestLicenses.GetTestNewUnsignedLicense(), expectedFilePath);

            Assert.IsTrue(File.Exists(expectedFilePath));
        }

        [TestMethod]
        public void LoadSignedLicense()
        {
            LicenseManager manager = new LicenseManager();
            manager.LoadPublicKeyFromString(TestKeys.PublicKey);
            License testLicense = manager.LoadLicenseFromString(TestLicenses.GetTestSignedLicenseString());

            Assert.IsNotNull(testLicense);
        }

        [TestMethod]
        [ExpectedException(typeof(LicenseVerificationException))]
        public void LoadUnSignedLicense()
        {
            LicenseManager manager = new LicenseManager();
            manager.LoadPublicKeyFromString(TestKeys.PublicKey);
            License testLicense = manager.LoadLicenseFromString(TestLicenses.GetTestUnsignedLicenseString());
        }

        [TestMethod]
        [ExpectedException(typeof(LicenseVerificationException))]
        public void LoadAlteredLicense()
        {
            LicenseManager manager = new LicenseManager();
            manager.LoadPublicKeyFromString(TestKeys.PublicKey);
            License testLicense = manager.LoadLicenseFromString(TestLicenses.GetTestAlteredLicenseString());
        }

        [TestMethod]
        [ExpectedException(typeof(LicenseVerificationException))]
        public void LoadInvalidLicense()
        {
            LicenseManager manager = new LicenseManager();
            manager.LoadPublicKeyFromString(TestKeys.PublicKey);
            License testLicense = manager.LoadLicenseFromString(TestLicenses.GetTestInvalidLicenseString());
        }

        [TestMethod]
        [ExpectedException(typeof(LicenseVerificationException))]
        public void LoadNonXml()
        {
            LicenseManager manager = new LicenseManager();
            manager.LoadPublicKeyFromString(TestKeys.PublicKey);
            License testLicense = manager.LoadLicenseFromString(TestLicenses.GetNonXml());
        }

        [TestMethod]
        public void ReadLicenseAttributes()
        {
            LicenseManager manager = new LicenseManager();
            manager.LoadPublicKeyFromString(TestKeys.PublicKey);
            License testLicense = manager.LoadLicenseFromString(TestLicenses.GetTestSignedLicenseString());

            string company = testLicense.GetAttribute("Company");
            string licensedOn = testLicense.GetAttribute("LicensedOn");

            Assert.AreEqual("Test Co.", company);
            Assert.AreEqual("6/24/2016", licensedOn);
        }


    }
}
