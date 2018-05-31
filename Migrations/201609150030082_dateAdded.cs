namespace BadmintonSvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dateAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Club", "Created", c => c.DateTime(nullable: false));
            AddColumn("dbo.Player", "MixInd", c => c.Long(nullable: false));
            AddColumn("dbo.Player", "Created", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Player", "Created");
            DropColumn("dbo.Player", "MixInd");
            DropColumn("dbo.Club", "Created");
        }
    }
}
