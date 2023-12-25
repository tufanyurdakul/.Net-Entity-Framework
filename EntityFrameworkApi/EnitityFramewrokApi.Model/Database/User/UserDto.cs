using EntityFrameworkApi.Model.Shared.Database;
using System.ComponentModel.DataAnnotations;

namespace EntityFrameworkApi.Model.Database.User
{
    public class UserDto : DataRecordBase
    {
        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(250)]
        public string Password { get; set; }

        public short Age { get; set; }
        public DateTime BirthDate { get; set; }
        public bool IsMarried { get; set; }
        public char Gender { get; set; }
    }
}
