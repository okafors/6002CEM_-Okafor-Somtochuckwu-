using TodoApp.Services;

namespace TodoApp.Views
{
    public partial class LoginPage : ContentPage
    {
        private readonly AuthService _authService;

        public LoginPage()
        {
            InitializeComponent();
            _authService = new AuthService();
        }

        private async void OnLoginClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(EmailEntry.Text) || string.IsNullOrWhiteSpace(PasswordEntry.Text))
            {
                await DisplayAlert("Error", "Please enter both email and password.", "OK");
                return;
            }

            try
            {
                var token = await _authService.SignInWithEmailAndPassword(EmailEntry.Text, PasswordEntry.Text);
                await Navigation.PushAsync(new TodoListPage());
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }

        private async void OnSignUpClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SignUpPage());
        }

        private async void OnForgotPasswordClicked(object sender, EventArgs e)
        {
            string email = await DisplayPromptAsync("Forgot Password", "Enter your email address:");
            if (!string.IsNullOrWhiteSpace(email))
            {
                try
                {
                    await _authService.SendPasswordResetEmailAsync(email);
                    await DisplayAlert("Success", "Password reset email sent. Please check your inbox.", "OK");
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", ex.Message, "OK");
                }
            }
        }
    }
}