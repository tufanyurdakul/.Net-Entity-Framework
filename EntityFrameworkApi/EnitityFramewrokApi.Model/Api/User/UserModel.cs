
using EntityFrameworkApi.Model.Attributes;

namespace EntityFrameworkApi.Model.Api.User
{
    public class UserModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public DateTime BirthDate { get; set; }
        public bool IsMarried { get; set; }
        public string Gender { get; set; }

    }

    public class UserSignModel
    {
        [RequiredParameterAttributes(3, 50)]
        public string Name { get; set; }

        [RequiredParameterAttributes(7, 15)]
        public string Password { get; set; }

        [RequiredParameterAttributes(0, 200)]
        public int Age { get; set; }

        public DateTime BirthDate { get; set; }
        public bool IsMarried { get; set; }

        [RequiredParameterAttributes(0, 5)]
        public string Gender { get; set; }
    }

    public class UserUpdateModel
    {
        [RequiredParameterAttributes(3, 50)]
        public string Name { get; set; }

        [RequiredParameterAttributes(0, 200)]
        public int Age { get; set; }

        public DateTime BirthDate { get; set; }
        public bool IsMarried { get; set; }

        [RequiredParameterAttributes(0, 5)]
        public string Gender { get; set; }
    }

    public class UserLoginModel
    {
        [RequiredParameterAttributes(3, 50)]
        public string Name { get; set; }

        [RequiredParameterAttributes(7, 15)]
        public string Password { get; set; }
    }
}
