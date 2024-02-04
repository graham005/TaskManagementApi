namespace TaskManagementApi.Data_Models
{
    public class Task
    {
            public int Id { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public DateTime DueDate { get; set; }
            public int Priority { get; set; }
            public string Status { get; set; }
        
    }
}
