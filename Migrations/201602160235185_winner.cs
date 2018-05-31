namespace BadmintonSvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class winner : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Queue", "Won", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Queue", "Won");
        }
    }
}
