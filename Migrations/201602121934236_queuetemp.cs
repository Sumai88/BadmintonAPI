namespace BadmintonSvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class queuetemp : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.QueueTemp",
                c => new
                    {
                        QueueTempID = c.Int(nullable: false, identity: true),
                        QueueID = c.Int(nullable: false),
                        ClubID = c.Int(nullable: false),
                        PlayerID = c.Int(nullable: false),
                        SkillsetID = c.Int(nullable: false),
                        QueueOrder = c.Int(nullable: false),
                        QStatusID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.QueueTempID)
                .ForeignKey("dbo.Queue", t => t.QueueID, cascadeDelete: true)
                .Index(t => t.QueueID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.QueueTemp", "QueueID", "dbo.Queue");
            DropIndex("dbo.QueueTemp", new[] { "QueueID" });
            DropTable("dbo.QueueTemp");
        }
    }
}
