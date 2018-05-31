namespace BadmintonSvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Final : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Club",
                c => new
                    {
                        ClubID = c.Int(nullable: false, identity: true),
                        ClubName = c.String(),
                        NoOfCourts = c.Int(nullable: false),
                        Organizer = c.String(),
                        ClubEmail = c.String(),
                        StreetName = c.String(),
                        City = c.String(),
                        State = c.String(),
                        Zipcode = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.ClubID);
            
            CreateTable(
                "dbo.Player",
                c => new
                    {
                        PlayerID = c.Int(nullable: false, identity: true),
                        PlayerName = c.String(nullable: false),
                        PlayerEmail = c.String(nullable: false),
                        Phone = c.Long(nullable: false),
                        Preference = c.String(nullable: false),
                        Username = c.String(),
                        Password = c.String(),
                        LoginType = c.String(),
                    })
                .PrimaryKey(t => t.PlayerID);
            
            CreateTable(
                "dbo.Queue",
                c => new
                    {
                        QueueID = c.Int(nullable: false, identity: true),
                        ClubID = c.Int(nullable: false),
                        PlayerID = c.Int(nullable: false),
                        Skillset = c.String(),
                        Score = c.Int(nullable: false),
                        PlayDateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.QueueID)
                .ForeignKey("dbo.Club", t => t.ClubID, cascadeDelete: true)
                .ForeignKey("dbo.Player", t => t.PlayerID, cascadeDelete: true)
                .Index(t => t.ClubID)
                .Index(t => t.PlayerID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Queue", "PlayerID", "dbo.Player");
            DropForeignKey("dbo.Queue", "ClubID", "dbo.Club");
            DropIndex("dbo.Queue", new[] { "PlayerID" });
            DropIndex("dbo.Queue", new[] { "ClubID" });
            DropTable("dbo.Queue");
            DropTable("dbo.Player");
            DropTable("dbo.Club");
        }
    }
}
