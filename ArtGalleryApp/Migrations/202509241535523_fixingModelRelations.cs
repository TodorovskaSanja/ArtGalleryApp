namespace ArtGalleryApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fixingModelRelations : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Artworks", "Order_Id", "dbo.Orders");
            DropForeignKey("dbo.Artists", "User_Id", "dbo.Users");
            DropForeignKey("dbo.Artworks", "ArtistId", "dbo.Artists");
            DropForeignKey("dbo.Events", "ArtistId", "dbo.Artists");
            DropForeignKey("dbo.Orders", "UserId", "dbo.Users");
            DropForeignKey("dbo.Tickets", "UserId", "dbo.Users");
            DropForeignKey("dbo.Tickets", "EventId", "dbo.Events");
            DropIndex("dbo.Artists", new[] { "User_Id" });
            DropIndex("dbo.Artworks", new[] { "ArtistId" });
            DropIndex("dbo.Artworks", new[] { "Order_Id" });
            DropIndex("dbo.Events", new[] { "ArtistId" });
            CreateTable(
                "dbo.OrderArtworks",
                c => new
                    {
                        OrderId = c.Int(nullable: false),
                        ArtworkId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.OrderId, t.ArtworkId })
                .ForeignKey("dbo.Orders", t => t.OrderId, cascadeDelete: true)
                .ForeignKey("dbo.Artworks", t => t.ArtworkId, cascadeDelete: true)
                .Index(t => t.OrderId)
                .Index(t => t.ArtworkId);
            
            CreateTable(
                "dbo.UserFollowedArtists",
                c => new
                    {
                        UserId = c.Int(nullable: false),
                        ArtistId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.UserId, t.ArtistId })
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.Artists", t => t.ArtistId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.ArtistId);
            
            AddColumn("dbo.Events", "Comment", c => c.String());
            AlterColumn("dbo.Artists", "Name", c => c.String(nullable: false));
            AlterColumn("dbo.Artists", "Surname", c => c.String(nullable: false));
            AlterColumn("dbo.Artists", "Email", c => c.String(nullable: false));
            AlterColumn("dbo.Artists", "Biography", c => c.String(nullable: false));
            AlterColumn("dbo.Artworks", "Name", c => c.String(nullable: false));
            AlterColumn("dbo.Artworks", "ArtistId", c => c.Int());
            AlterColumn("dbo.Artworks", "Category", c => c.String(nullable: false));
            AlterColumn("dbo.Artworks", "Description", c => c.String(nullable: false));
            AlterColumn("dbo.Artworks", "Year", c => c.String(nullable: false));
            AlterColumn("dbo.Events", "Name", c => c.String(nullable: false));
            AlterColumn("dbo.Events", "Description", c => c.String(nullable: false));
            AlterColumn("dbo.Events", "Date", c => c.String(nullable: false));
            AlterColumn("dbo.Events", "Time", c => c.String(nullable: false));
            AlterColumn("dbo.Events", "ArtistId", c => c.Int());
            AlterColumn("dbo.Orders", "Name", c => c.String(nullable: false));
            AlterColumn("dbo.Orders", "Surname", c => c.String(nullable: false));
            AlterColumn("dbo.Orders", "Email", c => c.String(nullable: false));
            AlterColumn("dbo.Orders", "PhoneNumber", c => c.String(nullable: false));
            AlterColumn("dbo.Orders", "Address", c => c.String(nullable: false));
            AlterColumn("dbo.Orders", "City", c => c.String(nullable: false));
            AlterColumn("dbo.Orders", "Country", c => c.String(nullable: false));
            AlterColumn("dbo.Users", "Name", c => c.String(nullable: false));
            AlterColumn("dbo.Users", "Surname", c => c.String(nullable: false));
            AlterColumn("dbo.Users", "Email", c => c.String(nullable: false));
            CreateIndex("dbo.Artworks", "ArtistId");
            CreateIndex("dbo.Events", "ArtistId");
            AddForeignKey("dbo.Artworks", "ArtistId", "dbo.Artists", "Id");
            AddForeignKey("dbo.Events", "ArtistId", "dbo.Artists", "Id");
            AddForeignKey("dbo.Orders", "UserId", "dbo.Users", "Id");
            AddForeignKey("dbo.Tickets", "UserId", "dbo.Users", "Id");
            AddForeignKey("dbo.Tickets", "EventId", "dbo.Events", "Id");
            DropColumn("dbo.Artists", "User_Id");
            DropColumn("dbo.Artworks", "Order_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Artworks", "Order_Id", c => c.Int());
            AddColumn("dbo.Artists", "User_Id", c => c.Int());
            DropForeignKey("dbo.Tickets", "EventId", "dbo.Events");
            DropForeignKey("dbo.Tickets", "UserId", "dbo.Users");
            DropForeignKey("dbo.Orders", "UserId", "dbo.Users");
            DropForeignKey("dbo.Events", "ArtistId", "dbo.Artists");
            DropForeignKey("dbo.Artworks", "ArtistId", "dbo.Artists");
            DropForeignKey("dbo.UserFollowedArtists", "ArtistId", "dbo.Artists");
            DropForeignKey("dbo.UserFollowedArtists", "UserId", "dbo.Users");
            DropForeignKey("dbo.OrderArtworks", "ArtworkId", "dbo.Artworks");
            DropForeignKey("dbo.OrderArtworks", "OrderId", "dbo.Orders");
            DropIndex("dbo.UserFollowedArtists", new[] { "ArtistId" });
            DropIndex("dbo.UserFollowedArtists", new[] { "UserId" });
            DropIndex("dbo.OrderArtworks", new[] { "ArtworkId" });
            DropIndex("dbo.OrderArtworks", new[] { "OrderId" });
            DropIndex("dbo.Events", new[] { "ArtistId" });
            DropIndex("dbo.Artworks", new[] { "ArtistId" });
            AlterColumn("dbo.Users", "Email", c => c.String());
            AlterColumn("dbo.Users", "Surname", c => c.String());
            AlterColumn("dbo.Users", "Name", c => c.String());
            AlterColumn("dbo.Orders", "Country", c => c.String());
            AlterColumn("dbo.Orders", "City", c => c.String());
            AlterColumn("dbo.Orders", "Address", c => c.String());
            AlterColumn("dbo.Orders", "PhoneNumber", c => c.String());
            AlterColumn("dbo.Orders", "Email", c => c.String());
            AlterColumn("dbo.Orders", "Surname", c => c.String());
            AlterColumn("dbo.Orders", "Name", c => c.String());
            AlterColumn("dbo.Events", "ArtistId", c => c.Int(nullable: false));
            AlterColumn("dbo.Events", "Time", c => c.String());
            AlterColumn("dbo.Events", "Date", c => c.String());
            AlterColumn("dbo.Events", "Description", c => c.String());
            AlterColumn("dbo.Events", "Name", c => c.String());
            AlterColumn("dbo.Artworks", "Year", c => c.String());
            AlterColumn("dbo.Artworks", "Description", c => c.String());
            AlterColumn("dbo.Artworks", "Category", c => c.String());
            AlterColumn("dbo.Artworks", "ArtistId", c => c.Int(nullable: false));
            AlterColumn("dbo.Artworks", "Name", c => c.String());
            AlterColumn("dbo.Artists", "Biography", c => c.String());
            AlterColumn("dbo.Artists", "Email", c => c.String());
            AlterColumn("dbo.Artists", "Surname", c => c.String());
            AlterColumn("dbo.Artists", "Name", c => c.String());
            DropColumn("dbo.Events", "Comment");
            DropTable("dbo.UserFollowedArtists");
            DropTable("dbo.OrderArtworks");
            CreateIndex("dbo.Events", "ArtistId");
            CreateIndex("dbo.Artworks", "Order_Id");
            CreateIndex("dbo.Artworks", "ArtistId");
            CreateIndex("dbo.Artists", "User_Id");
            AddForeignKey("dbo.Tickets", "EventId", "dbo.Events", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Tickets", "UserId", "dbo.Users", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Orders", "UserId", "dbo.Users", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Events", "ArtistId", "dbo.Artists", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Artworks", "ArtistId", "dbo.Artists", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Artists", "User_Id", "dbo.Users", "Id");
            AddForeignKey("dbo.Artworks", "Order_Id", "dbo.Orders", "Id");
        }
    }
}
