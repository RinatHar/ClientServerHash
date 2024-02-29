using ClientHash.Services;
using System.Configuration;
using System.Net.Http;
using System.Windows;

namespace ClientHash
{
    public partial class FormWindow : Window
    {
        private readonly DataService _service;
        private readonly UserService _userService;

        public event EventHandler DataAdded; // Событие, которое вызывается при добавлении данных

        public FormWindow(UserService userService)
        {
            InitializeComponent();
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _service = new DataService(new HttpClient(), new AesEncryptionService());
        }

        private async void AddData(object sender, RoutedEventArgs e)
        {
            string url = ConfigurationManager.AppSettings["UrlWriteData"];
            string value = formValue.Text;
            await _service.AddDataToServerAsync(
                url,
                value,
                _userService.CurrentUser.Login,
                _userService.CurrentUser.Password);
            DataAdded?.Invoke(this, EventArgs.Empty);
            Close();
        }
    }
}
