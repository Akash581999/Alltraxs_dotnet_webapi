show databases;
CREATE DATABASE pc_student;
use pc_student;
show tables;

CREATE TABLE pc_student.Alltraxs_users (
    UserId INT AUTO_INCREMENT PRIMARY KEY,
    FirstName VARCHAR(25) NOT NULL,
    LastName VARCHAR(25) NOT NULL,
    UserName VARCHAR(50),
    UserPassword VARCHAR(255) NOT NULL,
    Email VARCHAR(100) NOT NULL,
    Mobile VARCHAR(20) NOT NULL,
    ProfilePic BLOB
);
INSERT INTO pc_student.Alltraxs_users (FirstName, LastName, Email, Mobile, UserPassword) VALUES('Akash', 'Kumar', '58akash1999@gmail.com', '9458046883','58Akash1999@');
ALTER TABLE pc_student.Alltraxs_users ADD Role VARCHAR(10);
UPDATE pc_student.Alltraxs_users SET Username = 'Akash Kumar', UserPassword = '99Akash19@' WHERE UserId = 1;
TRUNCATE  pc_student.Alltraxs_users;
SELECT * FROM pc_student.Alltraxs_users;

CREATE TABLE pc_student.Alltraxs_Songs (
    SongId INT AUTO_INCREMENT PRIMARY KEY,
    Title VARCHAR(25) NOT NULL,
    Artist VARCHAR(25) NOT NULL,
    Album VARCHAR(50) NOT NULL,
    Genre VARCHAR(50) NOT NULL,
    Duration VARCHAR(20) NOT NULL,
    SongUrl VARCHAR(255),
    SongPic BLOB
);
INSERT INTO pc_student.Alltraxs_Songs (SongId, Title, Artist, Album, Genre, Duration, SongUrl, SongPic)
VALUES(1, 'In The Stars', 'Benson Boone', 'Starry Nights', 'Indie Pop', '3:03', 'nourl','nopic');
ALTER TABLE pc_student.Alltraxs_Songs ADD UploadOn TIMESTAMP DEFAULT CURRENT_TIMESTAMP;
UPDATE pc_student.Alltraxs_Songs SET SongUrl = 'http://www.song1url.com', SongPic = 'http://www.song1pic.com', Popularity ='90' WHERE SongId = 1;
TRUNCATE pc_student.Alltraxs_Songs;
SELECT * FROM pc_student.Alltraxs_Songs;

CREATE TABLE pc_student.Alltraxs_Playlists (
    Playlist_Id INT AUTO_INCREMENT PRIMARY KEY,
    UserId INT,
    Title VARCHAR(100) NOT NULL,
    Description VARCHAR(255) NOT NULL,
    CreatedOn TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (UserId) REFERENCES pc_student.Alltraxs_users (UserId),
    PlaylistImageUrl VARCHAR(255),
    IsPublic TINYINT DEFAULT 0,
    NumSongs INT DEFAULT 0
);
INSERT INTO pc_student.Alltraxs_Playlists (UserId, Title, Description, PlaylistImageUrl, IsPublic, NumSongs)VALUES(1, 'My Favorites', 'A collection of my favorite songs', 'https://example.com/favorites.jpg', 1, 10),(2, 'Workout Mix', 'High-energy songs for workouts', 'https://example.com/workout.jpg', 0, 15);
UPDATE pc_student.Alltraxs_Playlists SET Title = 'God Knight', Description = 'JAPAN', PlaylistImageUrl = 'Horrible website' WHERE Playlist_Id = 2;
ALTER TABLE pc_student.Alltraxs_Playlists MODIFY COLUMN IsPublic BIT NOT NULL;
DELETE FROM pc_student.Alltraxs_Playlists WHERE Playlist_Id BETWEEN 11 AND 22;
SELECT * FROM pc_student.Alltraxs_Playlists;

CREATE TABLE IF NOT EXISTS pc_student.Alltraxs_PlaylistSongs (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Title VARCHAR(255) NOT NULL,
    Artist VARCHAR(255),
    Album VARCHAR(255),
    Genre VARCHAR(100),
    Duration INT,
    SongUrl VARCHAR(255),
    SongPic VARCHAR(255),
    UserId INT,
    SongId INT,
    Playlist_Id INT,
    FOREIGN KEY (UserId) REFERENCES pc_student.Alltraxs_users (UserId),
    FOREIGN KEY (SongId) REFERENCES pc_student.Alltraxs_Songs(SongId),
    FOREIGN KEY (Playlist_Id) REFERENCES pc_student.Alltraxs_Playlists(Playlist_Id)
);
TRUNCATE pc_student.Alltraxs_PlaylistSongs;
SELECT * FROM pc_student.Alltraxs_PlaylistSongs;

CREATE TABLE pc_student.Alltraxs_ContactUs (
    Feedback_Id INT AUTO_INCREMENT PRIMARY KEY,
    UserName VARCHAR(50) NOT NULL,
    Email VARCHAR(100) NOT NULL,
    Country VARCHAR(20) NOT NULL,
    Comments VARCHAR(255) NOT NULL,
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
INSERT INTO pc_student.Alltraxs_ContactUs (UserName, Email, Country, Comments) VALUES('Akash Saini', 'akashsaini2611@gmail.com', 'India','Yeh kya bawal bnaya hai');
UPDATE pc_student.Alltraxs_ContactUs SET Username = 'God Knight', Country = 'JAPAN', Comments = 'Horrible website' WHERE UserId = 2;
ALTER TABLE pc_student.Alltraxs_ContactUs CHANGE COLUMN UserId FeedbackId INT AUTO_INCREMENT PRIMARY KEY;
SET SQL_SAFE_UPDATES = 0;
DELETE FROM pc_student.Alltraxs_ContactUs where Feedback_Id=2;
TRUNCATE pc_student.Alltraxs_ContactUs;
SELECT * FROM pc_student.Alltraxs_ContactUs;

CREATE TABLE pc_student.Alltraxs_Subscriptions (
    SubscriptionId INT AUTO_INCREMENT PRIMARY KEY,
    UserId INT NOT NULL UNIQUE,
    UserName VARCHAR(50)  UNIQUE,
    Email VARCHAR(100) NOT NULL UNIQUE,
    PlanType VARCHAR(10) NOT NULL,
    CouponCode VARCHAR(50),
    PaymentDate TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    StartDate DATE NOT NULL,
    EndDate DATE NOT NULL,
    LastUpdated TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    Active BOOLEAN NOT NULL DEFAULT true
);
INSERT INTO pc_student.Alltraxs_Subscriptions (UserId, UserName, Email, PlanType, StartDate, EndDate)
VALUES ('2','Gourav Rattan', 'gouravrattan11@gmail.com','Simple', '2024-07-01', '2024-12-31');
SELECT * FROM pc_student.Alltraxs_Subscriptions;

CREATE TABLE pc_student.Alltraxs_UserSessions (
    Session_Id INT AUTO_INCREMENT PRIMARY KEY,
    UserId INT,
    Token VARCHAR(255) NOT NULL,
    Loggedin TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    Loggedout TIMESTAMP NULL,
    Expirytime TIMESTAMP NULL,
    FOREIGN KEY (UserId) REFERENCES pc_student.Alltraxs_users (UserId)
);
INSERT INTO pc_student.Alltraxs_UserSessions (UserId, Token, Loggedin, Expirytime)VALUES (1, 'session_token_value', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP + INTERVAL 1 HOUR);
SELECT * FROM pc_student.Alltraxs_UserSessions;


