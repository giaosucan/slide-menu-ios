using MahApps.Metro.Controls;
using SlideMenuControl.CustomTile;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace TestApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private const String FOLDER_NAME = @"Icon";
        private const String FOLDER_NAME_HOVER = @"IconHover";
        private const String FOLDER_NAME_CLICK = @"IconClick";

        /// <summary>
        ///
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            string[] listIcon = Directory.GetFiles(FOLDER_NAME, "*.png");
            string[] listIconHover = Directory.GetFiles(FOLDER_NAME_HOVER, "*.png");
            string[] listIconClick = Directory.GetFiles(FOLDER_NAME_CLICK, "*.png");
            List<CustomTile> listItem = new List<CustomTile>();

            for (int i = 0; i < listIcon.Length; i++)
            {
                CustomTile tile = new CustomTile();
                tile.TileText = System.IO.Path.GetFileNameWithoutExtension(listIcon[i]);
                tile.ImageIcon = Directory.GetCurrentDirectory() + @"\" + listIcon[i];
                tile.ImagePathDefault = Directory.GetCurrentDirectory() + @"\" + listIcon[i];
                tile.ImagePathHover = Directory.GetCurrentDirectory() + @"\" + listIconHover[i];
                tile.ImagePathClick = Directory.GetCurrentDirectory() + @"\" + listIconClick[i];
                tile.TileHeight = 120;
                tile.TileWidth = 120;
                tile.TileMargin = 10;
                tile.TitleColor = Brushes.LightGray;
                tile.TileID = i.ToString();
                tile.Name = @"Item_" + i;
                tile.EventCommand += tile_Tap;
                listItem.Add(tile);
            }
            // Init menu.
            sldMenu.ListItem = listItem;
        }

        private void tile_Tap(object sender, RoutedEventArgs e)
        {
            CustomTile tile = e.Source as CustomTile;
            if (tile != null)
            {
                imgPage.Source = new BitmapImage(new Uri(tile.ImageIcon, UriKind.RelativeOrAbsolute));
            }
        }

        private void Border_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Debug.WriteLine("Test App : Border_MouseLeftButtonDown");
            sldMenu.StopMoveItemMode();           
            e.Handled = true;
        }
                
        private void Border_PreviewTouchDown(object sender, TouchEventArgs e)
        {
            Debug.WriteLine("Test App : Border_PreviewTouchDown");
            sldMenu.StopMoveItemMode();
            e.Handled = true;
        }
    }
}