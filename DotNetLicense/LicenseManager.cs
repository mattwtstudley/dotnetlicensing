using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DotNetLicense
{
    /// <summary>
    /// The license manager class is used to create and load keys for the creation and loading instances of <see cref="License"/>.
    /// </summary>
    public class LicenseManager
    {
        /// <summary>
        /// The public key that is used to verify that a license is valid. You must populate this to open / load licenses. 
        /// </summary>
        public string PublicKey { get; private set; }

        /// <summary>
        /// Private key that is used to create signed licenses. You must popualte this in order to create a new license. 
        /// </summary>
        public string PrivateKey { get; private set; }

        /// <summary>
        /// Loads a private key from the given filepath
        /// </summary>
        /// <param name="filepath"></param>
        public void LoadPrivateKeyFromFile(string filepath)
        {
            PrivateKey = LoadFromDiskAndVerify(filepath);
        }

        /// <summary>
        /// Loads a public key from the given filepath. 
        /// </summary>
        /// <param name="filepath"></param>
        public void LoadPublicKeyFromFile(string filepath)
        {
            PublicKey = LoadFromDiskAndVerify(filepath);
        }

        /// <summary>
        /// Loads a private key from a string. 
        /// </summary>
        /// <param name="keyString">An XML representation of an RSA key string.</param>
        public void LoadPrivateKeyFromString(string keyString)
        {
            RSA encrKey = RSA.Create();
            encrKey.FromXmlString(keyString);

            PrivateKey = keyString;
        }

        /// <summary>
        /// Loads a public key from a string. 
        /// </summary>
        /// <param name="keyString">An XML representation of an RSA key string.</param>
        public void LoadPublicKeyFromString(string keyString)
        {
            RSA encrKey = RSA.Create();
            encrKey.FromXmlString(keyString);

            PublicKey = keyString;
        }

        /// <summary>
        /// Creates a new set of private and public key pairs and saves the files to thr specified directory. 
        /// </summary>
        /// <param name="directory">The directory the keys will be written to. </param>
        /// <param name="keyPairName">The name of the keys, which will have _public or private appended to it.</param>
        public void CreateKeyPairs(string directory, string keyPairName)
        {
            RSACryptoServiceProvider key = new RSACryptoServiceProvider(2048);

            string publicPrivateKeyXML = key.ToXmlString(true);
            string publicOnlyKeyXML = key.ToXmlString(false);

            string privateKeyFileName = string.Format("{0}\\{1}_private.key", directory, keyPairName);
            string publicKeyFileName = string.Format("{0}\\{1}_public.key", directory, keyPairName);

            StringToFile(publicKeyFileName, publicOnlyKeyXML);
            StringToFile(privateKeyFileName, publicPrivateKeyXML);
        }

        /// <summary>
        /// Uses the defined Public Key and saves the license to the given filepath.
        /// </summary>
        /// <param name="license">An instance of a <see cref="License"/> to be saved to disk.</param>
        /// <param name="filepath">The file path where the license will be saved.</param>
        public void SignAndSaveNewLicense(License license, string filepath)
        {
            var textToSave = this.SignAndSaveNewLicense(license);

            try
            {
                File.WriteAllText(filepath, textToSave);
            }
            catch (Exception ex)
            {
                throw new Exception("Could not save license file to disk. See inner exception for details.", ex);
            }
        }

        /// <summary>
        /// Generates a license as an XML string. 
        /// </summary>
        /// <param name="license"></param>
        /// <returns></returns>
        public string SignAndSaveNewLicense(License license)
        {
            if (String.IsNullOrEmpty(PrivateKey)) throw new InvalidOperationException(@"The private key has not been set - can not create a new license.
                                                                                        Set the private key property before calling this method.");
            try
            {
                license.Sign(PrivateKey);
            }
            catch (Exception ex)
            {
                throw new Exception("Could not sign license. See inner exception for details.", ex);
            }

            return license.ToString();
        }

        /// <summary>
        /// Loads a license from disk. Throws an exception if it is not valid. 
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public License LoadLicenseFromDisk(string filepath)
        {
            if (String.IsNullOrEmpty(PublicKey)) throw new InvalidOperationException(@"The public key has not been set - can not verify if a license is valid.
                                                                                        Set the public key property before calling this method.");
            string rawText = "";
            try
            {
                rawText = File.ReadAllText(filepath);
            }
            catch (Exception ex)
            {
                throw new Exception("Could not read license file at {0} from disk. See inner exception for details.", ex);
            }

            License loadedLicense = new License(rawText);

            if (!loadedLicense.IsValid(PublicKey)) throw new LicenseVerificationException(string.Format("The license file at {0} is not valid!", filepath));

            return loadedLicense;
        }

        /// <summary>
        /// Load a license from the given string.
        /// </summary>
        /// <param name="licenseString">The string from which a license class is to be deserialized and instanciated into.</param>
        /// <returns></returns>
        public License LoadLicenseFromString(string licenseString)
        {
            if (String.IsNullOrEmpty(PublicKey)) throw new InvalidOperationException(@"The public key has not been set - can not verify if a license is valid.
                                                                                        Set the public key property before calling this method.");
            License loadedLicense = new License(licenseString);

            if (!loadedLicense.IsValid(PublicKey)) throw new LicenseVerificationException(string.Format("The license file at {0} is not valid!",licenseString));

            return loadedLicense;
        }

        /// <summary>
        /// Writes a string to a file. 
        /// </summary>
        /// <param name="outfile"></param>
        /// <param name="data"></param>
        private void StringToFile(string outfile, string data)
        {
            StreamWriter outStream = System.IO.File.CreateText(outfile);
            outStream.Write(data);
            outStream.Close();
        }

        /// <summary>
        /// Loads a file that is expected to be an XML RSA key. 
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        private string LoadFromDiskAndVerify(string filepath)
        {
            try
            {
                var rawText = File.ReadAllText(filepath);
                RSA encrKey = RSA.Create();
                encrKey.FromXmlString(rawText); //Test parsing the xml to make sure its valid. 

                return rawText;
            }
            catch (Exception ex)
            {
                throw new Exception("Could not load private key from disk. See inner exception for details.", ex);
            }
        }
    }
}
