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

namespace GI
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Normal)
            {
                this.WindowState = WindowState.Maximized;
                this.MaximizeButton.Content = new Image
                {
                    Source = new BitmapImage(new Uri("/imgs/expand.png", UriKind.Relative)),
                    Width = 16,
                    Height = 16
                };
            }
            else
            {
                this.WindowState = WindowState.Normal;
                this.MaximizeButton.Content = new Image
                {
                    Source = new BitmapImage(new Uri("/imgs/expand2.png", UriKind.Relative)),
                    Width = 16,
                    Height = 16
                };
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
