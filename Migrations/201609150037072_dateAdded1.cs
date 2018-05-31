namespace BadmintonSvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dateAdded1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.QueueTemp", "Created", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.QueueTemp", "Created");
        }
    }
}
