namespace BadmintonSvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Skilset : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Skillset", "SkillsetName", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Skillset", "SkillsetName", c => c.Int(nullable: false));
        }
    }
}
