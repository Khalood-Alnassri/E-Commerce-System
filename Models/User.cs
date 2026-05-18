using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Text.RegularExpressions;

namespace E_Commerce_System.Models
{
    [Index(nameof(Email), IsUnique = true)]
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int UId { get; set; }
        [Required]
        public string UName { get; set; }

        [Required]
        [EmailAddress]
        [RegularExpression(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        [Required]
        [RegularExpression(
      @"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d).{8,}$",
      ErrorMessage = "Password must contain at least 8 characters, one uppercase letter, one lowercase letter, and one number.")]

        public string Password { get; set; }

        [Required]
        public string Phone { get; set; }

        [Required]
        public string Role { get; set; }

        public DateTime? CreatedAt { get; set; }

        public virtual ICollection<Order> Orders { get; set; }

        public virtual ICollection<Review> Reviews { get; set; }







    }
}
