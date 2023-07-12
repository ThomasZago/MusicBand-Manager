﻿using MusicBand_Manager.Tool;
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

namespace MusicBand_Manager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            InitializeNavigation();
        }

        private void InitializeNavigation()
        {
            Navigation.Instance.Initialize(mainFrame);
            Navigation.Instance.NavigateTo("RepertoireView");
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                var tabItem = (TabItem)e.AddedItems[0];
                var pageName = (string)tabItem.Tag;
                Navigation.Instance.NavigateTo(pageName);
            }
        }
    }
}
