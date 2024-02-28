using ClientHash.Models;

namespace ClientHash.Services
{
    public class UserService
    {
        // Событие, которое будет вызываться при изменении пользователя
        public event Action<User> UserChanged;

        private User _currentUser;

        public User CurrentUser
        {
            get => _currentUser;
            set
            {
                _currentUser = value;
                // Вызываем событие при изменении пользователя
                UserChanged?.Invoke(value);
            }
        }
    }
}