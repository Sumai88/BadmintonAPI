namespace BadmintonSvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class debug : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Player", "DebugMode", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Player", "DebugMode");
        }
    }
}
