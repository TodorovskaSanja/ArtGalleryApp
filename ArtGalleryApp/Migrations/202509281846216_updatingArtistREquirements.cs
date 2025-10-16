namespace ArtGalleryApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatingArtistREquirements : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Artists", "Surname", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Artists", "Surname", c => c.String(nullable: false));
        }
    }
}
