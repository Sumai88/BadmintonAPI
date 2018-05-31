namespace BadmintonSvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class playerchanges : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Player", "Preference", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Player", "Preference", c => c.String(nullable: false));
        }
    }
}
