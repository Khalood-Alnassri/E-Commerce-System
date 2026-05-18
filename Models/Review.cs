using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace E_Commerce_System.Models
{
    
    
    public class Review
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RId { get; set; }

        [Required]
        [Range(1,5)]
        public int Rating { get; set; }

        public string? Comment { get; set; }

        public DateTime? ReviewDate { get; set; }

        [ForeignKey("User")]
        public int UId { get; set; }
        public virtual User User { get; set; }

        [ForeignKey("Product")]
        public int PId { get; set; }
        public virtual Product Product { get; set; }



    }
}
