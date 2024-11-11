﻿using System.ComponentModel.DataAnnotations;

namespace Talabat.Apis.DTOs
{
    public class BasketItemDto
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string productName { get; set; }
        [Required]
        public string PictureUrl { get; set; }
        [Required]
        public string Brand { get; set; }
        [Required]
        public string Type { get; set; }
        [Required]
        [Range(0.1 , double.MaxValue , ErrorMessage ="Price can not be zero ")]
        public decimal Price { get; set; }
        [Required]
        [Range(1 , int.MaxValue , ErrorMessage ="Quantity must be at least One Item ")]
        public int Quantity { get; set; }
    }
}