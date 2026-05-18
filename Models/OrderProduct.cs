using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;
using System.Text;

namespace E_Commerce_System.Models
{
    [PrimaryKey(nameof(OId), nameof(PId))]
    public class OrderProduct
    {
        [ForeignKey(nameof(Order))]
        public int OId { get; set; }
        public virtual Order Order { get; set; }

        [ForeignKey(nameof(Product))]
        public int PId { get; set; }
        public virtual Product Product { get; set; }




        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Quantity must be grater than 0")]
        public int Quantity { get; set; }

        
    }
}
