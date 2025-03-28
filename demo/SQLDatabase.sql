CREATE TABLE Users (
	UserID INT PRIMARY KEY IDENTITY(1,1),
    Username VARCHAR(32) NOT NULL UNIQUE,
	Password_Hash VARCHAR(60) NOT NULL,
);

CREATE TABLE Messages(
	 MessageID INT PRIMARY KEY IDENTITY(1,1),
	 ConversationsID INT NOT NULL,
	 SenderID INT NOT NULL,
	 Content VARCHAR(MAX) NULL,
	 Message_Type VARCHAR(8) NOT NULL,
	 Created_At DATETIME2(3) NOT NULL DEFAULT SYSDATETIME(),
	 FOREIGN KEY (ConversationsID) REFERENCES Users(UserID),
	 FOREIGN KEY (SenderID) REFERENCES Users(UserID)
);

CREATE TABLE Conversations(
	ConversationID INT PRIMARY KEY IDENTITY(1,1),
	UserAID INT NOT NULL,
	UserBID INT NOT NULL,
	FOREIGN KEY (UserAID) REFERENCES Users(UserID),
	FOREIGN KEY (UserBID) REFERENCES Users(UserID),
);