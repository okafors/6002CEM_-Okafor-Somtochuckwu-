public class TodoItem
{
    public string ID { get; set; }
    public string Name { get; set; }
    public string Notes { get; set; }
    public bool Done { get; set; }
    public DateTime? DueDate { get; set; }
    public bool IsPriority { get; set; }
    public List<Subtask> Subtasks { get; set; }

    public TodoItem()
    {
        Subtasks = new List<Subtask>();
    }
}