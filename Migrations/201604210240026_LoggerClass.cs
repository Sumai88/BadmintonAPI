namespace BadmintonSvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LoggerClass : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Logger",
                c => new
                    {
                        LoggerID = c.Int(nullable: false, identity: true),
                        LogMessage = c.String(),
                        LogType = c.String(),
                        LogSource = c.String(),
                        LogDate = c.DateTime(nullable: false),
                        ClubID = c.Int(),
                        PlayerID = c.Int(),
                        QueueID = c.Int(),
                        QueueTempID = c.Int(),
                    })
                .PrimaryKey(t => t.LoggerID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Logger");
        }
    }
}
