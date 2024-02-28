namespace ClientHash.Models
{
    public class User
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public List<string> Permissions { get; set; }
    }
}
