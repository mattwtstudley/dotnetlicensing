# DotNetLicense

DotNetLicense is a library that can be used to create and validate RSA signed xml license files which you can use for licensing an application.
 It is based on the original code examples found at [http://www.dotnetlicensing.net](http://www.dotnetlicensing.net), but has a lot of additional work done to seperate the implementation of the license from
the business logic. 

## Quick Start

1. Include the library in your project:

`PM> Install-Package DotNetLicense`

2. Create a class in your project that represents your license. In your `get` and `set` methods, utilize the base.GetAttribute() and base.AddOrChangeAttribute() methods from the
    license class to register your attributes. The license will store everything as a string, so make sure to parse where appropriate. 

    In the example below we have a sample license for the imaginary Northwind company. The license has three properties for ExpirationDate, NumberOfUsers,
    and LicensedTo:

```cs
using System;
using DotNetLicense;

namespace DotNetLicensing.Examples.Northwind.Models
{
    public class NorthwindLicense : License
    {
        /// <summary>
        /// Creates a new Northwind license, unsigned. 
        /// </summary>
        public NorthwindLicense() : base()
        {
            //We make sure to intialize our values in the license so that we have base values. 
            this.ExpirationDate = DateTime.Now;
            this.LicensedTo = "";
            this.NumberOfUsers = 0;
        }

        /// <summary>
        /// Constructs a NorthwindLicense based on the super-class license returned from DotNetLicense.LicenseManager
        /// </summary>
        /// <param name="baseLicense"></param>
        public NorthwindLicense(License baseLicense) : base(baseLicense.ToXml())
        {
         
        }

        /// <summary>
        /// The date the license expires. 
        /// </summary>
        public DateTime ExpirationDate
        {
            get
            {
                return DateTime.Parse(base.GetAttribute("ExpirationDate"));
            }
            set
            {
                base.AddOrChangeAttribute("ExpirationDate", value.ToShortDateString());
            }
        }

        /// <summary>
        /// The person or organization the license was issued to.
        /// </summary>
        public string LicensedTo
        {
            get
            {
                return base.GetAttribute("LicensedTo");
            }
            set
            {
                base.AddOrChangeAttribute("LicensedTo", value);
            }
        }

        /// <summary>
        /// The number of users this license supports. 
        /// </summary>
        public int NumberOfUsers
        {
            get
            {
                return int.Parse(base.GetAttribute("NumberOfUsers"));
            }
            set
            {
                base.AddOrChangeAttribute("NumberOfUsers", value.ToString());
            }
        }
    }
}
```



