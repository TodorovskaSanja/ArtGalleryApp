using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ArtGalleryApp.Models
{
	public class OrderArtwork
	{
        public int Id { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; }
        public String ArtworkName { get; set; }
        public String ArtworkArtistName { get; set; }
        public String ArtworkImageURL { get; set; }
        public int ArtworkPrice { get; set; }
    }
}