using Firebase.Auth;
using System;
using System.Threading.Tasks;

namespace TodoApp.Services
{
    public class AuthService
    {
        private FirebaseAuth GetFirebaseAuth()
        {
            if (App.Auth == null)
            {
                throw new InvalidOperationException("FirebaseAuth is not initialized. Please ensure Firebase is properly initialized.");
            }
            return App.Auth;
        }

        public async Task<string> SignUpWithEmailAndPassword(string email, string password)
        {
            try
            {
                var auth = GetFirebaseAuth();
                var authResult = await auth.CreateUserWithEmailAndPasswordAsync(email, password);
                return authResult.User.Uid;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SignUp error: {ex.Message}");
                throw;
            }
        }
        public async Task SendPasswordResetEmailAsync(string email)
        {
            try
            {
                var auth = GetFirebaseAuth();
                await auth.SendPasswordResetEmailAsync(email);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Password reset error: {ex.Message}");
                throw;
            }
        }

        public async Task<string> SignInWithEmailAndPassword(string email, string password)
        {
            try
            {
                var auth = GetFirebaseAuth();
                var authResult = await auth.SignInWithEmailAndPasswordAsync(email, password);
                return authResult.User.Uid;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SignIn error: {ex.Message}");
                throw;
            }
        }
    }
}