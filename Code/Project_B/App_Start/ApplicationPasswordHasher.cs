using Microsoft.AspNet.Identity;

namespace Project_B {
    public class ApplicationPasswordHasher : IPasswordHasher {
        public string HashPassword(string password) {
            return password;
        }

        public PasswordVerificationResult VerifyHashedPassword(string hashedPassword, string providedPassword) {
            return hashedPassword.Equals(providedPassword)
                ? PasswordVerificationResult.Success
                : PasswordVerificationResult.Failed;
        }
    }
}