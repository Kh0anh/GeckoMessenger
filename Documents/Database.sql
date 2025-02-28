-- Tạo bảng Privacy trước vì được tham chiếu bởi UserSettings
CREATE TABLE Privacy (
    PrivacyID TINYINT IDENTITY(1,1) PRIMARY KEY,
    PrivacyName VARCHAR(16) NOT NULL
);

-- Bảng Users (Bảng chính)
CREATE TABLE Users (
    UserID INT IDENTITY(1,1) PRIMARY KEY,
    Username VARCHAR(32) NOT NULL,
    Email VARCHAR(32) NOT NULL,
    PhoneNumber VARCHAR(16) NOT NULL,
    Birthday DATE NOT NULL,
    HashPassword VARCHAR(60) NOT NULL,
    FirstName VARCHAR(20) COLLATE Vietnamese_CI_AS NOT NULL,
    LastName VARCHAR(20) COLLATE Vietnamese_CI_AS NOT NULL,
    Bio VARCHAR(255) COLLATE Vietnamese_CI_AS NOT NULL,
    Avatar VARCHAR(128) NOT NULL,
    LastLogin DATETIME NOT NULL,
    CreatedAt DATETIME DEFAULT SYSDATETIME() NOT NULL
);

-- Bảng UserSettings (tham chiếu đến Users và Privacy)
CREATE TABLE UserSettings (
    UserID INT PRIMARY KEY,
    StatusPrivacy TINYINT NOT NULL,
    BioPrivacy TINYINT NOT NULL,
    PhoneNumberPrivacy TINYINT NOT NULL,
    EmailPrivacy TINYINT NOT NULL,
    BirthdayPrivacy TINYINT NOT NULL,
    CallPrivacy TINYINT NOT NULL,
    InviteGroupPrivacy TINYINT NOT NULL,
    MessagePrivacy TINYINT NOT NULL,
    FOREIGN KEY (UserID) REFERENCES Users(UserID),
    FOREIGN KEY (StatusPrivacy) REFERENCES Privacy(PrivacyID),
    FOREIGN KEY (BioPrivacy) REFERENCES Privacy(PrivacyID),
    FOREIGN KEY (PhoneNumberPrivacy) REFERENCES Privacy(PrivacyID),
    FOREIGN KEY (EmailPrivacy) REFERENCES Privacy(PrivacyID),
    FOREIGN KEY (BirthdayPrivacy) REFERENCES Privacy(PrivacyID),
    FOREIGN KEY (CallPrivacy) REFERENCES Privacy(PrivacyID),
    FOREIGN KEY (InviteGroupPrivacy) REFERENCES Privacy(PrivacyID),
    FOREIGN KEY (MessagePrivacy) REFERENCES Privacy(PrivacyID)
);

-- Bảng Contacts (khóa chính kết hợp)
CREATE TABLE Contacts (
    ContactID INT NOT NULL,
    UserID INT NOT NULL,
    AddedAt DATETIME NOT NULL,
    BlockAt DATETIME,
    PRIMARY KEY (ContactID, UserID),
    FOREIGN KEY (ContactID) REFERENCES Users(UserID),
    FOREIGN KEY (UserID) REFERENCES Users(UserID)
);

-- Bảng Managers
CREATE TABLE Managers (
    ManagerID INT IDENTITY(1,1) PRIMARY KEY,
    Username VARCHAR(32) NOT NULL,
    FirstName VARCHAR(20) COLLATE Vietnamese_CI_AS NOT NULL,
    LastName VARCHAR(20) COLLATE Vietnamese_CI_AS NOT NULL,
    HashPassword VARCHAR(60) NOT NULL
);

-- Bảng BannedAccounts (tham chiếu đến Managers và Users)
CREATE TABLE BannedAccounts (
    BanID INT IDENTITY(1,1) PRIMARY KEY,
    CreatorID INT NOT NULL,
    BannedID INT NOT NULL,
    Reason VARCHAR(MAX) NOT NULL,
    Expired DATETIME NOT NULL,
    CreateAt DATETIME DEFAULT SYSDATETIME() NOT NULL,
    FOREIGN KEY (CreatorID) REFERENCES Managers(ManagerID),
    FOREIGN KEY (BannedID) REFERENCES Users(UserID)
);

-- Bảng ReportStatus
CREATE TABLE ReportStatus (
    ReportStatusID TINYINT IDENTITY(1,1) PRIMARY KEY,
    ReportStatusName VARCHAR(16) NOT NULL
);

-- Bảng Conversations (tham chiếu đến Users và các bảng khác)
CREATE TABLE Conversations (
    ConversationID INT IDENTITY(1,1) PRIMARY KEY,
    ConversationName VARCHAR(32),
    ConversationTitle VARCHAR(32) COLLATE Vietnamese_CI_AS,
    CreatorID INT NOT NULL,
    ConversationTypeID TINYINT NOT NULL,
    GroupTypeID TINYINT,
    CreatedAt DATETIME NOT NULL,
    FOREIGN KEY (CreatorID) REFERENCES Users(UserID)
);

-- Bảng ConversationType
CREATE TABLE ConversationType (
    ConversationTypeID TINYINT IDENTITY(1,1) PRIMARY KEY,
    ConversationTypeName VARCHAR(16) NOT NULL
);

-- Bảng GroupType
CREATE TABLE GroupType (
    GroupTypeID TINYINT IDENTITY(1,1) PRIMARY KEY,
    GroupTypeName VARCHAR(16) NOT NULL
);

-- Bảng Participants (tham chiếu đến Conversations và Users)
CREATE TABLE Participants (
    ParticipantID INT IDENTITY(1,1) PRIMARY KEY,
    ConversationID INT NOT NULL,
    UserID INT NOT NULL,
    NickName VARCHAR(32) COLLATE Vietnamese_CI_AS,
    ConversationRoleID TINYINT NOT NULL,
    CreatedAt DATETIME NOT NULL,
    DeleteData DATETIME,
    FOREIGN KEY (ConversationID) REFERENCES Conversations(ConversationID),
    FOREIGN KEY (UserID) REFERENCES Users(UserID)
);

-- Bảng ConversationRole
CREATE TABLE ConversationRole (
    ConversationRoleID TINYINT IDENTITY(1,1) PRIMARY KEY,
    ConversationRoleName VARCHAR(16) NOT NULL
);

-- Bảng Messages (tham chiếu đến Conversations và Users)
CREATE TABLE Messages (
    MessageID INT IDENTITY(1,1) PRIMARY KEY,
    ConversationID INT NOT NULL,
    SenderID INT NOT NULL,
    Content VARCHAR(MAX) COLLATE Vietnamese_CI_AS NOT NULL,
    MessageType TINYINT NOT NULL,
    CreatedAt DATETIME NOT NULL,
    FOREIGN KEY (ConversationID) REFERENCES Conversations(ConversationID),
    FOREIGN KEY (SenderID) REFERENCES Users(UserID)
);

-- Bảng MessageType
CREATE TABLE MessageType (
    MessageTypeID TINYINT IDENTITY(1,1) PRIMARY KEY,
    MessageTypeName VARCHAR(16) NOT NULL
);

-- Bảng Attachments (tham chiếu đến Messages)
CREATE TABLE Attachments (
    AttachmentID INT IDENTITY(1,1) PRIMARY KEY,
    MessageID INT NOT NULL,
    AttachmentTypeID TINYINT NOT NULL,
    ThumbnailURL VARCHAR(128) NOT NULL,
    FileURL VARCHAR(128) NOT NULL,
    CreatedAt DATETIME NOT NULL,
    DeleteDate DATETIME,
    FOREIGN KEY (MessageID) REFERENCES Messages(MessageID)
);

-- Bảng AttachmentType
CREATE TABLE AttachmentType (
    AttachmentTypeID TINYINT IDENTITY(1,1) PRIMARY KEY,
    AttachmentTypeName VARCHAR(16) NOT NULL
);

-- Bảng MessageDelete (tham chiếu đến Messages và Users)
CREATE TABLE MessageDelete (
    MessageDeleteID INT IDENTITY(1,1) PRIMARY KEY,
    MessageID INT NOT NULL,
    DeleteByUserID INT NOT NULL,
    DeleteTypeID TINYINT NOT NULL,
    CreatedAt DATETIME NOT NULL,
    FOREIGN KEY (MessageID) REFERENCES Messages(MessageID),
    FOREIGN KEY (DeleteByUserID) REFERENCES Users(UserID)
);

-- Bảng DeleteType
CREATE TABLE DeleteType (
    DeleteTypeID TINYINT IDENTITY(1,1) PRIMARY KEY,
    DeleteTypeName VARCHAR(16) NOT NULL
);

-- Thêm khóa ngoại cho các bảng phụ thuộc sau
ALTER TABLE Conversations
ADD FOREIGN KEY (ConversationTypeID) REFERENCES ConversationType(ConversationTypeID),
    FOREIGN KEY (GroupTypeID) REFERENCES GroupType(GroupTypeID);

ALTER TABLE Participants
ADD FOREIGN KEY (ConversationRoleID) REFERENCES ConversationRole(ConversationRoleID);

ALTER TABLE Messages
ADD FOREIGN KEY (MessageType) REFERENCES MessageType(MessageTypeID);

ALTER TABLE Attachments
ADD FOREIGN KEY (AttachmentTypeID) REFERENCES AttachmentType(AttachmentTypeID);

ALTER TABLE MessageDelete
ADD FOREIGN KEY (DeleteTypeID) REFERENCES DeleteType(DeleteTypeID);