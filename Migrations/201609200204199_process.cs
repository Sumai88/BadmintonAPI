namespace BadmintonSvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class process : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Process",
                c => new
                    {
                        ProcessID = c.Int(nullable: false, identity: true),
                        ProcessName = c.String(),
                        Status = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ProcessID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Process");
        }
    }
}
