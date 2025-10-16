namespace ArtGalleryApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedOrderArtworks : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.OrderArtworks1",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OrderId = c.Int(nullable: false),
                        ArtworkName = c.String(),
                        ArtworkArtistName = c.String(),
                        ArtworkImageURL = c.String(),
                        ArtworkPrice = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Orders", t => t.OrderId, cascadeDelete: true)
                .Index(t => t.OrderId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OrderArtworks1", "OrderId", "dbo.Orders");
            DropIndex("dbo.OrderArtworks1", new[] { "OrderId" });
            DropTable("dbo.OrderArtworks1");
        }
    }
}
