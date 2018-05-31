namespace BadmintonSvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class clubstatus : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Club", "Status", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Club", "Status");
        }
    }
}
