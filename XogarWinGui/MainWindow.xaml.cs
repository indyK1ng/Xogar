using System.ComponentModel;
using System.Windows;
using XogarLib;

namespace XogarWinGui
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Games picker;

        public MainWindow()
        {
            InitializeComponent();
            picker = new Games();
        }

        // Electing not to use a command pattern right now because
        //  there is only one way to access this.  If/when the UI gets
        //  more complicated where there are multiple ways to do things,
        //  this will be refactored.
        private void LaunchRandom(object sender, RoutedEventArgs e)
        {
            Game launcher = picker.PickRandomGame();
            launcher.Launch();
        }

        private void NewThirdParty_OnClick(object sender, RoutedEventArgs e)
        {
            var window = new AddThirdParty();
            window.gameList = picker;
            window.ShowDialog();
        }

        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            picker.ThirdParty.Save();
        }
    }
}