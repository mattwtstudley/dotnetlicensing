using DotNetLicensing.Examples.Northwind.MVVM;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace DotNetLicensing.Examples.Northwind
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            ViewModel vm = new ViewModels.ManageLicenseVM();
            Window view = new MainWindow();
            view.Show();
            view.DataContext = vm;  
        }
    }
}
