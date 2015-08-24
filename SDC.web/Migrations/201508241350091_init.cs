namespace SDC.web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Avatars",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Url = c.String(),
                        CustomForUserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Cities",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.LogInTraces",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Timestamp = c.DateTime(nullable: false),
                        IPAddress = c.String(),
                        User_UserId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.UserProfile", t => t.User_UserId)
                .Index(t => t.User_UserId);
            
            CreateTable(
                "dbo.UserProfile",
                c => new
                    {
                        UserId = c.Int(nullable: false, identity: true),
                        UserName = c.String(nullable: false),
                        Email = c.String(nullable: false),
                        LastSeen = c.DateTime(nullable: false),
                        IsLocked = c.Boolean(nullable: false),
                        ShowEmail = c.Boolean(nullable: false),
                        Avatar_Id = c.Int(),
                        City_Id = c.Int(),
                    })
                .PrimaryKey(t => t.UserId)
                .ForeignKey("dbo.Avatars", t => t.Avatar_Id)
                .ForeignKey("dbo.Cities", t => t.City_Id)
                .Index(t => t.Avatar_Id)
                .Index(t => t.City_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.LogInTraces", "User_UserId", "dbo.UserProfile");
            DropForeignKey("dbo.UserProfile", "City_Id", "dbo.Cities");
            DropForeignKey("dbo.UserProfile", "Avatar_Id", "dbo.Avatars");
            DropIndex("dbo.UserProfile", new[] { "City_Id" });
            DropIndex("dbo.UserProfile", new[] { "Avatar_Id" });
            DropIndex("dbo.LogInTraces", new[] { "User_UserId" });
            DropTable("dbo.UserProfile");
            DropTable("dbo.LogInTraces");
            DropTable("dbo.Cities");
            DropTable("dbo.Avatars");
        }
    }
}
