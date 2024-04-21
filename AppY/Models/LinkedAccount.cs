﻿using System.ComponentModel.DataAnnotations.Schema;

namespace AppY.Models
{
    public class LinkedAccount : Base
    {
        public string? CodeName { get; set; }
        [ForeignKey("User")]
        public int UserId { get; set; }
        public User? User { get; set; }
    }
}