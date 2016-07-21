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
        /// <summary>
        /// A description of why the LicenseVerificationException was thrown. 
        /// </summary>
        public override string Message
        {
            get
            {
                return _message;
            }
        }

        /// <summary>
        /// Creates a new license verification exception with a given message. 
        /// </summary>
        /// <param name="message">A message describing the reason for the License exception.</param>
        public LicenseVerificationException(string message) : base(message)
        {
            _message = message;
        }

        /// <summary>
        /// Creates a license exception with an inner exception and a message. 
        /// </summary>
        /// <param name="message">A message describing the reason for the exception.</param>
        /// <param name="innerException">The inner exception that caused the license exception.</param>
        public LicenseVerificationException(string message, Exception innerException) : base(message,innerException)
        {
            _message = message;
        }
    }
}
