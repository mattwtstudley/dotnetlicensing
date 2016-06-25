using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetLicense
{
    /// <summary>
    /// Exception thrown when a DotNetLicense license file is not valid. 
    /// </summary>
    public class LicenseVerificationException : Exception
    {
        private string _message;
        public override string Message
        {
            get
            {
                return _message;
            }
        }

        public LicenseVerificationException(string message)
        {
            _message = message;
        }
    }
}
