
    
alter table AspNetUserRoles  drop foreign key FK86803B282B87AB2A


    
alter table AspNetUserRoles  drop foreign key FK86803B28EA778823


    
alter table AspNetUserLogins  drop foreign key FKEF896DAEEA778823


    
alter table AspNetUserClaims  drop foreign key FKF4F7D992EA778823


    
alter table ApplicationUser  drop foreign key FK4376B148E75DF37


    drop table if exists AspNetUsers

    drop table if exists AspNetUserRoles

    drop table if exists AspNetUserLogins

    drop table if exists AspNetRoles

    drop table if exists AspNetUserClaims

    drop table if exists ApplicationUser

    create table AspNetUsers (
        Id VARCHAR(255) not null,
       AccessFailedCount INTEGER,
       Email VARCHAR(255),
       EmailConfirmed TINYINT(1),
       LockoutEnabled TINYINT(1),
       LockoutEndDateUtc DATETIME,
       PasswordHash VARCHAR(255),
       PhoneNumber VARCHAR(255),
       PhoneNumberConfirmed TINYINT(1),
       TwoFactorEnabled TINYINT(1),
       UserName VARCHAR(255) not null unique,
       SecurityStamp VARCHAR(255),
       primary key (Id)
    )

    create table AspNetUserRoles (
        UserId VARCHAR(255) not null,
       RoleId VARCHAR(255) not null
    )

    create table AspNetUserLogins (
        UserId VARCHAR(255) not null,
       LoginProvider VARCHAR(255),
       ProviderKey VARCHAR(255)
    )

    create table AspNetRoles (
        Id VARCHAR(255) not null,
       Name VARCHAR(255) not null unique,
       primary key (Id)
    )

    create table AspNetUserClaims (
        Id INTEGER NOT NULL AUTO_INCREMENT,
       ClaimType VARCHAR(255),
       ClaimValue VARCHAR(255),
       UserId VARCHAR(255),
       primary key (Id)
    )

    create table ApplicationUser (
        applicationuser_key VARCHAR(255) not null,
       primary key (applicationuser_key)
    )

    alter table AspNetUserRoles 
        add index (RoleId), 
        add constraint FK86803B282B87AB2A 
        foreign key (RoleId) 
        references AspNetRoles (Id)

    alter table AspNetUserRoles 
        add index (UserId), 
        add constraint FK86803B28EA778823 
        foreign key (UserId) 
        references AspNetUsers (Id)

    alter table AspNetUserLogins 
        add index (UserId), 
        add constraint FKEF896DAEEA778823 
        foreign key (UserId) 
        references AspNetUsers (Id)

    alter table AspNetUserClaims 
        add index (UserId), 
        add constraint FKF4F7D992EA778823 
        foreign key (UserId) 
        references AspNetUsers (Id)

    alter table ApplicationUser 
        add index (applicationuser_key), 
        add constraint FK4376B148E75DF37 
        foreign key (applicationuser_key) 
        references AspNetUsers (Id)
