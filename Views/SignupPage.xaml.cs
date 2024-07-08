using TodoApp.Services;

namespace TodoApp.Views
{
    public partial class SignUpPage : ContentPage
    {
        private readonly AuthService _authService;

        public SignUpPage()
        {
            InitializeComponent();
            _authService = new AuthService();
        }

        private async void OnSignUpClicked(object sender, EventArgs e)
        {
            if (PasswordEntry.Text != ConfirmPasswordEntry.Text)
            {
                await DisplayAlert("Error", "Passwords do not match", "OK");
                return;
            }

            try
            {
                var token = await _authService.SignUpWithEmailAndPassword(EmailEntry.Text, PasswordEntry.Text);
                await Navigation.PushAsync(new TodoListPage());
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }
    }
}