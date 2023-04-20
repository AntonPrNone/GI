using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;

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

            LoadData();
        }

        private async void LoadData()
        {
            // Показываем загрузочный круг
            LoadingCircle.Visibility = Visibility.Visible;

            // Загружаем данные из БД

            /*var db = new CharactersManager();
            await db.UploadCharacterAsync(new CharacterDocument() { Name = "NAME1", Region = "Tatarstan", Stats = new Stats() });*/
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
