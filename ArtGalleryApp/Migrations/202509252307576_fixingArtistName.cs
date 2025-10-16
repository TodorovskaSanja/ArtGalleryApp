namespace ArtGalleryApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fixingArtistName : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Artworks", "ArtistName", c => c.String());
            DropColumn("dbo.Artists", "ArtistName");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Artists", "ArtistName", c => c.String());
            DropColumn("dbo.Artworks", "ArtistName");
        }
    }
}
