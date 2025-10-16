namespace ArtGalleryApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fixingTicketUserRelationship : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Tickets", "EventId", "dbo.Events");
            DropIndex("dbo.Tickets", new[] { "UserId" });
            AlterColumn("dbo.Tickets", "UserId", c => c.Int());
            CreateIndex("dbo.Tickets", "UserId");
            AddForeignKey("dbo.Tickets", "EventId", "dbo.Events", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Tickets", "EventId", "dbo.Events");
            DropIndex("dbo.Tickets", new[] { "UserId" });
            AlterColumn("dbo.Tickets", "UserId", c => c.Int(nullable: false));
            CreateIndex("dbo.Tickets", "UserId");
            AddForeignKey("dbo.Tickets", "EventId", "dbo.Events", "Id");
        }
    }
}
