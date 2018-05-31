namespace BadmintonSvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class game2 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Game",
                c => new
                    {
                        GameID = c.Int(nullable: false, identity: true),
                        player1 = c.Int(nullable: false),
                        player2 = c.Int(nullable: false),
                        player3 = c.Int(nullable: false),
                        player4 = c.Int(nullable: false),
                        ScoreA = c.Int(nullable: false),
                        ScoreB = c.Int(nullable: false),
                        StartTime = c.DateTime(nullable: false),
                        EndTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.GameID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Game");
        }
    }
}
