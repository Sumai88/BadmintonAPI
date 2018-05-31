namespace BadmintonSvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class queueorder : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Player", "DeviceID", c => c.String());
            AddColumn("dbo.Queue", "QueueOrder", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Queue", "QueueOrder");
            DropColumn("dbo.Player", "DeviceID");
        }
    }
}
