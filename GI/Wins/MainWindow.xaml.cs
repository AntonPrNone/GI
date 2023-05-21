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
		CharactersManager charactersManager = new CharactersManager();
		ImageManager imageManager = new ImageManager();
		List<CharacterDocument> characters;
		List<CharacterDocument> charactersActual;
		List<CharacterDocument> charactersPl;
		List<CharacterDocument> charactersNoPl;
		List<CharacterDocument> charactersFav;
		string directoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "GI", "imgs", "pers2");
		string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "LogPas.txt");
		string username;
		bool initial = true;
		ListBox listBox;
		User user;
		List<string> favoriteСharactersList;
		string pathFavEmpty = @"/imgs/favoriteEmpty.png";
		string pathFav = @"/imgs/favorite.png";
		ImageSource imageSourceFavEmpty = new BitmapImage(new Uri(@"/imgs/favoriteEmpty.png", UriKind.Relative));
		ImageSource imageSourceFav = new BitmapImage(new Uri(@"/imgs/favorite.png", UriKind.Relative));

		public MainWindow()
		{
			InitializeComponent();
			listBox = listBox1;
			listBox.Opacity = 0;
			username = File.ReadAllLines(path)[0];

			LoadData();
		}

		private async Task UpdateData()
		{
			user = await UserManager.GetUserAsync(username);
			favoriteСharactersList = user.FavoriteСharacters;
			Username_TextBlock.Text = username;
			characters = await charactersManager.LoadCharacterAsync();
			charactersPl = (List<CharacterDocument>)(listBox.ItemsSource = characters.Where(character => character.Category == "Играбельный").ToList());
			charactersNoPl = (List<CharacterDocument>)(listBox.ItemsSource = characters.Where(character => character.Category == "Неиграбельный").ToList());
			charactersActual = charactersPl;
			await imageManager.LoadImageFromDbAsync();

			foreach (string favoriteCharacterName in favoriteСharactersList)
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

			foreach (CharacterDocument character in characters)
			{
				if (character.FavoriteСharacters == null || character.FavoriteСharacters == "")
					character.FavoriteСharacters = pathFavEmpty;
				character.Photo = Path.Combine(directoryPath, Path.ChangeExtension(character.Name, ".png"));
			}

			charactersFav = (List<CharacterDocument>)(listBox.ItemsSource = characters.Where(character => character.FavoriteСharacters == pathFav).ToList());

			listBox.ItemsSource = charactersActual;
			await Anim.FadeInAsync(listBox, 1);
		}

		private async void LoadData()
		{
			var fadeInTasks = new Task[]
			{
				Anim.FadeInAsync(LoadingCircle, 1.5),
				Anim.FadeInAsync(Ldg_TextBlock, 1.5)
			};

			await UpdateData();

			var fadeOutTasks = new Task[]
			{
				Anim.FadeOutAsync(LoadingCircle, 1),
				Anim.FadeOutAsync(Ldg_TextBlock, 1)
			};
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

		private void TextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
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

		private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (!initial)
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

		private void Image_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
		{
			CharacterDocument selectedCharacter = listBox.SelectedItem as CharacterDocument;
			if (selectedCharacter != null)
			{
				if (user.FavoriteСharacters.Contains(selectedCharacter.Name))
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
						listBox3.ItemsSource = charactersActual;
						listBox2.ItemsSource = null;
						listBox2.ItemsSource = charactersNoPl;
						listBox1.ItemsSource = null;
						listBox1.ItemsSource = charactersPl;
					}
				}

				else
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
						listBox3.ItemsSource = charactersActual;
						listBox2.ItemsSource = null;
						listBox2.ItemsSource = charactersNoPl;
						listBox1.ItemsSource = null;
						listBox1.ItemsSource = charactersPl;
					}
				}
			}
		}

        // --------------------------------- Взаимодействие с окном --------------------------------------------

        private async void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            await UserManager.ReplaceUserAsync(user);
            File.WriteAllText(path, string.Empty);
            RegWin regWin = new RegWin();
            regWin.Show();
            await Anim.FadeOut2Async(this);
            Close();
        }

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

        private async void Window_Closed(object sender, EventArgs e)
		{
			await UserManager.ReplaceUserAsync(user);
		}

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await Anim.FadeInAsync(this, 1);
        }
    }
}
