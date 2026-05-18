using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace E_Commerce_System.Models
{
    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OId { get; set; }

        public DateTime? OrderDate { get; set; }

        [NotMapped]
        public decimal TotalAmount
        {
            get
            {
                return OrderProducts.Sum(op => op.Quantity * op.Product.Price);
            }
        }

        [ForeignKey("User")]
        public int UId { get; set; }
        public virtual User User { get; set; }

        public virtual ICollection<OrderProduct> OrderProducts { get; set; }





    }
}
