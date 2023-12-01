--Drop Tables 

DROP TABLE lecturers;
DROP TABLE subjectClassifications;

--Create Tables 

CREATE TABLE lecturers (
    userID int IDENTITY(1,1) PRIMARY KEY,
    LastName NVARCHAR(255) NOT NULL,
    FirstName NVARCHAR(255) NOT NULL,
    Email NVARCHAR(255) NOT NULL,
    Password NVARCHAR(60) NOT NULL,
    ConcurrentLoadCapacity int,
    ExpertiseFeild01 NVARCHAR(100),
    ExpertiseFeild02 NVARCHAR(100),
    ExpertiseFeild03 NVARCHAR(100),
    ExpertiseFeild04 NVARCHAR(100),
    ExpertiseFeild05 NVARCHAR(100),
    isLecturer BIT,
    isManager BIT,
    isAdministrator BIT
    );


	CREATE TABLE subjectClassifications(
		SubjectClassificationID int IDENTITY(1,1) PRIMARY KEY,
		SubjectClassifcationName NVARCHAR(255) NOT NULL
	);
    --Insert Data 

    INSERT INTO lecturers(LastName, FirstName, Email, Password, ConcurrentLoadCapacity, ExpertiseFeild01, isLecturer)
    VALUES  ('Smith', 'John', 'j.smith@latrobe.com.au','password', 6, 'Networking', 1),
            ('Long', 'Collin', 'c.long@latrobe.com.au','password', 6, 'Project Management', 1),
            ('Maddox', 'Amalia', 'a.maddox@latrobe.com.au','password', 6, 'Programming', 1),
            ('Ferrell', 'Molford', 'm.ferrell@latrobe.com.au','password', 6, 'System Administration', 1),
            ('Huber', 'Noble', 'n.huber@latrobe.com.au','password', 6, 'Project Management', 1),
            ('Pearson', 'Peter', 'p.pearson@latrobe.com.au','password', 6, 'Networking', 1),
            ('Whitehead', 'Pablo', 'p.whitehead@latrobe.com.au','password', 6, 'System Administration', 1),
            ('Jennings', 'Gretchen', 'j.gretchen@latrobe.com.au','password', 6, 'Networking', 1),
            ('Wu', 'Elizabeth', 'e.wu@latrobe.com.au','password', 6, 'Project Management', 1),
            ('Leach', 'Danielle', 'd.leach@latrobe.com.au','password', 6, 'Data Analysis', 1);

INSERT INTO subjectClassifications(SubjectClassifcationName)
	VALUES	('Networking'),
			('System Administration'),
			('Project Mangement'),
			('Programming'),
			('Data Analysis');


--SELECT STATEMENTS 

SELECT * FROM lecturers