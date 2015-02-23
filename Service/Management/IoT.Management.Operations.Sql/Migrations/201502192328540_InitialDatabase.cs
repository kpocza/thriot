namespace IoT.Management.Operations.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialDatabase : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Company",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 32, unicode: false),
                        Name = c.String(nullable: false, maxLength: 50),
                        TelemetryDataSinkSettingsJson = c.String(maxLength: 2048),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Service",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 32, unicode: false),
                        Name = c.String(nullable: false, maxLength: 50),
                        ApiKey = c.String(nullable: false, maxLength: 32, unicode: false),
                        TelemetryDataSinkSettingsJson = c.String(maxLength: 2048),
                        CompanyId = c.String(nullable: false, maxLength: 32, unicode: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Company", t => t.CompanyId)
                .Index(t => t.CompanyId, name: "IX_Company_Id");
            
            CreateTable(
                "dbo.Network",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 32, unicode: false),
                        Name = c.String(nullable: false, maxLength: 50),
                        NetworkKey = c.String(nullable: false, maxLength: 32, unicode: false),
                        TelemetryDataSinkSettingsJson = c.String(maxLength: 2048),
                        ParentNetworkId = c.String(maxLength: 32, unicode: false),
                        CompanyId = c.String(nullable: false, maxLength: 32, unicode: false),
                        ServiceId = c.String(nullable: false, maxLength: 32, unicode: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Network", t => t.ParentNetworkId)
                .ForeignKey("dbo.Company", t => t.CompanyId)
                .ForeignKey("dbo.Service", t => t.ServiceId)
                .Index(t => t.ParentNetworkId, name: "IX_ParentNetwork_Id")
                .Index(t => t.CompanyId, name: "IX_Company_Id")
                .Index(t => t.ServiceId, name: "IX_Service_Id");
            
            CreateTable(
                "dbo.Device",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 32, unicode: false),
                        Name = c.String(nullable: false, maxLength: 50),
                        DeviceKey = c.String(nullable: false, maxLength: 32, unicode: false),
                        NumericId = c.Long(nullable: false),
                        CompanyId = c.String(nullable: false, maxLength: 32, unicode: false),
                        NetworkId = c.String(nullable: false, maxLength: 32, unicode: false),
                        ServiceId = c.String(nullable: false, maxLength: 32, unicode: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Company", t => t.CompanyId)
                .ForeignKey("dbo.Network", t => t.NetworkId)
                .ForeignKey("dbo.Service", t => t.ServiceId)
                .Index(t => t.CompanyId, name: "IX_Company_Id")
                .Index(t => t.NetworkId, name: "IX_Network_Id")
                .Index(t => t.ServiceId, name: "IX_Service_Id");
            
            CreateTable(
                "dbo.User",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 32, unicode: false),
                        Name = c.String(nullable: false, maxLength: 50),
                        Email = c.String(nullable: false, maxLength: 128),
                        Activated = c.Boolean(nullable: false),
                        ActivationCode = c.String(maxLength: 8000, unicode: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.LoginUser",
                c => new
                    {
                        Email = c.String(nullable: false, maxLength: 128),
                        PasswordHash = c.String(nullable: false, maxLength: 64, unicode: false),
                        Salt = c.String(nullable: false, maxLength: 32, unicode: false),
                        UserId = c.String(nullable: false, maxLength: 32),
                    })
                .PrimaryKey(t => t.Email);
            
            CreateTable(
                "dbo.Setting",
                c => new
                    {
                        Category = c.String(nullable: false, maxLength: 32, unicode: false),
                        Config = c.String(nullable: false, maxLength: 32, unicode: false),
                        Value = c.String(nullable: false, maxLength: 2048),
                    })
                .PrimaryKey(t => new { t.Category, t.Config });
            
            CreateTable(
                "dbo.UserCompany",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 32, unicode: false),
                        CompanyId = c.String(nullable: false, maxLength: 32, unicode: false),
                    })
                .PrimaryKey(t => new { t.UserId, t.CompanyId })
                .ForeignKey("dbo.User", t => t.UserId)
                .ForeignKey("dbo.Company", t => t.CompanyId)
                .Index(t => t.UserId, name: "IX_User_Id")
                .Index(t => t.CompanyId, name: "IX_Company_Id");
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserCompany", "CompanyId", "dbo.Company");
            DropForeignKey("dbo.UserCompany", "UserId", "dbo.User");
            DropForeignKey("dbo.Network", "ServiceId", "dbo.Service");
            DropForeignKey("dbo.Device", "ServiceId", "dbo.Service");
            DropForeignKey("dbo.Device", "NetworkId", "dbo.Network");
            DropForeignKey("dbo.Device", "CompanyId", "dbo.Company");
            DropForeignKey("dbo.Network", "CompanyId", "dbo.Company");
            DropForeignKey("dbo.Network", "ParentNetworkId", "dbo.Network");
            DropForeignKey("dbo.Service", "CompanyId", "dbo.Company");
            DropIndex("dbo.UserCompany", "IX_Company_Id");
            DropIndex("dbo.UserCompany", "IX_User_Id");
            DropIndex("dbo.Device", "IX_Service_Id");
            DropIndex("dbo.Device", "IX_Network_Id");
            DropIndex("dbo.Device", "IX_Company_Id");
            DropIndex("dbo.Network", "IX_Service_Id");
            DropIndex("dbo.Network", "IX_Company_Id");
            DropIndex("dbo.Network", "IX_ParentNetwork_Id");
            DropIndex("dbo.Service", "IX_Company_Id");
            DropTable("dbo.UserCompany");
            DropTable("dbo.Setting");
            DropTable("dbo.LoginUser");
            DropTable("dbo.User");
            DropTable("dbo.Device");
            DropTable("dbo.Network");
            DropTable("dbo.Service");
            DropTable("dbo.Company");
        }
    }
}
