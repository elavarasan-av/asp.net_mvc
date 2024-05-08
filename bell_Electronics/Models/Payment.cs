using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace bell_Electronics.Models
{
    public class Payment
    {
        [Key]
        public int PyId { get; set; }

        [Required,StringLength(100)]
        public string name { get; set; }

        [Required, StringLength(100)]
        public string Name1 { get; set; }

        [Required, StringLength(100)]
        public string CardNo { get; set; }

        [Required, StringLength(100)]
        public string ExpDate { get; set; }

        [Required]
        public int CVVno { get; set; }

        [Required, StringLength(100)]
        public string Address { get; set; } 

        [Required, StringLength(100)]
        public string CardAddress { get; set; }

        public string PaymentId { get; set; }
        public string paymentMode { get; set; }
    }
    public class Order
    {
        [Key]
        [Required]
        public int OrderId { get; set; }

        [Required,StringLength(300)]
        public string OrderNo { get; set; }

        [Required]
        public int ProductsId { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required, StringLength(100)]
        public string UserName { get; set; }

        [Required, StringLength(100)]
        public string ProductsName { get; set; }

        [Required, StringLength(100)]
        public string Status { get; set; }

        [Required]
        public string PaymentId { get; set; }

        public string OrderDate { get; set; }
        [Required, StringLength(100)]
        public string PrdimageUrl { get; set; }

    }

    public class Invoice
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(300)]
        public string OrderNo { get; set; }

        [Required, StringLength(100)]
        public string ProductsName { get; set; }

        [Required]
        public int Price { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public int TotalPrice { get; set; }

        [Required, StringLength(100)]
        public string Status { get; set; }

        public string OrderDate { get; set; }
    }

    public class OrderStatus
    {
        public int Id { get; set; }
        public string Status { get; set; }
    }
}