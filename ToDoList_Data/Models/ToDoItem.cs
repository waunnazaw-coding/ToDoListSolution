using System;
using System.Collections.Generic;

namespace ToDoList_Data.Models;

public partial class ToDoItem
{
    public int TaskId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public bool? IsCompleted { get; set; }

    public DateTime? DueDate { get; set; }

    public DateTime? CreatedAt { get; set; }
}
