namespace BadmintonSvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class court : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Court",
                c => new
                    {
                        CourtID = c.Int(nullable: false, identity: true),
                        CourtNum = c.Int(nullable: false),
                        ClubID = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.CourtID)
                .ForeignKey("dbo.Club", t => t.ClubID, cascadeDelete: true)
                .Index(t => t.ClubID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Court", "ClubID", "dbo.Club");
            DropIndex("dbo.Court", new[] { "ClubID" });
            DropTable("dbo.Court");
        }
    }
}
