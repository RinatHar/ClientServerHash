using System.ComponentModel.DataAnnotations.Schema;

namespace ServerHash.Models
{
    [Table("users_rights")]
    public class UserRight
    {
        public int Id { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }
        public User User { get; set; }

        [Column("right_id")]
        public int RightId { get; set; }
        public Right Right { get; set; }
    }
}
