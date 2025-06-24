namespace TodoApi.DTOs
{
    // Data Transfer Object for creating and updating To-Do items.
    public class TodoItemDto
    {
        public int Id { get; set; }
        public string Task { get; set; }
        public bool IsDone { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
