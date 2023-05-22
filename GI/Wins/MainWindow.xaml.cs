using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using LogicLibrary;

namespace GI
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
	{
		readonly CharactersManager charactersManager = new CharactersManager();
        readonly ImageManager imageManager = new ImageManager();
		List<CharacterDocument> characters;
		List<CharacterDocument> charactersActual;
		List<CharacterDocument> charactersPl;
		List<CharacterDocument> charactersNoPl;
		List<CharacterDocument> charactersFav;
        readonly string directoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "GI", "imgs", "pers");
        readonly string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "LogPas.txt");
        readonly string username;
		bool initial = true;
		ListBox listBox;
		User user;
		readonly string pathFavEmpty = @"/imgs/favoriteEmpty.png";
		readonly string pathFav = @"/imgs/favorite.png";
		readonly ImageSource imageSourceFavEmpty = new BitmapImage(new Uri(@"/imgs/favoriteEmpty.png", UriKind.Relative));
		readonly ImageSource imageSourceFav = new BitmapImage(new Uri(@"/imgs/favorite.png", UriKind.Relative));

		public MainWindow()
		{
			InitializeComponent();
			listBox = listBox1;
			listBox.Opacity = 0;
			username = File.ReadAllLines(path)[0];

			LoadData();
		}

		private async Task UpdateData() // Импорт и расставление данных
		{
			user = await UserManager.GetUserAsync(username);
            if (user.FavoriteСharacters == null) user.FavoriteСharacters = new List<string>();
            Username_TextBlock.Text = username;
			characters = await charactersManager.LoadCharacterAsync();
			charactersPl = (List<CharacterDocument>)(listBox.ItemsSource = characters.Where(character => character.Category == "Играбельный").ToList());
			charactersNoPl = (List<CharacterDocument>)(listBox.ItemsSource = characters.Where(character => character.Category == "Неиграбельный").ToList());
			charactersActual = charactersPl;
			await imageManager.LoadImageFromDbAsync();

            if (user.FavoriteСharacters != null)
            {
                foreach (string favoriteCharacterName in user.FavoriteСharacters)
			    {
				    int chr = characters.FindIndex(c => c.Name == favoriteCharacterName);
				    if (chr != -1)
				    {
					    characters[chr].FavoriteСharacters = pathFav;
				    }

				    else
				    {
					    characters[chr].FavoriteСharacters = pathFavEmpty;
				    }
			    }
            }
			
			foreach (CharacterDocument character in characters)
			{
				if (character.FavoriteСharacters == null || character.FavoriteСharacters == "")
					character.FavoriteСharacters = pathFavEmpty;
				character.Photo = Path.Combine(directoryPath, Path.ChangeExtension(character.Name, ".png"));
			}

			charactersFav = (List<CharacterDocument>)(listBox.ItemsSource = characters.Where(character => character.FavoriteСharacters == pathFav).ToList());

			listBox.ItemsSource = charactersPl;
        }

		private async void LoadData() // Загрузка данных с анимацией загрузки
		{
            _ = new Task[]
            {
                Anim.FadeInAsync(LoadingCircle, 1.5),
                Anim.FadeInAsync(Ldg_TextBlock, 1.5)
            };

            await UpdateData();
            await Task.Delay(1);

            _ = new Task[]
            {
                Anim.FadeOutAsync(LoadingCircle, 1.5),
                Anim.FadeOutAsync(Ldg_TextBlock, 1.5)
            };
        }

		private void TextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e) // Поиск персонажа
		{
			// Применение фильтра к данным Characters на основе SearchText
			var filteredCharacters = charactersActual.Where(c => c.Name.Contains(((TextBox)sender).Text)).ToList();
			if (filteredCharacters.Count == 0)
				listBox.Width = listBox.ActualWidth;
			else
				listBox.Width = double.NaN;
			// Обновление отображаемых данных в ListBox
			listBox.ItemsSource = filteredCharacters;
		}

		private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e) // Смена вкладки
		{
			if (!initial) // Если сейчас не инциализация окна
			{
				Search_TextBox.Text = string.Empty;
				var selectedTab = TabControl.SelectedIndex;
				if (selectedTab == 0)
				{
					listBox = listBox1;
					charactersActual = charactersPl;
				}

				else if (selectedTab == 1)
				{
					listBox = listBox2;
					charactersActual = charactersNoPl;

				}

				else if (selectedTab == 2)
				{
					listBox = listBox3;
					charactersActual = charactersFav;
				}

				listBox.ItemsSource = charactersActual;
			}

			initial = false;
		}

		private void Image_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e) // Добавление/Удаление в/из Избранного
		{
            Image clickedImage = (Image)sender;
            ListBoxItem listBoxItem = FindParent<ListBoxItem>(clickedImage); // Получаем родительский listBoxItem

            if (listBoxItem != null)
			{
                // Получаем DataContext выбранного элемента
                if (listBoxItem.DataContext is CharacterDocument selectedCharacter)
                {
                    if (user.FavoriteСharacters != null && user.FavoriteСharacters.Contains(selectedCharacter.Name)) // Если уже в избранном
                    {
                        user.FavoriteСharacters.Remove(selectedCharacter.Name);
                        ((System.Windows.Controls.Image)sender).Source = imageSourceFavEmpty;

                        CharacterDocument characterToRemove = charactersFav.FirstOrDefault(c => c.Name == selectedCharacter.Name);
                        int character1 = charactersPl.FindIndex(c => c.Name == selectedCharacter.Name);
                        int character2 = charactersNoPl.FindIndex(c => c.Name == selectedCharacter.Name);
                        if (characterToRemove != null)
                        {
                            charactersFav.Remove(characterToRemove);
                            if (character1 != -1)
                                charactersPl[character1].FavoriteСharacters = pathFavEmpty;
                            if (character2 != -1)
                                charactersNoPl[character2].FavoriteСharacters = pathFavEmpty;

                            listBox3.ItemsSource = null;
                            listBox3.ItemsSource = charactersFav;
                            listBox2.ItemsSource = null;
                            listBox2.ItemsSource = charactersNoPl;
                            listBox1.ItemsSource = null;
                            listBox1.ItemsSource = charactersPl;
                        }
                    }

                    else // Если не в избранном
                    {
                        user.FavoriteСharacters.Add(selectedCharacter.Name);
                        ((System.Windows.Controls.Image)sender).Source = imageSourceFav;

                        CharacterDocument characterToAdd = characters.FirstOrDefault(c => c.Name == selectedCharacter.Name);
                        int character1 = charactersPl.FindIndex(c => c.Name == selectedCharacter.Name);
                        int character2 = charactersNoPl.FindIndex(c => c.Name == selectedCharacter.Name);
                        if (characterToAdd != null)
                        {
                            charactersFav.Add(characterToAdd);
                            if (character1 != -1)
                                charactersPl[character1].FavoriteСharacters = pathFav;
                            if (character2 != -1)
                                charactersNoPl[character2].FavoriteСharacters = pathFav;
                            charactersActual = charactersFav;

                            listBox3.ItemsSource = null;
                            listBox3.ItemsSource = charactersFav;
                            listBox2.ItemsSource = null;
                            listBox2.ItemsSource = charactersNoPl;
                            listBox1.ItemsSource = null;
                            listBox1.ItemsSource = charactersPl;
                        }
                    }
                }
            }
        }

        private T FindParent<T>(DependencyObject child) where T : DependencyObject // Поиск родителя
        {
            DependencyObject parent = VisualTreeHelper.GetParent(child);

            if (parent == null)
                return null;

            T parentT = parent as T;
            return parentT ?? FindParent<T>(parent);
        }

        private async void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) // Выбор критерия сортировки в ComboBox
        {
            if (!initial)
            {
                ComboBox comboBox = (ComboBox)sender;
                ComboBoxItem selectedItem = (ComboBoxItem)comboBox.SelectedItem;
                string selectedSorting = selectedItem.Content.ToString();

                // Выполнение сортировки в зависимости от выбранного варианта
                switch (selectedSorting)
                {
                    case "По имени ↑":
                        charactersPl = charactersPl.OrderBy(c => c.Name).ToList();
                        charactersNoPl = charactersNoPl.OrderBy(c => c.Name).ToList();
                        charactersFav = charactersFav.OrderBy(c => c.Name).ToList();
                        break;

                    case "По имени ↓":
                        charactersPl = charactersPl.OrderByDescending(c => c.Name).ToList();
                        charactersNoPl = charactersNoPl.OrderByDescending(c => c.Name).ToList();
                        charactersFav = charactersFav.OrderByDescending(c => c.Name).ToList();
                        break;

                    case "По редкости ↑":
                        charactersPl = charactersPl.OrderBy(c => c.Rarity).ToList();
                        charactersNoPl = charactersNoPl.OrderBy(c => c.Rarity).ToList();
                        charactersFav = charactersFav.OrderBy(c => c.Rarity).ToList();
                        break;


                    case "По редкости ↓":
                        charactersPl = charactersPl.OrderByDescending(c => c.Rarity).ToList();
                        charactersNoPl = charactersNoPl.OrderByDescending(c => c.Rarity).ToList();
                        charactersFav = charactersFav.OrderByDescending(c => c.Rarity).ToList();
                        break;

                    case "По региону":
                        charactersPl = charactersPl.OrderByDescending(c => c.Region).ToList();
                        charactersNoPl = charactersNoPl.OrderByDescending(c => c.Region).ToList();
                        charactersFav = charactersFav.OrderByDescending(c => c.Region).ToList();
                        break;

                    case "По оружию":
                        charactersPl = charactersPl.OrderByDescending(c => c.Weapon).ToList();
                        charactersNoPl = charactersNoPl.OrderByDescending(c => c.Weapon).ToList();
                        charactersFav = charactersFav.OrderByDescending(c => c.Weapon).ToList();
                        break;
                }

                // Обновление источника данных для ListBox
                await Anim.FadeOutAsync(listBox, 0.5);

                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    listBox1.ItemsSource = charactersPl;
                    listBox2.ItemsSource = charactersNoPl;
                    listBox3.ItemsSource = charactersFav;
                });

                await Task.Delay(100); // Дополнительная задержка для убедительности

                await Application.Current.Dispatcher.InvokeAsync(async () =>
                {
                    await Anim.FadeInAsync(listBox, 0.5);
                });
            }
        }

        // Непользовательское добавление персонажа
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
            character.FavoriteСharacters = string.Empty;

			var connect = new CharactersManager();
			await connect.UploadCharacterAsync(character);
		}

        // --------------------------------- Взаимодействие с окном --------------------------------------------

        private async void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) // Выход из профиля
        {
            File.WriteAllText(path, string.Empty);
            RegWin regWin = new RegWin();
            regWin.Show();
            await Anim.FadeOut2Async(this);
            Close();
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e) // Свернуть окно
        {
            this.WindowState = WindowState.Minimized;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e) // Закрытие окна
        {
            var closingAnimation = (Storyboard)FindResource("ClosingAnimation");
            closingAnimation.Completed += (s, _) => Close();
            BeginStoryboard(closingAnimation);
        }

        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e) // Перемещение окна
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private async void Window_Closed(object sender, EventArgs e) // Перед закрытием окна
		{
			await UserManager.ReplaceUserAsync(user);
		}

        private async void Window_Loaded(object sender, RoutedEventArgs e) // После отрисовки окна
        {
            await Anim.FadeInAsync(this, 1);
            await Anim.FadeInAsync(listBox1, 1);
        }
    }
}
