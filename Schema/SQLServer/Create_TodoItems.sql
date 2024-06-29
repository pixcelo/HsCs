CREATE TABLE TodoItems (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    Title NVARCHAR(255) NULL,
    Description NVARCHAR(MAX) NULL,
    IsComplete BIT NOT NULL DEFAULT 0,
    DueDate DATETIME NULL,
    CreatedAt DATETIME NOT NULL,
    UpdatedAt DATETIME NULL,
    DeletedAt DATETIME NULL,
    CONSTRAINT PK_TodoItems PRIMARY KEY CLUSTERED 
(
	Id ASC
));

GO

-- テストデータの削除
TRUNCATE TABLE TodoItems;
GO

-- テストデータの挿入
INSERT INTO TodoItems (Title, Description, IsComplete, DueDate, CreatedAt)
VALUES (N'タイトル1', N'説明1', 0, '2021-01-01', GETDATE()),
       (N'タイトル2', N'説明2', 0, '2021-01-02', GETDATE()),
       (N'タイトル3', N'説明3', 0, '2021-01-03', GETDATE());
GO