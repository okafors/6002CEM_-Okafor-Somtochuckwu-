using System;
using System.Collections.ObjectModel;
using TodoApp.Services;
using TodoApp.ViewModels;

namespace TodoApp.Views
{
    public partial class CalendarPage : ContentPage
    {
        private DateTime currentMonth;
        private Dictionary<DateTime, List<TodoItem>> tasksByDate;

        public CalendarPage()
        {
            InitializeComponent();
            currentMonth = DateTime.Today;
            tasksByDate = new Dictionary<DateTime, List<TodoItem>>();
            LoadCalendar();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadTasks();
            UpdateCalendar();
        }

        private async Task LoadTasks()
        {
            // New code
            var databaseService = new DatabaseService();
            var allTasks = await databaseService.GetTodoItemsAsync();
            tasksByDate = allTasks.Where(t => t.DueDate.HasValue)
                                  .GroupBy(t => t.DueDate.Value.Date)
                                  .ToDictionary(g => g.Key, g => g.ToList());
        }

        private void LoadCalendar()
        {
            MonthYearLabel.Text = currentMonth.ToString("MMMM yyyy");

            var firstDayOfMonth = new DateTime(currentMonth.Year, currentMonth.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            int row = 1;
            int col = (int)firstDayOfMonth.DayOfWeek;

            for (var date = firstDayOfMonth; date <= lastDayOfMonth; date = date.AddDays(1))
            {
                var button = new Button
                {
                    Text = date.Day.ToString(),
                    CornerRadius = 0,
                    BorderWidth = 0.5,
                    BorderColor = Colors.Gray
                };

                if (tasksByDate.ContainsKey(date))
                {
                    button.BackgroundColor = Colors.Orange;
                }

                button.Clicked += async (sender, e) => await OnDateSelected(date);

                CalendarGrid.Add(button, col, row);

                col++;
                if (col == 7)
                {
                    col = 0;
                    row++;
                }
            }
        }

        private void UpdateCalendar()
        {
            foreach (var child in CalendarGrid.Children.Where(c => c is Button))
            {
                var button = (Button)child;
                var date = new DateTime(currentMonth.Year, currentMonth.Month, int.Parse(button.Text));
                button.BackgroundColor = tasksByDate.ContainsKey(date) ? Colors.Orange : Colors.Transparent;
            }
        }

        private async Task OnDateSelected(DateTime date)
        {
            if (tasksByDate.TryGetValue(date, out var tasks))
            {
                // If tasks exist for this date, show them in a list
                await DisplayTaskList(date, tasks);
            }
            else
            {
                // If no tasks exist, go directly to add task page
                await NavigateToAddTaskPage(date);
            }
        }

        private async Task DisplayTaskList(DateTime date, List<TodoItem> tasks)
        {
            var action = await DisplayActionSheet($"Tasks for {date.ToShortDateString()}", "Cancel", "Add New Task", tasks.Select(t => t.Name).ToArray());

            if (action == "Add New Task")
            {
                await NavigateToAddTaskPage(date);
            }
            else if (action != "Cancel")
            {
                var selectedTask = tasks.FirstOrDefault(t => t.Name == action);
                if (selectedTask != null)
                {
                    await NavigateToEditTaskPage(selectedTask);
                }
            }
        }

        private async Task NavigateToAddTaskPage(DateTime date)
        {
            var todoItem = new TodoItem
            {
                DueDate = date
            };
            var viewModel = new TodoItemViewModel();
            viewModel.SelectedTodoItem = todoItem;
            await Navigation.PushAsync(new TodoItemPage { BindingContext = viewModel });
        }

        private async Task NavigateToEditTaskPage(TodoItem todoItem)
        {
            var viewModel = new TodoItemViewModel();
            viewModel.SelectedTodoItem = todoItem;
            await Navigation.PushAsync(new TodoItemPage { BindingContext = viewModel });
        }

        private void OnPreviousMonthClicked(object sender, EventArgs e)
        {
            currentMonth = currentMonth.AddMonths(-1);
            CalendarGrid.Clear();
            LoadCalendar();
        }

        private void OnNextMonthClicked(object sender, EventArgs e)
        {
            currentMonth = currentMonth.AddMonths(1);
            CalendarGrid.Clear();
            LoadCalendar();
        }
    }
}