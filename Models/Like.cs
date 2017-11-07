using System;
using System.ComponentModel.DataAnnotations;

namespace BrightIdeas.Models
{
    public class Like : BaseEntity {
        // [Key]
        // public int id {get;set;}

        [Required]
        public int user_id {get;set;}
        public int post_id {get;set;}

        public User user {get; set;}
        public Post post {get; set;}
    }
}