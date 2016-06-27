using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetLicense;
using DotNetLicensing.Examples.Northwind.MVVM;
using System.Windows.Input;

namespace DotNetLicensing.Examples.Northwind.ViewModels
{
    public class ManageLicenseVM : ViewModel
    {
        private DotNetLicense.LicenseManager _licenseManager;
        private Northwind.Models.NorthwindLicense _license;
        private string _keyName;
        private string _keyDirectory;

        public ManageLicenseVM()
        {
            _licenseManager = new LicenseManager();
            _license = new Models.NorthwindLicense();
        }

        public string PublicKey
        {
            get
            {
                return _licenseManager.PublicKey;
            }
            private set
            {
                _licenseManager.LoadPublicKeyFromString(value);
                RaisePropertyChangedEvent("PublicKey");
            }
        }

        public string PrivateKey
        {
            get
            {
                return _licenseManager.PrivateKey;
            }
            private set
            {
                _licenseManager.LoadPrivateKeyFromString(value);
                RaisePropertyChangedEvent("PrivateKey");
            }
        }

        public DateTime ExpirationDate
        {
            get
            {
                return _license.ExpirationDate;
            }
            set
            {
                _license.ExpirationDate = value;
                RaisePropertyChangedEvent("ExpirationDate");
            }
        }

        public string LicensedTo
        {
            get
            {
                return _license.LicensedTo;
            }
            set
            {
                _license.LicensedTo = value;
                RaisePropertyChangedEvent("LicensedTo");
            }
        }

        public int NumberOfUsers
        {
            get
            {
                if (_license == null) return 0;
                return _license.NumberOfUsers;
            }
            set
            {
                if (_license != null)
                {
                    _license.NumberOfUsers = value;
                    RaisePropertyChangedEvent("NumberOfUsers");
                }
            }
        }

        public bool CanLoadLicense
        {
            get
            {
                if (!String.IsNullOrEmpty(PublicKey))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool CanCreateLicense
        {
            get
            {
                if (!String.IsNullOrEmpty(PrivateKey))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }



        public string Expired
        {
            get
            {
                if (ExpirationDate < DateTime.Now)
                {
                    return "License Expired!";
                }
                else
                {
                    return "License is valid.";
                }
            }
        }

        public string KeyName
        {
            get
            {
                return _keyName;
            }
            set
            {
                _keyName = value;
                RaisePropertyChangedEvent("KeyName");
            }
        }
        public string KeyDirectory
        {
            get
            {
                return _keyDirectory;
            }
            set
            {
                _keyDirectory = value;
                RaisePropertyChangedEvent("KeyDirectory");
            }
        }

        #region Commands
        internal ICommand CreateKeyPairCommand()
        {
            DelegateCommand cmd = new DelegateCommand(CreateKeyPair);
            return cmd;
        }

        internal void LoadPrivateKey(string pathToKey)
        {
            _licenseManager.LoadPrivateKeyFromFile(pathToKey);
            RaisePropertyChangedEvent("PrivateKey");
            RaisePropertyChangedEvent("CanLoadLicense");
            RaisePropertyChangedEvent("CanCreateLicense");
        }

        internal void LoadPublicKey(string pathToKey)
        {
            _licenseManager.LoadPublicKeyFromFile(pathToKey);
            RaisePropertyChangedEvent("PublicKey");
            RaisePropertyChangedEvent("CanLoadLicense");
            RaisePropertyChangedEvent("CanCreateLicense");
        }

        internal void LoadLicense(string pathToLicense)
        {
            
            License baseLicense = _licenseManager.LoadLicenseFromDisk(pathToLicense);
            this._license = new Models.NorthwindLicense(baseLicense);
            RaisePropertyChangedEvent("LicensedTo");
            RaisePropertyChangedEvent("KeyName");
            RaisePropertyChangedEvent("NumberOfUsers");
            RaisePropertyChangedEvent("Expired");
        }

        internal void CreateLicense(string pathToSaveTo)
        {
            _licenseManager.SignAndSaveNewLicense(this._license, pathToSaveTo);
        }

        internal void CreateKeyPair()
        {
            _licenseManager.CreateKeyPairs(KeyDirectory, KeyName);
            string privateKeyFilePath = string.Format("{0}\\{1}_private.key", KeyDirectory, KeyName);
            string publicKeyFilePath = string.Format("{0}\\{1}_public.key", KeyDirectory, KeyName);

            LoadPrivateKey(privateKeyFilePath);
            LoadPublicKey(publicKeyFilePath);


        }
        #endregion

    }
}
