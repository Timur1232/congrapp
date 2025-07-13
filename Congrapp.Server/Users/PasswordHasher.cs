using System.Security.Cryptography;

namespace Congrapp.Server.Users;

public interface IPasswordHasher
{
    public string Hash(string password);
    public bool Varify(string password, string passwordHash);
}

public class PasswordHasher : IPasswordHasher
{
    private const int SaltSize = 16;
    private const int HashSize = 32;
    private const int Iterations = 100000;
    
    private readonly HashAlgorithmName _algorithm = HashAlgorithmName.SHA256;
    
    public string Hash(string password)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, _algorithm, HashSize);
        return $"{Convert.ToBase64String(hash)}-{Convert.ToBase64String(salt)}";
    }

    public bool Varify(string password, string passwordHash)
    {
        string[] pair = passwordHash.Split('-');
        byte[] hash = Convert.FromBase64String(pair[0]);
        byte[] salt = Convert.FromBase64String(pair[1]);
        byte[] inputHash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, _algorithm, HashSize);
        return CryptographicOperations.FixedTimeEquals(hash, inputHash);
    }
}