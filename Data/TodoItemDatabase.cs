using Firebase.Database;
using Firebase.Database.Query;

namespace TodoApp.Services
{
    public class DatabaseService
    {
        private readonly FirebaseClient _firebaseClient;

        public DatabaseService()
        {
            _firebaseClient = new FirebaseClient(FirebaseConfig.DatabaseUrl);
        }

        public async Task<List<TodoItem>> GetTodoItemsAsync()
        {
            var items = await _firebaseClient
                .Child("todoitems")
                .OnceAsync<TodoItem>();
            return items.Select(item =>
            {
                item.Object.ID = item.Key;
                return item.Object;
            }).ToList();
        }

        public async Task SaveTodoItemAsync(TodoItem item)
        {
            if (string.IsNullOrEmpty(item.ID))
            {
                var result = await _firebaseClient
                    .Child("todoitems")
                    .PostAsync(item);
                item.ID = result.Key;
            }
            else
            {
                await _firebaseClient
                    .Child("todoitems")
                    .Child(item.ID)
                    .PutAsync(item);
            }
        }

        public async Task DeleteTodoItemAsync(TodoItem item)
        {
            await _firebaseClient
                .Child("todoitems")
                .Child(item.ID)
                .DeleteAsync();
        }

        public async Task<List<Subtask>> GetSubtasksAsync(string todoItemId)
        {
            if (string.IsNullOrEmpty(todoItemId))
            {
                // Return an empty list if todoItemId is null or empty
                return new List<Subtask>();
            }

            try
            {
                var subtasks = await _firebaseClient
                    .Child("subtasks")
                    .Child(todoItemId)
                    .OnceAsync<Subtask>();
                return subtasks.Select(subtask =>
                {
                    subtask.Object.ID = subtask.Key;
                    return subtask.Object;
                }).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching subtasks: {ex.Message}");
                // Return an empty list in case of error
                return new List<Subtask>();
            }
        }

        public async Task SaveSubtaskAsync(Subtask subtask)
        {
            if (string.IsNullOrEmpty(subtask.ID))
            {
                var result = await _firebaseClient
                    .Child("subtasks")
                    .Child(subtask.TodoItemId)
                    .PostAsync(subtask);
                subtask.ID = result.Key;
            }
            else
            {
                await _firebaseClient
                    .Child("subtasks")
                    .Child(subtask.TodoItemId)
                    .Child(subtask.ID)
                    .PutAsync(subtask);
            }
        }

        public async Task DeleteSubtaskAsync(Subtask subtask)
        {
            await _firebaseClient
                .Child("subtasks")
                .Child(subtask.TodoItemId)
                .Child(subtask.ID)
                .DeleteAsync();
        }
    }
}