using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.Windows.Forms;

namespace XogarWinGui
{
    /// <summary>
    /// Interaction logic for SteamInstallSelectorWindow.xaml
    /// </summary>
    public partial class SteamInstallSelectorWindow : Window
    {
        public String InstallDirectory { get; private set; }
        public SteamInstallSelectorWindow()
        {
            InitializeComponent();
            InstallDirectory = (String)Registry.GetValue("HKEY_CURRENT_USER\\SOFTWARE\\Valve\\Steam", "SteamPath", null);
            SteamInstallBox.Text = InstallDirectory;
            ExplanationBlock.Text = Properties.Settings.Default.SteamNotInDefaultDir;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            InstallDirectory = SteamInstallBox.Text;
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            InstallDirectory = null;
            this.Close();
        }

        private void DirectorySelectButton_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();

            DialogResult result = dialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                SteamInstallBox.Text = dialog.SelectedPath;
            }
        }
    }
}
