using ClientHash.Services;
using System.Configuration;
using System.Net.Http;
using System.Windows;

namespace ClientHash
{
    public partial class RegisterWindow : Window
    {
        public event Action<string, string, List<string>> SuccessfulRegistration;

        private readonly AuthService _authService;

        public RegisterWindow()
        {
            InitializeComponent();
            _authService = new AuthService(new HttpClient(), new AesEncryptionService());
        }

        private async void Register(object sender, RoutedEventArgs e)
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
                string urlRegister = ConfigurationManager.AppSettings["UrlRegister"];
                string urlLogin = ConfigurationManager.AppSettings["UrlLogin"];
                await _authService.AddUser(urlRegister, login, password);
                List<string> perms = await _authService.LoginUser(urlLogin, login, password);
                SuccessfulRegistration?.Invoke(login, AuthService.GetSHA1Hash(password), perms);
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

    }
}
