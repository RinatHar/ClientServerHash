namespace ClientHash.Models
{
    public class User
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public string[] Permissions { get; set; }
    }
}
