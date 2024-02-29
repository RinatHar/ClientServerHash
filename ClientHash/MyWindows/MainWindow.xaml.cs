using ClientHash.Models;
using ClientHash.Services;
using System.Configuration;
using System.Net.Http;
using System.Windows;

namespace ClientHash
{
    public partial class MainWindow : Window
    {
        private readonly DataService _service;
        private readonly UserService _userService;

        public MainWindow(UserService userService)
        {
            InitializeComponent();
            _service = new DataService(new HttpClient(), new AesEncryptionService());
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));

            // Подписываемся на событие изменения пользователя
            _userService.UserChanged += HandleUserChanged;

            CheckPermissionsAndAddButton();
            LoadData();
         }

        private void HandleUserChanged(User newUser)
        {
            CheckPermissionsAndAddButton();
        }


        public async void LoadData()
        {
            string url = ConfigurationManager.AppSettings["UrlReadData"];
            var data = await _service.GetDataFromServerAsync (
                url,
                _userService.CurrentUser.Login, 
                _userService.CurrentUser.Password);
            MyData.ItemsSource = data;
        }

        private void CheckPermissionsAndAddButton()
        {
            // Получаем текущего пользователя из сервиса
            User currentUser = _userService.CurrentUser;

            // Используем данные пользователя, например, проверяем наличие права "WRITE"
            if (currentUser != null && currentUser.Permissions.Contains("WRITE"))
            {
                // Показываем кнопку "Добавить"
                addValue.Visibility = Visibility.Visible;
            }
            else
            {
                // Скрываем кнопку "Добавить"
                addValue.Visibility = Visibility.Collapsed;
            }
        }

        private void OpenFormWindow(object sender, RoutedEventArgs e)
        {
            FormWindow formWindow = new(_userService);
            formWindow.DataAdded += OnDataAdded; // Подписываемся на событие
            formWindow.Show();
        }

        private void OnDataAdded(object sender, EventArgs e)
        {
            LoadData(); // Обновляем данные после добавления новых данных
        }

        private void OpenLogInWindow(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWindow = new();
            loginWindow.Show();
            this.Close();
        }

    }
}
