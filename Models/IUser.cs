namespace st10157545_giftgiversPOEs.Models
{
    public interface IUser
    {//for handled dynamically the common values for all users.
        string username { get; set; }
        string email { get; set; }
        string phone { get; set; }
        string password { get; set; }
        UserType UserType { get; }
    }
}
