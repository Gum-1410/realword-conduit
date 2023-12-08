namespace RealWorldConduit.Application.Users.DTOs
{
    internal class ProfileDTO
    {
        public string Slug { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Bio { get; set; }
        public string ProfileImage { get; set; }
        public bool Following { get; set; }
    }
}
