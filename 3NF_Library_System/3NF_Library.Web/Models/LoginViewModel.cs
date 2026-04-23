namespace _3NF_Library.Web.Models
{
    public class LoginViewModel
    {
        public string AccountType { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string? SecretCode { get; set; } // Mã bảo mật cấp 2 (có thể rỗng)
    }
}