namespace BadmintonSvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class skillset : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Player", "SkillsetID", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Player", "SkillsetID");
        }
    }
}
