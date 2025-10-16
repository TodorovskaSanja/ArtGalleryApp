using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ArtGalleryApp.Models
{
	public class Artwork
	{
        public int Id { get; set; }
        [Required]
        public String Name { get; set; }
        public int? ArtistId { get; set; }
        public Artist Artist { get; set; }
        [Required]
        public String Category { get; set; }    // painting, drawing, photography, sculpture, print
        [Required]
        public String Description { get; set; }
        [Required]
        public String Year { get; set; }
        [Display(Name = "Image")]
        public String ImageURL { get; set; }
        public String Status { get; set; }  // not for sale, for sale, purchased
        [Range(0, Int32.MaxValue - 1)]
        public int Price { get; set; }
        public String ArtistName { get; set; }
    }
}