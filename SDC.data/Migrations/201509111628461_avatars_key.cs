namespace SDC.data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class avatars_key : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Avatars", "Key", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Avatars", "Key");
        }
    }
}
