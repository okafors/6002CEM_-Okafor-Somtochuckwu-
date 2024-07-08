using System;

using TodoApp.ViewModels;


namespace TodoApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TodoItemPage : ContentPage
    {
        public TodoItemPage(TodoItem todoItem = null)
        {
            InitializeComponent();

            var viewModel = new TodoItemViewModel();
            if (todoItem != null)
            {
                viewModel.SelectedTodoItem = todoItem;
            }
            else
            {
                viewModel.SelectedTodoItem = new TodoItem();
            }

            BindingContext = viewModel;

            MessagingCenter.Subscribe<TodoItemViewModel>(this, "TodoItemSaved", async (sender) =>
            {
                await DisplayAlert("Success", "Todo item saved successfully!", "OK");
                await Navigation.PopAsync();
            });
        }
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (BindingContext is TodoItemViewModel viewModel)
            {
                viewModel.LoadSubtasks();
            }
        }

        async void OnDeleteClicked(object sender, EventArgs e)
        {
            if (BindingContext is TodoItemViewModel viewModel)
            {
                bool deleted = await viewModel.DeleteTodoItemWithConfirmationAsync();
                if (deleted)
                {
                    await Navigation.PopAsync();
                }
            }
        }

        async void OnCancelClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}


