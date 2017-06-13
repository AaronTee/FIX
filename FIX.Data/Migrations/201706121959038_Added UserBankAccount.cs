namespace FIX.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedUserBankAccount : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Gender",
                c => new
                    {
                        GenderId = c.Int(nullable: false, identity: true),
                        Description = c.String(nullable: false),
                        CreatedTimestamp = c.DateTime(nullable: false),
                        ModifiedTimestamp = c.DateTime(),
                    })
                .PrimaryKey(t => t.GenderId);
            
            CreateTable(
                "dbo.UserBankAccount",
                c => new
                    {
                        UserId = c.Int(nullable: false),
                        BankId = c.Int(nullable: false),
                        BankAccountNo = c.String(nullable: false),
                        BankAccountHolder = c.String(nullable: false),
                        BankBranch = c.String(nullable: false),
                        IsPrimary = c.Boolean(nullable: false),
                        CreatedTimestamp = c.DateTime(nullable: false),
                        ModifiedTimestamp = c.DateTime(),
                        Bank_BankId = c.Int(),
                        User_UserId = c.Int(),
                    })
                .PrimaryKey(t => new { t.UserId, t.BankId })
                .ForeignKey("dbo.Bank", t => t.Bank_BankId)
                .ForeignKey("dbo.Bank", t => t.BankId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.User_UserId)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.BankId)
                .Index(t => t.Bank_BankId)
                .Index(t => t.User_UserId);
            
            CreateTable(
                "dbo.Bank",
                c => new
                    {
                        BankId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        CreatedTimestamp = c.DateTime(nullable: false),
                        ModifiedTimestamp = c.DateTime(),
                    })
                .PrimaryKey(t => t.BankId);
            
            AddColumn("dbo.UserProfile", "Country", c => c.String());
            AddColumn("dbo.UserProfile", "PhoneNo", c => c.String());
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserBankAccount", "UserId", "dbo.Users");
            DropForeignKey("dbo.UserBankAccount", "User_UserId", "dbo.Users");
            DropForeignKey("dbo.UserBankAccount", "BankId", "dbo.Bank");
            DropForeignKey("dbo.UserBankAccount", "Bank_BankId", "dbo.Bank");
            DropIndex("dbo.UserBankAccount", new[] { "User_UserId" });
            DropIndex("dbo.UserBankAccount", new[] { "Bank_BankId" });
            DropIndex("dbo.UserBankAccount", new[] { "BankId" });
            DropIndex("dbo.UserBankAccount", new[] { "UserId" });
            DropColumn("dbo.UserProfile", "PhoneNo");
            DropColumn("dbo.UserProfile", "Country");
            DropTable("dbo.Bank");
            DropTable("dbo.UserBankAccount");
            DropTable("dbo.Gender");
        }
    }
}
