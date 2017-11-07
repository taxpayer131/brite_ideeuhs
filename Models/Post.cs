using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BrightIdeas.Models
{
    public class Post : BaseEntity
    {
        [Key]
        public int id {get; set;}
        
        [Required]
        [MinLength(1, ErrorMessage = "A post must contain text or numbers.")]
        public string content { get; set; }
        public DateTime created_at { get; set; }
        public int user_id {get;set;}

        public User user {get;set;}
        public List<Like> likes {get;set;}
    }
}