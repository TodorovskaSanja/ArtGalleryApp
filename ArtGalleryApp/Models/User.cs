using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ArtGalleryApp.Models
{
	public class User
	{
        public User()
        {
            Tickets = new List<Ticket>();
            Events = new List<Event>();
            Cart = new List<Artwork>();
            PurchasedArtworks = new List<Artwork>();
            FavoriteArtworks = new List<Artwork>();
            FollowedArtists = new List<Artist>();
            Orders = new List<Order>();
        }

        public int Id { get; set; }
        [Required]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Use letters only please")]
        public String Name { get; set; }
        [Required]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Use letters only please")]
        public String Surname { get; set; }
        [Required]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public String Email { get; set; }
        [Display(Name="Profile picture")]
        public String ImageURL { get; set; }
        public ICollection<Ticket> Tickets { get; set; }
        public ICollection<Event> Events { get; set; }
        public ICollection<Artwork> Cart { get; set; }
        public ICollection<Artwork> PurchasedArtworks { get; set; }
        public ICollection<Artwork> FavoriteArtworks { get; set; }
        public ICollection<Artist> FollowedArtists { get; set; }
        public ICollection<Order> Orders { get; set; }
    }
}