using DotNetLicensing.Examples.Northwind.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DotNetLicensing.Examples.Northwind
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// <remarks>Hey! You've got code in your code-behind! This isn't MVVM!!! - Yes, you are technically correct.
    ///             However, since WPF's support of file/folder open dialogs is poor and I don't feel like including
    ///             a bunch of components for the sake of an example on licensing, I've decided to go this route.</remarks>

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnLoadPrivateKey_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Filter = "License Keys (*.key)|*.key";

            var result = dialog.ShowDialog();
            if (result.HasValue && result.Value)
            {
                (this.DataContext as ManageLicenseVM).LoadPrivateKey(dialog.FileName);
            }
        }

        private void btnLoadPublicKey_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Filter = "License Keys (*.key)|*.key";

            var result = dialog.ShowDialog();
            if (result.HasValue && result.Value)
            {
                (this.DataContext as ManageLicenseVM).LoadPublicKey(dialog.FileName);
            }
        }

        private void btnGenerateKeys_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                (this.DataContext as ManageLicenseVM).KeyDirectory = dialog.SelectedPath;
                (this.DataContext as ManageLicenseVM).CreateKeyPair();
            }
        }

        private void btnLoadLicense_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Filter = "License Files (*.lic)|*.lic|All Files|*.*";

            var result = dialog.ShowDialog();
            if (result.HasValue && result.Value)
            {
                (this.DataContext as ManageLicenseVM).LoadLicense(dialog.FileName);
            }
        }

        private void btnCreateLicense_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.SaveFileDialog();
            dialog.Filter = "License Files (*.lic)|*.lic|All Files|*.*";

            var result = dialog.ShowDialog();
            if (result.HasValue && result.Value)
            {
                (this.DataContext as ManageLicenseVM).CreateLicense(dialog.FileName);
            }
        }
    }
}
