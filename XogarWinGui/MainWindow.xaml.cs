using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
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
        private readonly Playlists gamesLists;
        private IEnumerable<Playlist> AllPlaylists => new List<Playlist> { picker.AllGamesPlaylist }.Union(gamesLists.CustomPlaylists);
        public Dictionary<String, Playlist> PlaylistDictionary => AllPlaylists.ToDictionary(l => l.Name, l => l);


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

            playlistBox.ItemsSource = PlaylistDictionary;
            playlistBox.SelectedIndex = 0;
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
            var window = new CreatePlaylist { GameContainer = picker };
            window.ShowDialog();

            if (window.newList != null)
            {
                gamesLists.CustomPlaylists.Add(window.newList);
            }

            playlistBox.ItemsSource = PlaylistDictionary;
        }

        private void PlayRandom_Click(object sender, RoutedEventArgs e)
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

        private SteamInstallSelectorWindow ChangeSteamInstallDir()
        {
            var window = new SteamInstallSelectorWindow();
            window.ShowDialog();
            return window;
        }

        private void Exit_OnClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}