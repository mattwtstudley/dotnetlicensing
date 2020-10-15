using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace DotNetLicense
{
    /// <summary>
    /// Represents an instance of a License. You use the <see cref="LicenseManager"/> to create and read these licenses. 
    /// </summary>
    public class License
    {
        private Dictionary<string, string> _licenseAttributes;
        private SignedXml _signature;
        private XmlDocument _xmlLicense;

        #region Constructor
        /// <summary>
        /// Creates a new license. Use this when creating a license from scratch. 
        /// </summary>
        public License()
        {
            _licenseAttributes = new Dictionary<string, string>();
        }

        /// <summary>
        /// Creates a license class from an existing XmlDocument representing a signed xml license. It must have been created using this class. 
        /// </summary>
        /// <param name="licenseContent"></param>
        public License(XmlDocument licenseContent)
        {
            _licenseAttributes = new Dictionary<string, string>();
            InitializeFromXmlFile(licenseContent);
        }

        /// <summary>
        /// Creates a license class from an existing string representing a signed xml license. 
        /// </summary>
        /// <param name="licenseContent"></param>
        public License(string licenseContent)
        {
            _licenseAttributes = new Dictionary<string, string>();
            XmlDocument xDoc = new XmlDocument();
            
            try
            { 
                xDoc.LoadXml(licenseContent); 
            }
            catch (XmlException xmlEx)
            {
                throw new LicenseVerificationException("This license is not valid XML - it may be malformed! See inner exception for details.", xmlEx);
            }

            InitializeFromXmlFile(xDoc);
        }
        #endregion

        #region LicenseAttribute Management
        /// <summary>
        /// Adds or updates a license attribute. 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void AddOrChangeAttribute(string name, string value)
        {
            if (_signature != null) throw new InvalidOperationException("This license has been signed, you may not change it.");

            if (_licenseAttributes.ContainsKey(name))
            {
                _licenseAttributes[name] = value;
            }
            else
            {
                _licenseAttributes.Add(name, value);
            }
        }

        /// <summary>
        /// Gets a license attribute from the license. 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string GetAttribute(string name)
        {
            if (_licenseAttributes.ContainsKey(name))
            {
                return _licenseAttributes[name];
            }
            else
            {
                throw new ArgumentException(string.Format("The license does not have an attribute named {0}", name));
            }
        }

        /// <summary>
        /// Removes an attribute from the license. 
        /// </summary>
        /// <param name="name"></param>
        public void RemoveAttribute(string name)
        {
            if (_signature != null) throw new InvalidOperationException("This license has been signed, you may not change it.");

            if (_licenseAttributes.ContainsKey(name))
            {
                _licenseAttributes.Remove(name);
            }
            else
            {
                throw new ArgumentException(string.Format("The license does not have an attribute named {0}", name));
            }
        }
        #endregion

        /// <summary>
        /// Signs the license and generates XML from the attributes and private key. Once signed, the license may not be altered.
        /// </summary>
        /// <param name="privateKey"></param>
        /// <returns></returns>
        public void Sign(string privateKey)
        {
            if (_signature != null)
            {
                throw new InvalidOperationException("This license has already been signed. You may not re-sign it.");
            }

            XmlDocument XDoc = new XmlDocument();

            //Establish root element. 
            XmlElement license = XDoc.CreateElement("license");
            XDoc.AppendChild(license);

            foreach (var key in _licenseAttributes.Keys)
            {
                XmlElement licenseAttribute = XDoc.CreateElement("LicenseAttribute");
                XmlAttribute nameAttribute = XDoc.CreateAttribute("Name");
                nameAttribute.Value = key;

                licenseAttribute.Attributes.Append(nameAttribute);
                licenseAttribute.InnerText = _licenseAttributes[key];

                license.AppendChild(licenseAttribute);
            }

            //Now we need to append the security signature, which needs to be regenerated. 
            RSA encrKey = RSA.Create();
            encrKey.FromXmlString(privateKey);
            
            SignedXml sxml = new SignedXml(XDoc);
            sxml.SigningKey = encrKey;
            sxml.SignedInfo.CanonicalizationMethod = SignedXml.XmlDsigCanonicalizationUrl;

            // Add reference to XML data
            Reference r = new Reference("");
            r.AddTransform(new XmlDsigEnvelopedSignatureTransform(false));
            sxml.AddReference(r);

            // Build signature
            sxml.ComputeSignature();

            // Attach signature to XML Document
            XmlElement sig = sxml.GetXml();
            XDoc.DocumentElement.AppendChild(sig);

            _xmlLicense = XDoc;
        }

        /// <summary>
        /// Returns an XmlDocument object representing the license. 
        /// </summary>
        /// <returns></returns>
        public XmlDocument ToXml()
        {
            if (_xmlLicense == null) throw new InvalidOperationException("The license has not been loaded from xml or has not yet been signed - cannot generate XML.");
            return _xmlLicense;
        }

        /// <summary>
        /// Initializes a license from an XmlDocument object. 
        /// </summary>
        /// <param name="xmlDoc"></param>
        public void FromXml(XmlDocument xmlDoc)
        {
            InitializeFromXmlFile(xmlDoc);
        }

        /// <summary>
        /// Returns the string representation of the XML license. 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (_xmlLicense == null) return "Unloaded License.";
            return _xmlLicense.OuterXml;
        }
   
        /// <summary>
        /// Checks a license to ensure that it has not been validated.
        /// </summary>
        /// <param name="publicKey">The public key that was used to generate this license. This is NOT the private key that is used at generation time.</param>
        /// <returns>True or false. False means the file has been modified.</returns>
        public bool IsValid(string publicKey)
        {
            RSA key = RSA.Create();
            key.FromXmlString(publicKey);

            SignedXml sxml = new SignedXml(this._xmlLicense);
            try
            {
                // Find signature node
                XmlNode sig = this._xmlLicense.GetElementsByTagName("Signature")[0];
                sxml.LoadXml((XmlElement)sig);
            }
            catch
            {
                // Not signed!
                return false;
            }

            return sxml.CheckSignature(key);
        }

        /// <summary>
        /// Initializes the license from an XML file. 
        /// </summary>
        /// <param name="licenseContent"></param>
        private void InitializeFromXmlFile(XmlDocument licenseContent)
        {
            try
            {
                var xmlLicenseAttributes = licenseContent.GetElementsByTagName("LicenseAttribute");

                foreach (XmlNode node in xmlLicenseAttributes)
                {
                    string name = node.Attributes["Name"].Value;
                    string value = node.InnerText;
                    _licenseAttributes.Add(name, value);
                }

                _xmlLicense = licenseContent;
            }
            catch (Exception ex)
            {
                throw new Exception("Could not load license XML, it may be malformed. See inner exception for details", ex);
            }

            try
            {
                XmlElement signedElement = (XmlElement)licenseContent.SelectSingleNode("/license/*[local-name()='Signature']");

                _signature = new SignedXml(signedElement);
            }
            catch
            {
                //There was no signature! 
                _signature = null;
            }
        }
    }
}
