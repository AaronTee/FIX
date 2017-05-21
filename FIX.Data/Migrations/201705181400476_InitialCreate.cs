namespace FIX.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        UserName = c.String(nullable: false),
                        Email = c.String(nullable: false),
                        Password = c.String(nullable: false),
                        IP = c.String(nullable: false),
                        CreatedTimestamp = c.DateTime(nullable: false),
                        ModifiedTimestamp = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.UserProfiles",
                c => new
                    {
                        ID = c.Long(nullable: false),
                        FirstName = c.String(nullable: false, maxLength: 100),
                        LastName = c.String(maxLength: 100),
                        Address = c.String(maxLength: 4000),
                        CreatedTimestamp = c.DateTime(nullable: false),
                        ModifiedTimestamp = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Users", t => t.ID)
                .Index(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserProfiles", "ID", "dbo.Users");
            DropIndex("dbo.UserProfiles", new[] { "ID" });
            DropTable("dbo.UserProfiles");
            DropTable("dbo.Users");
        }
    }
}
