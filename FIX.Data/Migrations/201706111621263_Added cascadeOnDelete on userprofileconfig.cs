namespace FIX.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedcascadeOnDeleteonuserprofileconfig : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.UserProfile", "UserProfileId", "dbo.Users");
            AddForeignKey("dbo.UserProfile", "UserProfileId", "dbo.Users", "UserId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserProfile", "UserProfileId", "dbo.Users");
            AddForeignKey("dbo.UserProfile", "UserProfileId", "dbo.Users", "UserId");
        }
    }
}
