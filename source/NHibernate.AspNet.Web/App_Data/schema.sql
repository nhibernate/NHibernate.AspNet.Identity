
    PRAGMA foreign_keys = OFF

    drop table if exists AspNetUsers

    drop table if exists AspNetUserRoles

    drop table if exists AspNetUserLogins

    drop table if exists AspNetRoles

    drop table if exists AspNetUserClaims

    drop table if exists ApplicationUser

    PRAGMA foreign_keys = ON

    create table AspNetUsers (
        Id TEXT not null,
       AccessFailedCount INT,
       Email TEXT,
       EmailConfirmed BOOL,
       LockoutEnabled BOOL,
       LockoutEndDateUtc DATETIME,
       PasswordHash TEXT,
       PhoneNumber TEXT,
       PhoneNumberConfirmed BOOL,
       TwoFactorEnabled BOOL,
       UserName TEXT not null unique,
       SecurityStamp TEXT,
       primary key (Id)
    )

    create table AspNetUserRoles (
        UserId TEXT not null,
       RoleId TEXT not null,
       constraint FKFAADC1EF92E2FD93 foreign key (RoleId) references AspNetRoles,
       constraint FKFAADC1EF526E4265 foreign key (UserId) references AspNetUsers
    )

    create table AspNetUserLogins (
        UserId TEXT not null,
       LoginProvider TEXT,
       ProviderKey TEXT,
       constraint FK6B768E3C526E4265 foreign key (UserId) references AspNetUsers
    )

    create table AspNetRoles (
        Id TEXT not null,
       Name TEXT not null unique,
       primary key (Id)
    )

    create table AspNetUserClaims (
        Id  integer primary key autoincrement,
       ClaimType TEXT,
       ClaimValue TEXT,
       UserId TEXT,
       constraint FKE3450235526E4265 foreign key (UserId) references AspNetUsers
    )

    create table ApplicationUser (
        applicationuser_key TEXT not null,
       primary key (applicationuser_key),
       constraint FKBF196D2F8745E746 foreign key (applicationuser_key) references AspNetUsers
    )
