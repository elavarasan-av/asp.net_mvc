using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace bell_Electronics.Models
{
    public class Products
    {
        [Key]
        public int ProductsID { get; set; }
        [StringLength(100)]
        public string ProductName { get; set; }
        [StringLength(300)]
        public string Description { get; set; }
        [Required]
        public int CategoryId { get; set; }
        [Required]
        public double Price { get; set; }
        [Required]
        public string PrdimageUrl { get; set; }
       
        [Required]
        public int Prod_Status { get; set; }

        [StringLength(100)]
        public string CategoryName { get; set; }
    }
}