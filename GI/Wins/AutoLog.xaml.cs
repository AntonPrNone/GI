using System;
using System.IO;
using System.Windows;
using LogicLibrary;

namespace GI
{
    /// <summary>
    /// Логика взаимодействия для AutoLog.xaml
    /// </summary>
    public partial class AutoLog : Window
    {
        // Путь к файлу, содержащему логин и пароль пользователя
        readonly string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "LogPas.txt");
        readonly string login;
        readonly string password;

        public AutoLog()
        {
            InitializeComponent();
            Hide();

            // Если файл существует и содержит не менее двух строк
            if (File.Exists(path) && File.ReadAllLines(path).Length >= 2)
            {
                string[] fileContents = File.ReadAllLines(path);
                login = fileContents[0];
                password = fileContents[1];
                ValidationCheck();
            }

            else
            {
                RegWin();
                Close();
            }
        }

        private async void ValidationCheck() // Метод проверки валидности логина и пароля пользователя
        {
            try
            {
                User user = await UserManager.GetUserAsync(login);

                // Если пользователь существует и пароль совпадает, открываем окно настроек
                if (user != null && password == user.Password)
                {
                    MainWindow mainWindow = new MainWindow();
                    mainWindow.Show();
                }

                // Иначе открываем окно авторизации
                else
                {
                    RegWin();
                }

                Close();
            }

            // Обрабатываем исключение, если не будет получен ответ от сервера в течении 30 сек
            catch (Exception)
            {
                MessageBox.Show("Проверьте статус сервера БД", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
            }
        }

        private void RegWin()
        {
            RegWin regWin = new RegWin();
            regWin.Show();
        }
    }
}
