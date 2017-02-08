namespace Chat.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.conversations",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Sender = c.String(),
                        Receiver = c.String(),
                        Text = c.String(maxLength: 150),
                        SentTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.user_details",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UserName = c.String(nullable: false),
                        Password = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.user_details");
            DropTable("dbo.conversations");
        }
    }
}
