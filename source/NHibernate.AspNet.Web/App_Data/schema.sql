
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
       constraint FK86803B282B87AB2A foreign key (RoleId) references AspNetRoles,
       constraint FK86803B28EA778823 foreign key (UserId) references AspNetUsers
    )

    create table AspNetUserLogins (
        UserId TEXT not null,
       LoginProvider TEXT,
       ProviderKey TEXT,
       constraint FKEF896DAEEA778823 foreign key (UserId) references AspNetUsers
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
       constraint FKF4F7D992EA778823 foreign key (UserId) references AspNetUsers
    )

    create table ApplicationUser (
        applicationuser_key TEXT not null,
       primary key (applicationuser_key),
       constraint FK4376B148E75DF37 foreign key (applicationuser_key) references AspNetUsers
    )
