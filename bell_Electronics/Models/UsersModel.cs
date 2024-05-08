using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace bell_Electronics.Models
{
    public class UsersModel
    {
        [Key]
        public int id { get; set; }
        [Required, StringLength(100)]
        public string Name { get; set; }
        [Required, StringLength(100)]
        public string MobileNo { get; set; }

        [Required, StringLength(100)]
        public string Email { get; set; }

        [Required,StringLength(100)]
        public string UserName { get; set; }

        [Required, StringLength(100)]
        public string Password { get; set; }
    }

    public class tbl_Cart
    {
        [Key]
        public int Card_Id { get; set; }
        [Required]
        public int ProductsID { get; set;}
        [Required]
        public int Quantity { get; set; }
        public float Price { get; set; }
        public float total { get; set; }
        [Required]
        public string UserName { get; set; }
        public string ProductName { get; set; }
        public string Cartimg { get; set; }

    }
}