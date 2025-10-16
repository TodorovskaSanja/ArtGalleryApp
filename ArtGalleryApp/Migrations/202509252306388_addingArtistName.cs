namespace ArtGalleryApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addingArtistName : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Artists", "ArtistName", c => c.String());
            AddColumn("dbo.Events", "ArtistName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Events", "ArtistName");
            DropColumn("dbo.Artists", "ArtistName");
        }
    }
}
