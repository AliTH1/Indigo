﻿using System.ComponentModel.DataAnnotations;

namespace Indigo.Models
{
    public class Post
    {
        public int Id { get; set; }
        [Required]
        public string ImagePath { get; set; }
        [Required, MaxLength(30)]
        public string Title { get; set; }
        [Required, MaxLength(60)]
        public string Description { get; set; }
        public bool IsDeleted { get; set; }
    }
}
