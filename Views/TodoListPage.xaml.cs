using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using TodoApp.Services;
using TodoApp.ViewModels;

namespace TodoApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TodoListPage : ContentPage
    {
        public TodoListPage()
        {
            InitializeComponent();
        }
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await RefreshTodoItems();
        }

        async Task RefreshTodoItems()
        {
            // New code
            var databaseService = new DatabaseService();
            // New code
            var items = await databaseService.GetTodoItemsAsync();
            var sortedItems = items.OrderByDescending(item => item.IsPriority).ToList();
            listView.ItemsSource = sortedItems;
        }

        async void OnItemAdded(object sender, EventArgs e)
        {
            var newItem = new TodoItem { Name = "New Item" };
            var viewModel = new TodoItemViewModel();
            viewModel.SelectedTodoItem = newItem;
            await Navigation.PushAsync(new TodoItemPage { BindingContext = viewModel });
        }

        async void OnCalendarClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CalendarPage());
        }

        async void OnListItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                var todoItem = e.SelectedItem as TodoItem;
                var viewModel = new TodoItemViewModel();
                viewModel.SelectedTodoItem = todoItem;
                await Navigation.PushAsync(new TodoItemPage { BindingContext = viewModel });
                ((ListView)sender).SelectedItem = null;
            }
        }

        private async void OnModeToggled(object sender, ToggledEventArgs e)
        {
            if (e.Value)
            {
                // Dark mode
                App.Current.Resources["BackgroundColor"] = Color.FromHex("#808080"); // Dark gray background
                App.Current.Resources["TextColor"] = Color.FromHex("#FFFFFF");
                App.Current.Resources["ItemTextColor"] = Color.FromHex("#000000");
            }
            else
            {
                // Light mode
                App.Current.Resources["BackgroundColor"] = Color.FromHex("#FFFFFF"); // White background
                App.Current.Resources["TextColor"] = Color.FromHex("#000000");
                App.Current.Resources["ItemTextColor"] = Color.FromHex("#333333");
            }
        }
    }
}
