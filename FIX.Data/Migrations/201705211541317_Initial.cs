namespace FIX.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserProfiles", "Gender", c => c.String(maxLength: 2));
            RenameColumn("dbo.Users", "UserName", "Username");
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserProfiles", "Gender");
        }
    }
}
