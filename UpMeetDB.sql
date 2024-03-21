CREATE DATABASE UpMeetDB

CREATE TABLE Images (
	ImageID INT IDENTITY(101, 1),
	Path NVARCHAR(1000) NOT NULL,

	--CONSTRAINTS
	--PRIMARY KEY
	CONSTRAINT images_imageid_pk PRIMARY KEY(ImageID)
);

CREATE TABLE [User] (
	UserID INT IDENTITY(101, 1),
	FirstName NVARCHAR(20) NOT NULL,
	LastName NVARCHAR(20) NOT NULL,
	UserName NVARCHAR(30) NOT NULL,
	Password NVARCHAR(35) NOT NULL,
	Bio NVARCHAR(2000),
	ImageID INT,
	Active BIT NOT NULL DEFAULT 'true',

	--CONSTRAINTS
	--PRIMARY KEY
	CONSTRAINT user_userid_pk PRIMARY KEY(UserID),

	--FOREIGN KEY
	CONSTRAINT user_imageid_fk FOREIGN KEY(ImageID) REFERENCES Images(ImageID)
);

CREATE TABLE [Event] (
	EventID INT IDENTITY(101, 1),
	Name NVARCHAR(100) NOT NULL,
	Description NVARCHAR(2000) NOT NULL,
	ImageID INT,
	StartDate DATE NOT NULL,
	EndDate DATE NOT NULL,
	Expired BIT NOT NULL DEFAULT 'false',
	Active BIT NOT NULL DEFAULT 'true',

	--CONSTRAINTS
	--PRIMARY KEY
	CONSTRAINT event_eventid_pk PRIMARY KEY(EventID),

	--FOREIGN KEY
	CONSTRAINT event_imageid_fk FOREIGN KEY(ImageID) REFERENCES Images(ImageID),

	--CHECK
	CONSTRAINT event_startdate_ck CHECK (StartDate >= GETDATE()),
	CONSTRAINT event_enddate_ck CHECK (EndDate >= StartDate)
);

CREATE TABLE UserFavorites (
	EventID INT,
	UserID INT,

	--CONSTRAINTS
	--PRIMARY KEY
	CONSTRAINT userfavorites_userfavoritesid_pk PRIMARY KEY(EventID, UserID),

	--FOREIGN KEYS
	CONSTRAINT userfavorites_eventid_fk FOREIGN KEY (EventID) REFERENCES [Event](EventID),
	CONSTRAINT userfavorites_userid_fk FOREIGN KEY (UserID) REFERENCES [User](UserID)
);

ALTER TABLE [User]
ADD Password NVARCHAR(35) NOT NULL;

ALTER TABLE [Event] DROP CONSTRAINT event_stardate_ck
ALTER TABLE [Event] DROP CONSTRAINT event_startdate_ck
ALTER TABLE [Event] DROP CONSTRAINT event_enddate_ck

ALTER TABLE [Event] ADD CONSTRAINT event_startdate_ck CHECK (StartDate >= GETDATE())
ALTER TABLE [Event] ADD CONSTRAINT event_enddate_ck CHECK (EndDate >= StartDate)

select * from [User] JOIN UserFavorites ON [User].UserID = UserFavorites.UserID JOIN [Event] ON [Event].EventID = UserFavorites.EventID

select * from  Images

select * from [User]

select * from UserFavorites