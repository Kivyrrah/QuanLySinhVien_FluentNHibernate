# QuanLySinhVien_FluentNHibernate
Example_FluentNHibernate_Project

3 Table Query: 

CREATE TABLE Teacher (
    ID INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(50) NOT NULL,
    Birthday DATETIME NOT NULL
);

CREATE TABLE ClassRoom (
    ID INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(50) NOT NULL,
    Subject NVARCHAR(100) NOT NULL,
    TeacherID INT NOT NULL,
    CONSTRAINT FK_ClassRoom_Teacher FOREIGN KEY (TeacherID) REFERENCES Teacher(ID)
);

CREATE TABLE Student (
    ID INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(50) NOT NULL,
    Birthday DATETIME NOT NULL,
    Address NVARCHAR(100) NOT NULL,
    ClassRoomID INT NOT NULL,
    CONSTRAINT FK_Student_ClassRoom FOREIGN KEY (ClassRoomID) REFERENCES ClassRoom(ID)
);
