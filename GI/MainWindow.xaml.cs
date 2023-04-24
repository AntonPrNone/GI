using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Contexts;
using System.Text.RegularExpressions;
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

            List<CharacterDocument> people = new List<CharacterDocument>
            {
                new CharacterDocument { Name = "Аль-Хайтам", Category = "Играбельный", Description = "Аль-Хайтам - Дендро персонаж, который сражается при помощи Зеркал света. Он использует элементальный навык, совершает рывок и создаёт Зеркало света. Пока существуют Зеркала света, аль-Хайтам обретает Дендро инфузию. Кроме того, при попадании по противнику возникает эффект совместной атаки, который наносит Дендро урон по площади.При использовании взрыва стихии аль-Хайтам создаёт Особое связующее поле и наносит многократный Дендро урон по площади. Если во время активации навыка существуют Зеркала света, все зеркала израсходуются, увеличивая количество раз нанесения урона.", Element = "Дендро", Photo = @"\imgs\Камисато Аяка.png", Rarity = 5, Region = "Сумеру", Stats = new Stats { Health = 13348, Attack = 313, Defense = 781 }, Weapon = "Одноручное", UploadDate = DateTime.UtcNow.AddHours(3) },

                new CharacterDocument { Photo = @"\imgs\Камисато Аяка.png",Name = "Alice", Category = "1234" },
                new CharacterDocument { Photo = @"\imgs\Камисато Аяка.png",Name = "Alice", Category = "1234" },
                new CharacterDocument { Photo = @"\imgs\Камисато Аяка.png",Name = "Alice", Category = "1234" },
                new CharacterDocument { Photo = @"\imgs\Камисато Аяка.png",Name = "Alice", Category = "1234" },
                new CharacterDocument { Name = "Bob", Category = "1234e" },
                new CharacterDocument { Name = "Charlie", Category = "1234r" }
            };

            listBox.ItemsSource = people;
        }

        private async void LoadData()
        {
            await Task.Delay(1000);

            var fadeInTasks = new Task[]
            {
                FadeInAsync(LoadingCircle, 1),
                FadeInAsync(Ldg_label, 1)
            };
            await Task.WhenAll(fadeInTasks);

            var character = new CharacterDocument() { Stats = new Stats() };

            character.Name = "Аль-Хайтам";
            character.Photo = "";
            character.Category = "Играбельные";
            character.Description = "Аль-Хайтам - Дендро персонаж, который сражается при помощи Зеркал света.Он использует элементальный навык, совершает рывок и создаёт Зеркало света. Пока существуют Зеркала света, аль-Хайтам обретает Дендро инфузию. Кроме того, при попадании по противнику возникает эффект совместной атаки, который наносит Дендро урон по площади. При использовании взрыва стихии аль-Хайтам создаёт Особое связующее поле и наносит многократный Дендро урон по площади. Если во время активации навыка существуют Зеркала света, все зеркала израсходуются, увеличивая количество раз нанесения урона.";
            character.Rarity = 5;
            character.Element = "Дендро";
            character.Weapon = "Одноручное";
            character.Region = "Сумеру";
            character.Stats.Attack = 313;
            character.Stats.Health = 13348;
            character.Stats.Defense = 781;

            var connect = new CharactersManager();
            await connect.UploadCharacterAsync(character);

            //await Task.Delay(10000);

            var fadeOutTasks = new Task[]
            {
                FadeOutAsync(LoadingCircle, 1),
                FadeOutAsync(Ldg_label, 1)
            };

            await Task.WhenAll(fadeOutTasks);

        }

        private static async Task FadeInAsync(UIElement element, double duration)
        {
            DoubleAnimation fadeIn = new DoubleAnimation()
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromSeconds(duration)
            };
            element.Visibility = Visibility.Visible;
            element.BeginAnimation(UIElement.OpacityProperty, fadeIn);

            await Task.Delay((int)(duration * 1000));
        }

        private static async Task FadeOutAsync(UIElement element, double duration)
        {
            DoubleAnimation fadeOut = new DoubleAnimation()
            {
                From = 1,
                To = 0,
                Duration = TimeSpan.FromSeconds(duration)
            };
            element.BeginAnimation(UIElement.OpacityProperty, fadeOut);

            await Task.Delay((int)(duration * 1000));
            element.Visibility = Visibility.Collapsed;
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
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private async void AddButton_ClickAsync(object sender, RoutedEventArgs e)
        {
            var character = new CharacterDocument() { Stats = new Stats() };

            character.Name = NameTextBox.Text;
            character.Category = CategoryTextBox.Text;
            character.Description = Regex.Replace(DescriptionTextBox.Text.Replace("\r", " ").Replace("\n", " ").Trim(), @"\s+", " ");
            character.Rarity = Convert.ToInt32(RarityTextBox.Text);
            character.Element = ElementTextBox.Text;
            character.Weapon = WeaponTextBox.Text;
            character.Region = RegionTextBox.Text;
            character.Stats.Attack = Convert.ToInt32(AttackTextBox.Text);
            character.Stats.Health = Convert.ToInt32(HealthTextBox.Text);
            character.Stats.Defense = Convert.ToInt32(DefenseTextBox.Text);

            var connect = new CharactersManager();
            await connect.UploadCharacterAsync(character);
        }
    }
}
