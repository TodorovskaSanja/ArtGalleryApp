namespace ArtGalleryApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fixingUserRelations : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Artworks", "User_Id", "dbo.Users");
            DropForeignKey("dbo.Artworks", "User_Id1", "dbo.Users");
            DropForeignKey("dbo.Artworks", "User_Id2", "dbo.Users");
            DropIndex("dbo.Artworks", new[] { "User_Id" });
            DropIndex("dbo.Artworks", new[] { "User_Id1" });
            DropIndex("dbo.Artworks", new[] { "User_Id2" });
            CreateTable(
                "dbo.UserCart",
                c => new
                    {
                        UserId = c.Int(nullable: false),
                        ArtworkId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.UserId, t.ArtworkId })
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.Artworks", t => t.ArtworkId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.ArtworkId);
            
            CreateTable(
                "dbo.UserFavoriteArtworks",
                c => new
                    {
                        UserId = c.Int(nullable: false),
                        ArtworkId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.UserId, t.ArtworkId })
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.Artworks", t => t.ArtworkId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.ArtworkId);
            
            CreateTable(
                "dbo.UserPurchasedArtworks",
                c => new
                    {
                        UserId = c.Int(nullable: false),
                        ArtworkId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.UserId, t.ArtworkId })
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.Artworks", t => t.ArtworkId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.ArtworkId);
            
            AddColumn("dbo.Artworks", "Year", c => c.String());
            AddColumn("dbo.Events", "TicketPrice", c => c.Int(nullable: false));
            DropColumn("dbo.Artworks", "User_Id");
            DropColumn("dbo.Artworks", "User_Id1");
            DropColumn("dbo.Artworks", "User_Id2");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Artworks", "User_Id2", c => c.Int());
            AddColumn("dbo.Artworks", "User_Id1", c => c.Int());
            AddColumn("dbo.Artworks", "User_Id", c => c.Int());
            DropForeignKey("dbo.UserPurchasedArtworks", "ArtworkId", "dbo.Artworks");
            DropForeignKey("dbo.UserPurchasedArtworks", "UserId", "dbo.Users");
            DropForeignKey("dbo.UserFavoriteArtworks", "ArtworkId", "dbo.Artworks");
            DropForeignKey("dbo.UserFavoriteArtworks", "UserId", "dbo.Users");
            DropForeignKey("dbo.UserCart", "ArtworkId", "dbo.Artworks");
            DropForeignKey("dbo.UserCart", "UserId", "dbo.Users");
            DropIndex("dbo.UserPurchasedArtworks", new[] { "ArtworkId" });
            DropIndex("dbo.UserPurchasedArtworks", new[] { "UserId" });
            DropIndex("dbo.UserFavoriteArtworks", new[] { "ArtworkId" });
            DropIndex("dbo.UserFavoriteArtworks", new[] { "UserId" });
            DropIndex("dbo.UserCart", new[] { "ArtworkId" });
            DropIndex("dbo.UserCart", new[] { "UserId" });
            DropColumn("dbo.Events", "TicketPrice");
            DropColumn("dbo.Artworks", "Year");
            DropTable("dbo.UserPurchasedArtworks");
            DropTable("dbo.UserFavoriteArtworks");
            DropTable("dbo.UserCart");
            CreateIndex("dbo.Artworks", "User_Id2");
            CreateIndex("dbo.Artworks", "User_Id1");
            CreateIndex("dbo.Artworks", "User_Id");
            AddForeignKey("dbo.Artworks", "User_Id2", "dbo.Users", "Id");
            AddForeignKey("dbo.Artworks", "User_Id1", "dbo.Users", "Id");
            AddForeignKey("dbo.Artworks", "User_Id", "dbo.Users", "Id");
        }
    }
}
