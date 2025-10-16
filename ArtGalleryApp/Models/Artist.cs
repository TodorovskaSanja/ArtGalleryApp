using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ArtGalleryApp.Models
{
	public class Artist
	{
        public Artist()
        {
            Artworks = new List<Artwork>();
            Events = new List<Event>();
        }

        public int Id { get; set; }
        [Required]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Use letters only please")]
        public String Name { get; set; }
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Use letters only please")]
        public String Surname { get; set; }
        [Required]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public String Email { get; set; }
        [Required]
        public String Biography { get; set; }
        public ICollection<Artwork> Artworks { get; set; }
        public ICollection<Event> Events { get; set; }
        [Display(Name = "Profile picture")]
        public String ImageURL { get; set; }
        [Display(Name = "Number of followers")]
        [Range(0, Int32.MaxValue - 1)]
        public int NumFollowers { get; set; }
    }
}