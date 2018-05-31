namespace BadmintonSvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MM : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Price",
                c => new
                    {
                        PriceID = c.Int(nullable: false, identity: true),
                        MeatName = c.String(),
                        Amount = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.PriceID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Price");
        }
    }
}
