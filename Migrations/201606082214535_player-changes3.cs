namespace BadmintonSvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class playerchanges3 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Player", "Phone", c => c.Long());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Player", "Phone", c => c.Long(nullable: false));
        }
    }
}
