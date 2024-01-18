namespace CSE3PAX
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string UserGuid { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public bool IsAdmin { get; set; } = false;
        public bool isManager { get; set; } = false;
        public bool isLecturer { get; set; } = false;
        public bool isPasswordRestRequired { get; set; } = false;

    }
}
