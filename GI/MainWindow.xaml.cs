﻿using System;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GI
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /*  DateTime.UtcNow.AddHours(3) */

        public MainWindow()
        {
            InitializeComponent();

            var db = new ImageUploader(collectionName : "ImgsGI2");
            //db.UploadImageAsync("close");
            //db.LoadImageFromDbAsync();
            //var db2 = new CharactersManager();
            //db2.UploadCharacterAsync(new Character() { Name = "NAME", Region = "Tatarstan", Stats = new Stats() });
            LoadData();

        }

        private async void LoadData()
        {
            // Показываем загрузочный круг
            LoadingCircle.Visibility = Visibility.Visible;

            // Загружаем данные из БД
            var db2 = new CharactersManager();
            //await db2.UploadCharacterAsync(new Character() { Name = "NAME1", Region = "Tatarstan", Stats = new Stats() });
            await Task.Delay(-1);

            // Скрываем загрузочный круг
            LoadingCircle.Visibility = Visibility.Collapsed;
        }

        // --------------------------------- Взаимодействие с окном --------------------------------------------

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            var closingAnimation = (Storyboard)FindResource("ClosingAnimation");
            closingAnimation.Completed += (s, _) => Close();
            BeginStoryboard(closingAnimation);
        }

        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
    }
}
