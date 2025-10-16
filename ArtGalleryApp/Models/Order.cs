using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ArtGalleryApp.Models
{
	public class Order
	{
        public Order()
        {
            Artworks = new List<Artwork>();
            OrderArtworks = new List<OrderArtwork>();
        }
        public int Id { get; set; }
        public int? UserId { get; set; }
        public User User { get; set; }
        [Required]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Use letters only please")]
        public String Name { get; set; }
        [Required]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Use letters only please")]
        public String Surname { get; set; }
        [Required]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public String Email { get; set; }
        [Required]
        public String PhoneNumber { get; set; }
        [Required]
        public String Address { get; set; }
        [Required]
        public String City { get; set; }
        [Required]
        public String Country { get; set; }
        public ICollection<Artwork> Artworks { get; set; }
        public String ShippingMethod { get; set; }
        public int ShippingPrice { get; set; }
        public int Total { get; set; }
        public DateTime WhenOrdered { get; set; }
        public ICollection<OrderArtwork> OrderArtworks { get; set; }
    }
}