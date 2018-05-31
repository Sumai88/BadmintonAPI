namespace BadmintonSvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class unknownmig : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Queue", "QueueOrder");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Queue", "QueueOrder", c => c.Int(nullable: false));
        }
    }
}
