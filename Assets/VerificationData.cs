[System.Serializable]
public class VerificationData
{
    public string userId;
    public string verificationCode;

    public VerificationData(string userId, string verificationCode)
    {
        this.userId = userId;
        this.verificationCode = verificationCode;
    }
}