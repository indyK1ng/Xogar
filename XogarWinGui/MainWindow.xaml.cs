using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Documents;
using XogarLib;

namespace XogarWinGui
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Games picker;
        private Playlists gamesLists;

        private static string steamInstallDirStoragePath =
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Xogar\\" +
            Properties.Settings.Default.SteamInstallDirStorage;

        public MainWindow()
        {
            InitializeComponent();
            try
            {
                picker = new Games();
            }
            catch (Exception)
            {
                if (File.Exists(steamInstallDirStoragePath))
                {
                    ReadInstallLocationAndLoadGames();
                }
                else
                {
                    PromptForSteamInstallDir();
                }
            }

            gamesLists = Playlists.Load();
            DataContext = this;

            playlistBox.ItemsSource = playlistDictionary;
        }

        private void ReadInstallLocationAndLoadGames()
        {
            FileStream reader = new FileStream(steamInstallDirStoragePath, FileMode.Open);
            StreamReader locationReader = new StreamReader(reader);

            String actualInstallDir = locationReader.ReadToEnd();
            picker = new Games(actualInstallDir);
            locationReader.Close();
        }

        private void PromptForSteamInstallDir()
        {
            String actualInstallDir = ChangeSteamInstallDir().InstallDirectory;
            picker = new Games(actualInstallDir);

            SaveSteamInstallDir(actualInstallDir);
        }

        private static void SaveSteamInstallDir(string actualInstallDir)
        {
            CreateAppDataFolder();
            FileStream saveLocation = new FileStream(steamInstallDirStoragePath, FileMode.Create);
            StreamWriter locationWriter = new StreamWriter(saveLocation);
            locationWriter.AutoFlush = true;
            locationWriter.Write(actualInstallDir.ToString());
            saveLocation.Close();
        }

        public Dictionary<String, Playlist> playlistDictionary
        {
            get
            {
                Dictionary<String, Playlist> tempDict = new Dictionary<string, Playlist>();

                foreach (Playlist tempList in gamesLists.Lists)
                {
                    tempDict.Add(tempList.Name, tempList);
                }

                return tempDict;
            }
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
            picker.UpdateGameList();
        }

        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            CreateAppDataFolder();

            picker.ThirdParty.Save();
            gamesLists.Save();
        }

        private static void CreateAppDataFolder()
        {
            string envFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Xogar\\";
            if (!Directory.Exists(envFolder))
            {
                Directory.CreateDirectory(envFolder);
            }
        }

        private void Create_PlaylistClick(object sender, RoutedEventArgs e)
        {
            var window = new CreatePlaylist();
            window.GameContainer = picker;
            window.ShowDialog();

            if (window.newList != null)
            {
                gamesLists.Lists.Add(window.newList);
            }

            playlistBox.ItemsSource = playlistDictionary;
        }

        private void PlaylistRandom_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Game launcher = picker.PickRandomGameFromPlaylist(0);
                launcher.Launch();
            }
            catch
            {
                MessageBox.Show(
                    "There appears to have been an issue finding a game to launch from that playlist.\nIt's possible no games from that list are installed.");
            }
        }

        private void playlistBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (playlistBox.SelectedIndex > -1)
            {
                picker.SelectedPlaylist = ((KeyValuePair<String, Playlist>) playlistBox.SelectedValue).Value;

                List<Game> playlistGames = new List<Game>();

                foreach (String gameHash in picker.SelectedPlaylist.GameHashes)
                {
                    if (picker.GamesToPick.ContainsKey(gameHash))
                    {
                        playlistGames.Add(picker.GamesToPick[gameHash]);
                    }
                }

                PlaylistItems.ItemsSource = playlistGames;
            }
            else
            {
                picker.SelectedPlaylist = null;

                PlaylistItems.ItemsSource = new List<Game>();
            }
        }

        private SteamInstallSelectorWindow ChangeSteamInstallDir()
        {
            var window = new SteamInstallSelectorWindow();
            window.ShowDialog();
            return window;
        }
    }
}