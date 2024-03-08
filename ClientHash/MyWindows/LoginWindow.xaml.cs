using ClientHash.Models;
using ClientHash.Services;
using System.Configuration;
using System.Net.Http;
using System.Windows;

namespace ClientHash
{
    public partial class LoginWindow : Window
    {
        private readonly AuthService _authService;
        private readonly UserService _userService;

        public LoginWindow()
        {
            InitializeComponent();
            _authService = new AuthService();
            _userService = new UserService();

        }

        private async void Login(object sender, RoutedEventArgs e)
        {
            // Получаем логин и пароль из нового окна
            string login = loginValue.Text;
            string password = passValue.Text;

            // Проверка на null или пустую строку логина и пароля
            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Логин и пароль не могут быть пустыми.");
                return;
            }

            try
            {
                string url = ConfigurationManager.AppSettings["UrlLogin"];
                string[] perms = await _authService.LoginUser(url, login, password);
                MessageBox.Show("Вход выполнен.");

                string hashedPass = AuthService.GetSHA1Hash(password);

                OpenMainWindow(login, hashedPass, perms);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void OpenRegisterWindow(object sender, RoutedEventArgs e)
        {
            RegisterWindow registerWindow = new();
            registerWindow.SuccessfulRegistration += OpenMainWindow;
            registerWindow.Show();
        }

        private void OpenMainWindow(string login, string hashedPass, string[] perms)
        {
            _userService.CurrentUser = new User
            {
                Login = login,
                Password = hashedPass,
                Permissions = perms
            };

            MainWindow mainWindow = new(_userService);
            mainWindow.Show();
            Close();
        }
    }
}
