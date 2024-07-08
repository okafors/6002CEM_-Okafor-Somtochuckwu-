using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using TodoApp.Services;
using Microsoft.Maui.Controls;
using System.Linq;

namespace TodoApp.ViewModels
{
    public class CalendarViewModel : BaseViewModel
    {
        private DateTime _selectedDate;
        public DateTime SelectedDate
        {
            get => _selectedDate;
            set
            {
                _selectedDate = value;
                OnPropertyChanged();
                LoadTasksForSelectedDate();
            }
        }

        private ObservableCollection<TodoItem> _tasksForSelectedDate;
        public ObservableCollection<TodoItem> TasksForSelectedDate
        {
            get => _tasksForSelectedDate;
            set
            {
                _tasksForSelectedDate = value;
                OnPropertyChanged();
            }
        }

        private TodoItem _selectedTask;
        public TodoItem SelectedTask
        {
            get => _selectedTask;
            set
            {
                _selectedTask = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<DateTime> TaskDates { get; set; }

        public ICommand DateSelectedCommand { get; }
        public ICommand AddTaskCommand { get; }

        public CalendarViewModel()
        {
            SelectedDate = DateTime.Today;
            TasksForSelectedDate = new ObservableCollection<TodoItem>();
            TaskDates = new ObservableCollection<DateTime>();

            DateSelectedCommand = new Command<DateTime>(date => SelectedDate = date);
            AddTaskCommand = new Command(async () => await AddTask());

            LoadTaskDates();

            MessagingCenter.Subscribe<TodoItemViewModel>(this, "CalendarUpdated", (sender) =>
            {
                LoadTaskDates();
                LoadTasksForSelectedDate();
            });
        }

        private async void LoadTaskDates()
        {
            // New code
            var databaseService = new DatabaseService();
            var allTasks = await databaseService.GetTodoItemsAsync();
            TaskDates.Clear();
            foreach (var task in allTasks.Where(t => t.DueDate.HasValue))
            {
                TaskDates.Add(task.DueDate.Value.Date);
            }
            OnPropertyChanged(nameof(TaskDates));
        }

        private async void LoadTasksForSelectedDate()
        {
            // New code
            var databaseService = new DatabaseService();
            var allTasks = await databaseService.GetTodoItemsAsync();
            var tasksForDate = allTasks.Where(t => t.DueDate?.Date == SelectedDate.Date).ToList();
            TasksForSelectedDate = new ObservableCollection<TodoItem>(tasksForDate);
        }

        private async Task AddTask()
        {
            var newTask = new TodoItem
            {
                Name = "New Task",
                DueDate = SelectedDate
            };

            // New code
            var databaseService = new DatabaseService();
            await databaseService.SaveTodoItemAsync(newTask);

            TasksForSelectedDate.Add(newTask);
            if (!TaskDates.Contains(SelectedDate.Date))
            {
                TaskDates.Add(SelectedDate.Date);
                OnPropertyChanged(nameof(TaskDates));
            }
        }
    }
}