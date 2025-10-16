namespace ArtGalleryApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedOrderModel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Orders",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        Name = c.String(),
                        Surname = c.String(),
                        Email = c.String(),
                        PhoneNumber = c.String(),
                        Address = c.String(),
                        City = c.String(),
                        Country = c.String(),
                        ShippingMethod = c.String(),
                        ShippingPrice = c.Int(nullable: false),
                        Total = c.Int(nullable: false),
                        WhenOrdered = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            AddColumn("dbo.Artworks", "Order_Id", c => c.Int());
            CreateIndex("dbo.Artworks", "Order_Id");
            AddForeignKey("dbo.Artworks", "Order_Id", "dbo.Orders", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Orders", "UserId", "dbo.Users");
            DropForeignKey("dbo.Artworks", "Order_Id", "dbo.Orders");
            DropIndex("dbo.Orders", new[] { "UserId" });
            DropIndex("dbo.Artworks", new[] { "Order_Id" });
            DropColumn("dbo.Artworks", "Order_Id");
            DropTable("dbo.Orders");
        }
    }
}
