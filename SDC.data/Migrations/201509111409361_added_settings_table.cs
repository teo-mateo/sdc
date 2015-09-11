namespace SDC.data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class added_settings_table : DbMigration
    {
        public override void Up()
        {   
            CreateTable(
                "dbo.Settings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Key = c.String(),
                        Value = c.String(),
                    })
                .PrimaryKey(t => t.Id);
        }
        
        public override void Down()
        {
            DropTable("dbo.Settings");
        }
    }
}
