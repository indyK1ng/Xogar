using System;
using System.Collections.Generic;
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
using XogarLib;

namespace XogarWinGui
{
    /// <summary>
    /// Interaction logic for CreatePlaylist.xaml
    /// </summary>
    public partial class CreatePlaylist : Window
    {
        public Playlist newList;

        public Games GameContainer { get; set; }

        public CreatePlaylist()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            newList = new Playlist(NameBox.Text);

            foreach (KeyValuePair<String, Game> selected in SelectedGames.SelectedItems)
            {
                newList.AddGame(selected.Key);
            }

            this.Close();
        }
    }
}
