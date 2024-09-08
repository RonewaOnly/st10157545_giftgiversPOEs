namespace ST10157545_GIFTGIVERS.Models
{
    public interface IUser
    {//for handled dynamically the common values for all users.
        string Username { get; set; }
        string Email { get; set; }
        string Phone { get; set; }
        string Password { get; set; }
        UserType UserType { get; }
    }
}
