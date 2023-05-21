using System.Text;
using System;
using System.Windows;
using System.Threading.Tasks;
using System.Windows.Input;
using System.IO;
using LogicLibrary;

namespace GI
{
    /// <summary>
    /// Логика взаимодействия для RegWin.xaml
    /// </summary>
    public partial class RegWin : Window
    {
        public bool LOGReg = true;
        readonly string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "LogPas.txt");
        User user;
        public RegWin()
        {
            InitializeComponent();
        }

        private async void Login_TextBox_LostFocus(object sender, RoutedEventArgs e) // При потери фокуса с поля логина проверяем существование пользователя
        {
            try
            {
                if (await UserManager.GetUserAsync(Login_TextBox.Text) is null)
                    Dispatcher.Invoke(RegInterface);

                else
                    Dispatcher.Invoke(LogInterface);
            }

            // Обрабатываем исключение, если не будет получен ответ от сервера в течении 30 сек
            catch (Exception)
            {
                MessageBox.Show("Проверьте статус сервера БД", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
            }
        }

        private async Task Login_TextBox_LostFocus() // Перегрузка с возвращаемым типом Task
        {
            try
            {
                if (await UserManager.GetUserAsync(Login_TextBox.Text) is null)
                    Dispatcher.Invoke(RegInterface);

                else
                    Dispatcher.Invoke(LogInterface);
            }

            // Обрабатываем исключение, если не будет получен ответ от сервера в течении 30 сек
            catch (Exception)
            {
                MessageBox.Show("Проверьте статус сервера БД", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
            }
        }

        private void RegInterface() // Приведение интерфейса к регистрации
        {
            LOGReg = false;
            RegPasLabel.Visibility = Visibility.Visible;
            Password2_TextBox.Visibility = Visibility.Visible;
            ErorPassword_TextBlock.Visibility = Visibility.Collapsed;
            Log_Button.Content = "Зарегистрироваться";
        }

        private void LogInterface() // Приведение интерфейса ко входу
        {
            LOGReg = true;
            RegPasLabel.Visibility = Visibility.Collapsed;
            Password2_TextBox.Visibility = Visibility.Collapsed;
            ErorPassword2_TextBlock.Visibility = Visibility.Collapsed;
            Log_Button.Content = "Войти";
        }

        private async void Log_Button_ClickAsync(object sender, RoutedEventArgs e) // Кпока авторизации
        {
            await Login_TextBox_LostFocus();
            if (await ValidationCheck())
            {
                try
                {
                    if (!LOGReg) await UserManager.AddUserAsync(new User() { Login = Login_TextBox.Text, Password = Password_TextBox.Password });
                }

                // Обрабатываем исключение, если не будет получен ответ от сервера в течении 30 сек
                catch (Exception)
                {
                    MessageBox.Show("Проверьте статус сервера БД", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    Application.Current.Shutdown();
                }

                // Перезаписываем файл, если он уже существует
                using (StreamWriter writer = new StreamWriter(path, false, Encoding.Unicode))
                {
                    writer.WriteLine(Login_TextBox.Text);
                    writer.Write(Password_TextBox.Password);
                }

                MainWindow mainWindow = new MainWindow();
                await Anim.FadeOut2Async(this);
                mainWindow.Show(); // Выполняем авторизацию и переходим в настройки
                this.Close();
            }
        }

        private async Task<bool> ValidationCheck() // Проверка валидности
        {
            if (!LOGReg) // Проверка режма Вход / Регистрация
            {
                if ((Password_TextBox.Password != Password2_TextBox.Password) || (Password_TextBox.Password == "") || (Password2_TextBox.Password == "") || (Login_TextBox.Text == ""))
                    ErorPassword2_TextBlock.Visibility = Visibility.Visible; // Отображение ошибки неверного пароля

                return !((Password_TextBox.Password != Password2_TextBox.Password) || (Password_TextBox.Password == "") || (Password2_TextBox.Password == "") || (Login_TextBox.Text == ""));
            }

            else
            {
                try
                {
                    user = await UserManager.GetUserAsync(Login_TextBox.Text);
                }

                // Обрабатываем исключение, если не будет получен ответ от сервера в течении 30 сек
                catch (Exception)
                {
                    MessageBox.Show("Проверьте статус сервера БД", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    Application.Current.Shutdown();
                }

                if (Password_TextBox.Password != user.Password)
                    ErorPassword_TextBlock.Visibility = Visibility.Visible; // Отображение ошибки не совпадения пароля

                return Password_TextBox.Password == user.Password;
            }
        }

        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e) // Перемещение окна
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e) // После отрисовки окна
        {
            await Anim.FadeInAsync(this, 1);
        }

        private async void Image_MouseLeftButtonDownAsync(object sender, MouseButtonEventArgs e) // Закрытие окна по кнопке
        {
            await Anim.FadeOut2Async(this);
            Close();
        }
    }
}