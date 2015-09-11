namespace SDC.data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class settings_key : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.Settings");
            AlterColumn("dbo.Settings", "Key", c => c.String(nullable: false, maxLength: 128));
            AddPrimaryKey("dbo.Settings", "Key");
            DropColumn("dbo.Settings", "Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Settings", "Id", c => c.Int(nullable: false, identity: true));
            DropPrimaryKey("dbo.Settings");
            AlterColumn("dbo.Settings", "Key", c => c.String());
            AddPrimaryKey("dbo.Settings", "Id");
        }
    }
}
