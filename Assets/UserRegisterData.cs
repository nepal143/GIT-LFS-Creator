[System.Serializable]
public class UserRegisterData
{
    public string username;
    public string phoneNumber;
    public string password;

    public UserRegisterData(string username, string phoneNumber, string password)
    {
        this.username = username;
        this.phoneNumber = phoneNumber;
        this.password = password;
    }
}
