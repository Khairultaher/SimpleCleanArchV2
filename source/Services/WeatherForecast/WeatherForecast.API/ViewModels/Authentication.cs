namespace WeatherForecast.API.ViewModels
{
    public class AuthenticationModel
    {
        public List<User> Users = new List<User>();
        public AuthenticationModel()
        {
            Users.Add(new User() { UserName = "Khairul", Pasword = "123", Depertment = "IT", Roles = new List<string>() { "Admin", "User" } });
            Users.Add(new User() { UserName = "Alam", Pasword = "123", Depertment = "HR", Roles = new List<string>() { "Admin" } });
            Users.Add(new User() { UserName = "Taher", Pasword = "123", Depertment = "Accounts", Roles = new List<string>() { "Admin" } });
        }

        public User Login(string username = "", string password = "")
        {
            return Users.FirstOrDefault(w => w.UserName.ToLower() == username.ToLower()
                                && w.Pasword == password);
        }
        public class User
        {
            public string? UserName { get; set; }
            public string? Pasword { get; set; }
            public List<string>? Roles { get; set; }
            public string? Depertment { get; set; }

            public string? RefreshToken { get; set; }
            public DateTime RefreshTokenExpiryTime { get; set; }
        }
    }
}
