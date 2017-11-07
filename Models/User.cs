using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BrightIdeas.Models
{
    public abstract class BaseEntity {}
    public class User : BaseEntity
    {
        [Key]
        public int id {get; set;}
        
        [Required]
        [MinLength(3, ErrorMessage = "Minimum 3 characters required.")]
        public string name { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "Minimum 1 characters required.")]
        public string alias { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Please enter an email address.")]
        public string email { get; set; }

        [Required]
        [StringLength(255, ErrorMessage = "Your password must be at least 10 characters in length.", MinimumLength = 10)]
        [DataType(DataType.Password)]
        public string password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("password", ErrorMessage = "The passwords do not match.  Try again.")]
        public string confirmation { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }

        // public List<Like> likes {get;set;}
    }
}