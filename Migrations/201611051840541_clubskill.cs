namespace BadmintonSvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class clubskill : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Club", "SkillPredefined", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Club", "SkillPredefined");
        }
    }
}
