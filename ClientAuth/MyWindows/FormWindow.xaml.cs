using ClientHash.Services;
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
            _service = new DataService(new HttpClient());
        }

        private async void AddData(object sender, RoutedEventArgs e)
        {
            string value = formValue.Text;
            await _service.AddDataToServerAsync(
                "https://localhost:7218/api/data/write",
                value,
                _userService.CurrentUser.Login,
                _userService.CurrentUser.Password);
            DataAdded?.Invoke(this, EventArgs.Empty);
            Close();
        }
    }
}
