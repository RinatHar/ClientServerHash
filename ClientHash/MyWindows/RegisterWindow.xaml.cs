using ClientHash.Services;
using System.Net.Http;
using System.Windows;

namespace ClientHash
{
    public partial class RegisterWindow : Window
    {
        private readonly AuthService _authService;

        public RegisterWindow()
        {
            InitializeComponent();
            _authService = new AuthService(new HttpClient());
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
                await _authService.AddUser(login, password);
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

    }
}
