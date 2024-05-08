using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace bell_Electronics.Models
{
    public class tbl_Category
    {
        [Key]
        public int CategoryId { get; set; }
        [Required]
        [StringLength(100)]
        public string CategoryName { get; set; }
        [Required]
        [StringLength(100)]
        public string imageURL { get; set; }
        [Required]
        public int Cate_Status {  get; set; }

     
    }
}




