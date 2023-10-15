-- AspNetUsers
CREATE TABLE [dbo].[AspNetUsers] (
    [Id] NVARCHAR(128) NOT NULL PRIMARY KEY,
    [UserName] NVARCHAR(256) NOT NULL,
    [PasswordHash] NVARCHAR(MAX),
    [SecurityStamp] NVARCHAR(MAX),
    [Email] NVARCHAR(256),
    [EmailConfirmed] BIT NOT NULL DEFAULT 0,
    [PhoneNumber] NVARCHAR(50),
    [PhoneNumberConfirmed] BIT NOT NULL DEFAULT 0,
    [TwoFactorEnabled] BIT NOT NULL DEFAULT 0,
    [LockoutEndDateUtc] DATETIME,
    [LockoutEnabled] BIT NOT NULL,
    [AccessFailedCount] INT NOT NULL,
   
);

-- AspNetRoles
CREATE TABLE [dbo].[AspNetRoles] (
    [Id] NVARCHAR(128) NOT NULL PRIMARY KEY,
    [Name] NVARCHAR(256) NOT NULL
);

-- AspNetUserRoles
CREATE TABLE [dbo].[AspNetUserRoles] (
    [UserId] NVARCHAR(128) NOT NULL,
    [RoleId] NVARCHAR(128) NOT NULL,
    PRIMARY KEY ([UserId], [RoleId]),
    FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers]([Id]) ON DELETE CASCADE,
    FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles]([Id]) ON DELETE CASCADE
);

-- AspNetUserClaims
CREATE TABLE [dbo].[AspNetUserClaims] (
    [Id] INT NOT NULL PRIMARY KEY IDENTITY,
    [UserId] NVARCHAR(128) NOT NULL,
    [ClaimType] NVARCHAR(MAX),
    [ClaimValue] NVARCHAR(MAX),
    FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers]([Id]) ON DELETE CASCADE
);

-- AspNetUserLogins
CREATE TABLE [dbo].[AspNetUserLogins] (
    [LoginProvider] NVARCHAR(128) NOT NULL,
    [ProviderKey] NVARCHAR(128) NOT NULL,
    [UserId] NVARCHAR(128) NOT NULL,
    PRIMARY KEY ([LoginProvider], [ProviderKey], [UserId]),
    FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers]([Id]) ON DELETE CASCADE
);

-- Indexes and uniqueness constraints
CREATE UNIQUE INDEX [UserNameIndex] ON [dbo].[AspNetUsers]([UserName] ASC);
CREATE UNIQUE INDEX [RoleNameIndex] ON [dbo].[AspNetRoles]([Name] ASC);

CREATE TABLE Patient (
    PatientID INT PRIMARY KEY IDENTITY(1,1),
    IdentityUserID NVARCHAR(128) UNIQUE NOT NULL,  -- Link to the ASP.NET Core Identity user
    FirstName NVARCHAR(255) NOT NULL,
    LastName NVARCHAR(255) NOT NULL,
    BirthDate DATE,
    ContactNumber NVARCHAR(50),
     Email NVARCHAR(255) NOT NULL, -- Added Email column
    CreatedAt DATETIME DEFAULT GETDATE(),
    UpdatedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (IdentityUserID) REFERENCES AspNetUsers(Id)
);

GO
CREATE TABLE XrayRecord (
       RecordID INT PRIMARY KEY IDENTITY(1,1),
    PatientID INT NOT NULL,
    TypeID INT NOT NULL,
    XrayImage VARBINARY(MAX),
    XrayImagePath NVARCHAR(255),
    PractitionerID INT NOT NULL,

    Description NVARCHAR(1000),
    CreatedAt DATETIME DEFAULT GETDATE(),
    UpdatedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (PatientID) REFERENCES Patient(PatientID),
    FOREIGN KEY (TypeID) REFERENCES XrayType(TypeID),
    FOREIGN KEY (PractitionerID) REFERENCES MedicalPractitioner(PractitionerID)
);
GO
CREATE TABLE Feedback (
    FeedbackID INT PRIMARY KEY IDENTITY(1,1),
    PatientID INT NOT NULL,
    PractitionerID INT NOT NULL, -- Link feedback to a practitioner
    AppointmentID INT,  
    RecordID INT,  
    Rating TINYINT NOT NULL,
    Comments NVARCHAR(MAX),
    FeedbackGiver NVARCHAR(255) NOT NULL,
    CreatedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (PatientID) REFERENCES Patient(PatientID),
    FOREIGN KEY (PractitionerID) REFERENCES MedicalPractitioner(PractitionerID), -- FK to MedicalPractitioner
    FOREIGN KEY (AppointmentID) REFERENCES Appointment(AppointmentID),
    FOREIGN KEY (RecordID) REFERENCES XrayRecord(RecordID)
);


GO
CREATE TABLE MedicalPractitioner (
    PractitionerID INT PRIMARY KEY IDENTITY(1,1),
    IdentityUserID NVARCHAR(128) UNIQUE NOT NULL,
    FirstName NVARCHAR(255) NOT NULL,
    LastName NVARCHAR(255) NOT NULL,
    Specialization NVARCHAR(255) NOT NULL,
    Email NVARCHAR(255) NOT NULL, 
    CreatedAt DATETIME DEFAULT GETDATE(),
    UpdatedAt DATETIME DEFAULT GETDATE(),
    NumberOfXrayTypesExpertise INT,
     Email NVARCHAR(255) NOT NULL, -- Added Email column
    Facility NVARCHAR(255),  -- Added Facility column
    Latitude float, 
    Longitude float,
    FOREIGN KEY (IdentityUserID) REFERENCES AspNetUsers(Id)
);
GO
CREATE TABLE Appointment (
    AppointmentID INT PRIMARY KEY IDENTITY(1,1),
    PatientID INT NOT NULL,
    PractitionerID INT NOT NULL,
    DateScheduled DATETIME NOT NULL,
    TypeOfXray NVARCHAR(255) NOT NULL,
    Status NVARCHAR(50) NOT NULL,
    Reason NVARCHAR(MAX),
    CreatedAt DATETIME DEFAULT GETDATE(),
    UpdatedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (PatientID) REFERENCES Patient(PatientID),
    FOREIGN KEY (PractitionerID) REFERENCES MedicalPractitioner(PractitionerID)
);

GO
CREATE TABLE XrayType (
    TypeID INT PRIMARY KEY IDENTITY(1,1),
    TypeName NVARCHAR(255) NOT NULL,
    Description NVARCHAR(MAX)
);
GO
CREATE TABLE Innovations (
    InnovationID INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(255) NOT NULL,
    Description NVARCHAR(MAX),
    Impact NVARCHAR(MAX)
);

GO