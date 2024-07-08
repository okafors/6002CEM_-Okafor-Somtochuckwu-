using Firebase;
using Firebase.Auth;
using Firebase.Database;
using TodoApp.Services;
using TodoApp.Views;
using FirebaseOptions = Firebase.FirebaseOptions;

namespace TodoApp
{
    public partial class App : Application
    {
        public static FirebaseClient FirebaseClient { get; private set; }
        public static FirebaseAuth Auth { get; private set; }

        public App()
        {
            InitializeComponent();
            InitializeFirebaseServices();
            MainPage = new NavigationPage(new LoginPage());
        }

        private void InitializeFirebaseServices()
        {
            try
            {
                // Ensure Firebase is initialized
                var options = new FirebaseOptions.Builder()
                    .SetApplicationId(FirebaseConfig.AppId)
                    .SetApiKey(FirebaseConfig.ApiKey)
                    .SetDatabaseUrl(FirebaseConfig.DatabaseUrl)
                    .SetProjectId(FirebaseConfig.ProjectId)
                    .Build();

                var app = FirebaseApp.InitializeApp(Android.App.Application.Context, options);

                // Initialize FirebaseAuth
                Auth = FirebaseAuth.GetInstance(app);

                // Initialize FirebaseClient (for Realtime Database)
                FirebaseClient = new FirebaseClient(FirebaseConfig.DatabaseUrl);

                System.Diagnostics.Debug.WriteLine("Firebase services initialized successfully.");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Firebase initialization error: {ex.Message}");
            }
        }

        protected override void OnStart()
        {
            base.OnStart();
        }
    }
}