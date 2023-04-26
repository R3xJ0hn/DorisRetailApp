namespace DorisApp.Data.Library.Model
{
    public class AuthenticatedUserModel
    {
        public string? Access_Token { get; set; }
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Status { get; set; }
    }
}
