namespace BadmintonSvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class QAttributes : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.QStatus",
                c => new
                    {
                        QStatusID = c.Int(nullable: false, identity: true),
                        StatusName = c.String(),
                    })
                .PrimaryKey(t => t.QStatusID);
            
            CreateTable(
                "dbo.Skillset",
                c => new
                    {
                        SkillsetID = c.Int(nullable: false, identity: true),
                        SkillsetName = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.SkillsetID);
            
            AddColumn("dbo.Queue", "SkillsetID", c => c.Int(nullable: false));
            AddColumn("dbo.Queue", "QStatusID", c => c.Int(nullable: false));
            CreateIndex("dbo.Queue", "SkillsetID");
            CreateIndex("dbo.Queue", "QStatusID");
            AddForeignKey("dbo.Queue", "QStatusID", "dbo.QStatus", "QStatusID", cascadeDelete: true);
            AddForeignKey("dbo.Queue", "SkillsetID", "dbo.Skillset", "SkillsetID", cascadeDelete: true);
            DropColumn("dbo.Queue", "Skillset");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Queue", "Skillset", c => c.String());
            DropForeignKey("dbo.Queue", "SkillsetID", "dbo.Skillset");
            DropForeignKey("dbo.Queue", "QStatusID", "dbo.QStatus");
            DropIndex("dbo.Queue", new[] { "QStatusID" });
            DropIndex("dbo.Queue", new[] { "SkillsetID" });
            DropColumn("dbo.Queue", "QStatusID");
            DropColumn("dbo.Queue", "SkillsetID");
            DropTable("dbo.Skillset");
            DropTable("dbo.QStatus");
        }
    }
}
