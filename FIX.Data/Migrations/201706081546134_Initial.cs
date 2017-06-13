namespace FIX.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Role",
                c => new
                    {
                        RoleId = c.Int(nullable: false, identity: true),
                        RoleName = c.String(nullable: false),
                        Description = c.String(),
                        CreatedTimestamp = c.DateTime(nullable: false),
                        ModifiedTimestamp = c.DateTime(),
                    })
                .PrimaryKey(t => t.RoleId);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        UserId = c.Int(nullable: false, identity: true),
                        Username = c.String(nullable: false, maxLength: 25),
                        Email = c.String(nullable: false),
                        Password = c.String(nullable: false),
                        IP = c.String(),
                        HasAcceptedTerms = c.Boolean(nullable: false),
                        HasEmailVerified = c.Boolean(nullable: false),
                        CreatedTimestamp = c.DateTime(nullable: false),
                        ModifiedTimestamp = c.DateTime(),
                    })
                .PrimaryKey(t => t.UserId)
                .Index(t => t.Username, unique: true);
            
            CreateTable(
                "dbo.UserProfile",
                c => new
                    {
                        UserProfileId = c.Int(nullable: false),
                        FirstName = c.String(nullable: false, maxLength: 100),
                        LastName = c.String(maxLength: 100),
                        Gender = c.String(maxLength: 2),
                        Address = c.String(maxLength: 4000),
                        CreatedTimestamp = c.DateTime(nullable: false),
                        ModifiedTimestamp = c.DateTime(),
                    })
                .PrimaryKey(t => t.UserProfileId)
                .ForeignKey("dbo.Users", t => t.UserProfileId)
                .Index(t => t.UserProfileId);
            
            CreateTable(
                "dbo.UserRole",
                c => new
                    {
                        User_UserId = c.Int(nullable: false),
                        Role_RoleId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.User_UserId, t.Role_RoleId })
                .ForeignKey("dbo.Users", t => t.User_UserId, cascadeDelete: true)
                .ForeignKey("dbo.Role", t => t.Role_RoleId, cascadeDelete: true)
                .Index(t => t.User_UserId)
                .Index(t => t.Role_RoleId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserRole", "Role_RoleId", "dbo.Role");
            DropForeignKey("dbo.UserRole", "User_UserId", "dbo.Users");
            DropForeignKey("dbo.UserProfile", "UserProfileId", "dbo.Users");
            DropIndex("dbo.UserRole", new[] { "Role_RoleId" });
            DropIndex("dbo.UserRole", new[] { "User_UserId" });
            DropIndex("dbo.UserProfile", new[] { "UserProfileId" });
            DropIndex("dbo.Users", new[] { "Username" });
            DropTable("dbo.UserRole");
            DropTable("dbo.UserProfile");
            DropTable("dbo.Users");
            DropTable("dbo.Role");
        }
    }
}
