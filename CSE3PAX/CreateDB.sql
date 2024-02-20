CREATE TABLE Users (
    UserID INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    Email NVARCHAR(100),
    Password NVARCHAR(255) NOT NULL,
    UserGuid uniqueidentifier NULL,
    FirstName NVARCHAR(50),
    LastName NVARCHAR(50),
    IsAdmin BIT NOT NULL DEFAULT 0,
    IsManager BIT NOT NULL DEFAULT 0,
    IsLecturer BIT NOT NULL DEFAULT 0,
    IsPasswordResetRequired BIT NULL DEFAULT 0,
    CreatedOn datetime NOT NULL DEFAULT GETDATE()
);

CREATE TABLE PasswordResets(
	email VARCHAR(150) NOT NULL PRIMARY KEY,
	token VARCHAR(150) NOT NULL,
	CreatedOn datetime NOT NULL DEFAULT GETDATE()
);


CREATE TABLE Lecturers (
    LecturerID INT IDENTITY(1,1) NOT NULL,
    UserID INT,
    FOREIGN KEY (UserID) REFERENCES Users(UserID),
    Expertise01 NVARCHAR(255),
    Expertise02 NVARCHAR(255),
    Expertise03 NVARCHAR(255),
    Expertise04 NVARCHAR(255),
    Expertise05 NVARCHAR(255),
    Expertise06 NVARCHAR(255),
    ConcurrentLoadCapacity DECIMAL(5,3)
);

CREATE TABLE Subjects(
    SubjectID INT IDENTITY(1,1) NOT NULL,
    SubjectCode NVARCHAR(255),
    SubjectName NVARCHAR(255),
    SubjectClassification NVARCHAR(255),
    YearLevel INT,
    DevelopmentDifficulty NVARCHAR(255)
    PRIMARY KEY (SubjectID)
);

CREATE TABLE SubjectInstance(
    SubjectInstanceID INT IDENTITY(1,1) NOT NULL,
    SubjectID INT,
    FOREIGN KEY (SubjectID) REFERENCES Subjects(SubjectID),
    SubjectInstanceName NVARCHAR(100),
    SubjectInstanceCode NVARCHAR(100),
    LecturerID INT,
    StartDate DATETIME,
    EndDate DATETIME,
    SubjectInstanceYear INT,
);

INSERT INTO USERS (Email, Password, UserGuid, FirstName, LastName, IsAdmin, IsManager, IsLecturer, IsPasswordResetRequired, CreatedOn)
VALUES ('admin@mail.com', '8d242d653568af8ea3453c053c706603ba8a627c05da8de23e9c378e0c57687d','70270F25-EA4A-4885-925B-D467B04E29F1', 'Admin', 'Admin', 1, 0, 0, 0, GETDATE());

INSERT INTO USERS (Email, Password, UserGuid, FirstName, LastName, IsAdmin, IsManager, IsLecturer, IsPasswordResetRequired, CreatedOn)
VALUES ('manager@mail.com', 'ac702a3275058e40843881baf4c445a466d0988fb8c27d7471dae343b83bf8ac','c2a446b6-7b13-432b-8518-924babd3d0c5', 'Manager', 'Manager', 0, 1, 0, 0, GETDATE());

INSERT INTO USERS (Email, Password, UserGuid, FirstName, LastName, IsAdmin, IsManager, IsLecturer, IsPasswordResetRequired, CreatedOn)
VALUES ('lecturer@mail.com', 'ac702a3275058e40843881baf4c445a466d0988fb8c27d7471dae343b83bf8ac','4ad859b9-b3dd-4cc8-a2a9-d9f21fcad28f', 'Lecturer', 'Lecturer', 0, 0, 1, 0, GETDATE());


SELECT * FROM USERS