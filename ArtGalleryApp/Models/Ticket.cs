using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ArtGalleryApp.Models
{
	public class Ticket
	{
        public int Id { get; set; }
        public int? EventId { get; set; }
        public Event Event { get; set; }
        public int? UserId { get; set; }
        public User User { get; set; }
        [Required]
        public int NumTickets { get; set; }
        public int Price { get; set; }
        public DateTime WhenBought { get; set; }
    }
}