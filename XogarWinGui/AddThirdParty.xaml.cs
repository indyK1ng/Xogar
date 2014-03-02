using System.Windows;
using Microsoft.Win32;
using XogarLib;

namespace XogarWinGui
{
    /// <summary>
    /// Interaction logic for AddThirdParty.xaml
    /// </summary>
    public partial class AddThirdParty : Window
    {
        public Games gameList;

        public AddThirdParty()
        {
            InitializeComponent();
        }

        private void FileSelectorClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();

            if (dialog.ShowDialog().Value)
            {
                ExecutableLocation.Text = dialog.FileName;
            }
        }

        private void OkClick(object sender, RoutedEventArgs e)
        {
            ThirdPartyGame newGame = new ThirdPartyGame();
            newGame.Executable = ExecutableLocation.Text;
            newGame.Name = GameName.Text;
            newGame.Arguments = RunningOptions.Text;
            gameList.ThirdParty.Add(newGame);
            this.Close();
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
