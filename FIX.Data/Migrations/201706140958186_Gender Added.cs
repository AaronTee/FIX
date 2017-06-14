namespace FIX.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class GenderAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserProfile", "Gender_GenderId", c => c.Int());
            AlterColumn("dbo.UserProfile", "Address", c => c.String(maxLength: 255));
            AlterColumn("dbo.UserProfile", "Country", c => c.String(maxLength: 20));
            AlterColumn("dbo.UserProfile", "PhoneNo", c => c.String(maxLength: 20));
            CreateIndex("dbo.UserProfile", "Gender_GenderId");
            AddForeignKey("dbo.UserProfile", "Gender_GenderId", "dbo.Gender", "GenderId");
            DropColumn("dbo.UserProfile", "Gender");
        }
        
        public override void Down()
        {
            AddColumn("dbo.UserProfile", "Gender", c => c.String(maxLength: 2));
            DropForeignKey("dbo.UserProfile", "Gender_GenderId", "dbo.Gender");
            DropIndex("dbo.UserProfile", new[] { "Gender_GenderId" });
            AlterColumn("dbo.UserProfile", "PhoneNo", c => c.String());
            AlterColumn("dbo.UserProfile", "Country", c => c.String());
            AlterColumn("dbo.UserProfile", "Address", c => c.String(maxLength: 4000));
            DropColumn("dbo.UserProfile", "Gender_GenderId");
        }
    }
}
