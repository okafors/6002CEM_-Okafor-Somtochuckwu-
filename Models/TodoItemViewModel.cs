using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Linq;
using TodoApp.Services;
using Microsoft.Maui.Controls;

namespace TodoApp.ViewModels
{
    public class TodoItemViewModel : BaseViewModel
    {
        private readonly DatabaseService _databaseService;

        private ObservableCollection<TodoItem> _todoItems;
        public ObservableCollection<TodoItem> TodoItems
        {
            get => _todoItems;
            set
            {
                _todoItems = value;
                OnPropertyChanged();
            }
        }

        private TodoItem _selectedTodoItem;
        public TodoItem SelectedTodoItem
        {
            get => _selectedTodoItem;
            set
            {
                _selectedTodoItem = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<Subtask> _subtasks;
        public ObservableCollection<Subtask> Subtasks
        {
            get => _subtasks;
            set
            {
                _subtasks = value;
                OnPropertyChanged();
            }
        }

        public ICommand SaveTodoItemCommand { get; private set; }
        public ICommand DeleteTodoItemCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }
        public ICommand AddSubtaskCommand { get; private set; }
        public ICommand DeleteSubtaskCommand { get; private set; }

        public TodoItemViewModel()
        {
            _databaseService = new DatabaseService();
            TodoItems = new ObservableCollection<TodoItem>();
            SelectedTodoItem = new TodoItem();
            LoadTodoItems();

            SaveTodoItemCommand = new Command(async () => await SaveTodoItemAsync());
            DeleteTodoItemCommand = new Command(async () => await DeleteTodoItemAsync());
            AddSubtaskCommand = new Command(async () => await AddSubtaskAsync());
            DeleteSubtaskCommand = new Command<Subtask>(async (subtask) => await DeleteSubtaskAsync(subtask));
        }

        public async Task LoadTodoItems()
        {
            var items = await _databaseService.GetTodoItemsAsync();
            TodoItems = new ObservableCollection<TodoItem>(items);
        }

        private async Task AddSubtaskAsync()
        {
            if (SelectedTodoItem == null)
            {
                Console.WriteLine("SelectedTodoItem is null in AddSubtaskAsync");
                return;
            }

            string title = await Application.Current.MainPage.DisplayPromptAsync("New Subtask", "Enter subtask title:");
            if (string.IsNullOrWhiteSpace(title))
                return;

            string note = await Application.Current.MainPage.DisplayPromptAsync("New Subtask", "Enter subtask note (optional):");

            var newSubtask = new Subtask
            {
                Title = title,
                Note = note,
                TodoItemId = SelectedTodoItem.ID
            };

            if (SelectedTodoItem.Subtasks == null)
            {
                SelectedTodoItem.Subtasks = new List<Subtask>();
            }

            SelectedTodoItem.Subtasks.Add(newSubtask);
            await _databaseService.SaveSubtaskAsync(newSubtask);

            OnPropertyChanged(nameof(SelectedTodoItem));
        }

        private async Task DeleteSubtaskAsync(Subtask subtask)
        {
            if (SelectedTodoItem != null && subtask != null)
            {
                bool answer = await Application.Current.MainPage.DisplayAlert(
                    "Confirm Deletion",
                    "Are you sure you want to delete this subtask?",
                    "Yes", "No");

                if (answer)
                {
                    await _databaseService.DeleteSubtaskAsync(subtask);
                    SelectedTodoItem.Subtasks.Remove(subtask);
                    OnPropertyChanged(nameof(SelectedTodoItem));
                }
            }
        }

        public async Task<bool> DeleteTodoItemWithConfirmationAsync()
        {
            bool answer = await Application.Current.MainPage.DisplayAlert(
                "Confirm Deletion",
                "Are you sure you want to delete this task?",
                "Yes", "No");

            if (answer)
            {
                await DeleteTodoItemAsync();
                return true;
            }
            return false;
        }

        public async Task SaveTodoItemAsync()
        {
            if (SelectedTodoItem != null)
            {
                await _databaseService.SaveTodoItemAsync(SelectedTodoItem);

                if (SelectedTodoItem.Subtasks != null)
                {
                    foreach (var subtask in SelectedTodoItem.Subtasks)
                    {
                        subtask.TodoItemId = SelectedTodoItem.ID;
                        await _databaseService.SaveSubtaskAsync(subtask);
                    }
                }

                UpdateParentTaskCompletion();
                await LoadTodoItems();
                MessagingCenter.Send(this, "TodoItemSaved");
                MessagingCenter.Send(this, "CalendarUpdated");
            }
        }

        private void UpdateParentTaskCompletion()
        {
            if (SelectedTodoItem != null)
            {
                SelectedTodoItem.Done = SelectedTodoItem.Subtasks.All(s => s.Done);
            }
        }

        public async Task DeleteTodoItemAsync()
        {
            await _databaseService.DeleteTodoItemAsync(SelectedTodoItem);
            await LoadTodoItems();
        }

        public async void LoadSubtasks()
        {
            if (SelectedTodoItem != null)
            {
                if (string.IsNullOrEmpty(SelectedTodoItem.ID))
                {
                    // If it's a new item, initialize an empty list of subtasks
                    SelectedTodoItem.Subtasks = new List<Subtask>();
                }
                else
                {
                    SelectedTodoItem.Subtasks = await _databaseService.GetSubtasksAsync(SelectedTodoItem.ID);
                }
                OnPropertyChanged(nameof(SelectedTodoItem));
            }
        }

        public async Task SaveSubtasksAsync()
        {
            if (SelectedTodoItem == null)
            {
                Console.WriteLine("SelectedTodoItem is null in SaveSubtasksAsync");
                return;
            }

            if (SelectedTodoItem.Subtasks == null)
            {
                Console.WriteLine("SelectedTodoItem.Subtasks is null in SaveSubtasksAsync");
                return;
            }

            foreach (var subtask in SelectedTodoItem.Subtasks)
            {
                if (subtask == null)
                {
                    Console.WriteLine("A subtask is null in SaveSubtasksAsync");
                    continue;
                }
                subtask.TodoItemId = SelectedTodoItem.ID;
                await _databaseService.SaveSubtaskAsync(subtask);
            }
        }
    }
}