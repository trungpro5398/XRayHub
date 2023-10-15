namespace XRayHub.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddLatLongToMedicalPractitioner : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Appointment",
                c => new
                    {
                        AppointmentID = c.Int(nullable: false, identity: true),
                        PatientID = c.Int(nullable: false),
                        PractitionerID = c.Int(nullable: false),
                        DateScheduled = c.DateTime(nullable: false),
                        TypeOfXray = c.String(nullable: false, maxLength: 255),
                        Status = c.String(nullable: false, maxLength: 50),
                        Reason = c.String(),
                        CreatedAt = c.DateTime(),
                        UpdatedAt = c.DateTime(),
                    })
                .PrimaryKey(t => t.AppointmentID)
                .ForeignKey("dbo.MedicalPractitioner", t => t.PractitionerID)
                .ForeignKey("dbo.Patient", t => t.PatientID)
                .Index(t => t.PatientID)
                .Index(t => t.PractitionerID);
            
            CreateTable(
                "dbo.MedicalPractitioner",
                c => new
                    {
                        PractitionerID = c.Int(nullable: false, identity: true),
                        IdentityUserID = c.String(nullable: false, maxLength: 128),
                        FirstName = c.String(nullable: false, maxLength: 255),
                        LastName = c.String(nullable: false, maxLength: 255),
                        Specialization = c.String(nullable: false, maxLength: 255),
                        Email = c.String(nullable: false, maxLength: 255),
                        CreatedAt = c.DateTime(),
                        UpdatedAt = c.DateTime(),
                        NumberOfXrayTypesExpertise = c.Int(),
                        Facility = c.String(),
                        Latitude = c.Double(nullable: false),
                        Longitude = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.PractitionerID)
                .ForeignKey("dbo.AspNetUsers", t => t.IdentityUserID)
                .Index(t => t.IdentityUserID);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        UserType = c.Int(nullable: false),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Patient",
                c => new
                    {
                        PatientID = c.Int(nullable: false, identity: true),
                        IdentityUserID = c.String(nullable: false, maxLength: 128),
                        FirstName = c.String(nullable: false, maxLength: 255),
                        LastName = c.String(nullable: false, maxLength: 255),
                        BirthDate = c.DateTime(storeType: "date"),
                        ContactNumber = c.String(maxLength: 50),
                        CreatedAt = c.DateTime(),
                        UpdatedAt = c.DateTime(),
                    })
                .PrimaryKey(t => t.PatientID)
                .ForeignKey("dbo.AspNetUsers", t => t.IdentityUserID)
                .Index(t => t.IdentityUserID);
            
            CreateTable(
                "dbo.Feedback",
                c => new
                    {
                        FeedbackID = c.Int(nullable: false, identity: true),
                        PatientID = c.Int(nullable: false),
                        TypeOfXray = c.String(nullable: false, maxLength: 255),
                        Rating = c.Byte(nullable: false),
                        Comments = c.String(),
                        FeedbackGiver = c.String(nullable: false, maxLength: 255),
                    })
                .PrimaryKey(t => t.FeedbackID)
                .ForeignKey("dbo.Patient", t => t.PatientID)
                .Index(t => t.PatientID);
            
            CreateTable(
                "dbo.XrayRecord",
                c => new
                    {
                        RecordID = c.Int(nullable: false, identity: true),
                        PatientID = c.Int(nullable: false),
                        TypeOfXray = c.String(nullable: false, maxLength: 255),
                        XrayImage = c.Binary(),
                        CreatedAt = c.DateTime(),
                        UpdatedAt = c.DateTime(),
                    })
                .PrimaryKey(t => t.RecordID)
                .ForeignKey("dbo.Patient", t => t.PatientID)
                .Index(t => t.PatientID);
            
            CreateTable(
                "dbo.__MigrationHistory",
                c => new
                    {
                        MigrationId = c.String(nullable: false, maxLength: 150),
                        ContextKey = c.String(nullable: false, maxLength: 300),
                        Model = c.Binary(nullable: false),
                        ProductVersion = c.String(nullable: false, maxLength: 32),
                    })
                .PrimaryKey(t => new { t.MigrationId, t.ContextKey });
            
            CreateTable(
                "dbo.Innovations",
                c => new
                    {
                        InnovationID = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 255),
                        Description = c.String(),
                        Impact = c.String(),
                    })
                .PrimaryKey(t => t.InnovationID);
            
            CreateTable(
                "dbo.XrayType",
                c => new
                    {
                        TypeID = c.Int(nullable: false, identity: true),
                        TypeName = c.String(nullable: false, maxLength: 255),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.TypeID);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        RoleId = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.RoleId, t.UserId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.RoleId)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Patient", "IdentityUserID", "dbo.AspNetUsers");
            DropForeignKey("dbo.XrayRecord", "PatientID", "dbo.Patient");
            DropForeignKey("dbo.Feedback", "PatientID", "dbo.Patient");
            DropForeignKey("dbo.Appointment", "PatientID", "dbo.Patient");
            DropForeignKey("dbo.MedicalPractitioner", "IdentityUserID", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.Appointment", "PractitionerID", "dbo.MedicalPractitioner");
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.XrayRecord", new[] { "PatientID" });
            DropIndex("dbo.Feedback", new[] { "PatientID" });
            DropIndex("dbo.Patient", new[] { "IdentityUserID" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.MedicalPractitioner", new[] { "IdentityUserID" });
            DropIndex("dbo.Appointment", new[] { "PractitionerID" });
            DropIndex("dbo.Appointment", new[] { "PatientID" });
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.XrayType");
            DropTable("dbo.Innovations");
            DropTable("dbo.__MigrationHistory");
            DropTable("dbo.XrayRecord");
            DropTable("dbo.Feedback");
            DropTable("dbo.Patient");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.MedicalPractitioner");
            DropTable("dbo.Appointment");
        }
    }
}
