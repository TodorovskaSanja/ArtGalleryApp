using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ArtGalleryApp.Models
{
	public class Event
	{
        public int Id { get; set; }
        [Required]
        public String Name { get; set; }
        [Required]
        public String Description { get; set; }
        [Required]
        public String Date { get; set; }
        [Required]
        public String Time { get; set; }
        [Range(1, 12)]
        [Required]
        public int Duration { get; set; }   // hours
        [Display(Name = "Poster")]
        public String ImageURL { get; set; }
        public int? ArtistId { get; set; }
        public Artist Artist { get; set; }
        public String Status { get; set; }  // pending, approved, denied
        [Range(0, Int32.MaxValue - 1)]
        public int TicketPrice { get; set; }
        public String Comment { get; set; }
        public String ArtistName { get; set; }
    }
}