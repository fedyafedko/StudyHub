﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace StudyHub.Entities;

public class Teacher 
{
    [Key, ForeignKey(nameof(User))]
    public Guid UserId { get; set; }
    public string Telegram { get; set; } = string.Empty;
    public User User { get; set; } = null!;

    public List<Subject> Subjects { get; set; } = null!;
}
