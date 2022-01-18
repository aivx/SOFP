USE [master]
GO

DROP DATABASE SOFP4
GO

USE [master]
GO

IF DB_ID('SOFP4') IS NULL
	CREATE DATABASE SOFP4
ELSE
	PRINT 'SOFP4 ALREADY EXISTS!!!'
GO

USE [SOFP4]
GO

IF OBJECT_ID(N'dbo.Customers', N'U') IS NULL
	BEGIN
		CREATE TABLE Customers(					-- ������� �����������
			[ID] int NOT NULL IDENTITY,			-- �������������
			[FName] nvarchar(20) NOT NULL,		-- ���
			[MName] nvarchar(20) NOT NULL,		-- �������
			[LName] nvarchar(20) NOT NULL,		-- ��������
			[Nname] nvarchar(20) NOT NULL,		-- ���������� ���� (nickname)
			[Address] nvarchar(50) NOT NULL,	-- �����
			[Phone] char(13) NOT NULL			-- ����� ��������
		)
	END
ELSE
	PRINT 'ERROR! ������� Customers ��� ����������!'

IF OBJECT_ID(N'dbo.Orders', N'U') IS NULL
	BEGIN
		CREATE TABLE Orders(					-- ������� ������
			[ID] int NOT NULL IDENTITY,			-- ������������� ������
			[CustomerID] int NULL,				-- ������������� ����������
			[OrderDate] date DEFAULT GETDATE()	-- ���� ������
		)
	END
ELSE
	PRINT 'ERROR! ������� Orders ��� ����������!'

IF OBJECT_ID(N'dbo.Products', N'U') IS NULL
	BEGIN
		CREATE TABLE Products(					-- ������� �������
			[ID] int NOT NULL IDENTITY,			-- �������������
			[Name] nvarchar(50) NOT NULL,		-- ������������ ��������
			[Description] nvarchar(max) NULL	-- ��������
		)
	END
ELSE
	PRINT 'ERROR! ������� Products ��� ����������!'

IF OBJECT_ID(N'dbo.ProductPrice', N'U') IS NULL
	BEGIN
		CREATE TABLE ProductPrice(					
			[ProductID] int NOT NULL,					
			[WPrice] money NOT NULL,					
			[RPrice] money NOT NULL,
		)
	END
ELSE
	PRINT 'ERROR! ������� ProductPrice ��� ����������!'

IF OBJECT_ID(N'dbo.ProductDiscount', N'U') IS NULL
	BEGIN
		CREATE TABLE ProductDiscount(					
			[ProductID] int NOT NULL,						
			[Percentage] tinyint NOT NULL,			
			[From] int NOT NULL,						
			[To] int NOT NULL
		)
	END
ELSE
	PRINT 'ERROR! ������� ProductDiscount ��� ����������!'

IF OBJECT_ID(N'dbo.OrderDetails', N'U') IS NULL
	BEGIN
		CREATE TABLE OrderDetails(				-- ������� ���������� � �������
			[OrderID] int NOT NULL,				-- ������������� ������
			[ProductID] int NULL,
			[Count] int DEFAULT 0,				-- ���������� ������
			[Price] money NULL
		)
	END
ELSE
	PRINT 'ERROR! ������� OrderDetails ��� ����������!'

IF OBJECT_ID(N'dbo.Stocks', N'U') IS NULL
	BEGIN
		CREATE TABLE Stocks
		(
			[ProductID] int NOT NULL,
			[Count] int DEFAULT 0
		)
	END
ELSE
	PRINT 'ERROR! ������� Stocks ��� ����������!'

ALTER TABLE Customers 
ADD
PRIMARY KEY(ID)
GO

ALTER TABLE Orders
ADD
PRIMARY KEY([ID])
GO

ALTER TABLE Products
ADD
PRIMARY KEY(ID)
GO

ALTER TABLE ProductPrice
ADD
UNIQUE(ProductID)
GO

ALTER TABLE ProductDiscount
ADD
UNIQUE(ProductID)
GO

ALTER TABLE OrderDetails
ADD
UNIQUE(OrderID)
GO

ALTER TABLE Stocks
ADD
UNIQUE(ProductID)
GO

-- ��������� ������� Orders � �������� Cuscomers
ALTER TABLE Orders
ADD
FOREIGN KEY ([CustomerID]) REFERENCES Customers([ID])	
	ON UPDATE CASCADE
	ON DELETE SET NULL	--��� �������� �������� �� NULL						
GO
-- ��������� ������� ProductPrice � �������� Products
ALTER TABLE ProductPrice
ADD
FOREIGN KEY ([ProductID]) REFERENCES Products([ID])	
	ON UPDATE CASCADE
	ON DELETE CASCADE	--��������� ��������							
GO
-- ��������� ������� ProductDiscount � �������� Products
ALTER TABLE ProductDiscount
ADD
FOREIGN KEY ([ProductID]) REFERENCES Products([ID])	
	ON UPDATE CASCADE
	ON DELETE CASCADE	--��������� ��������							
GO
-- ��������� ������� OrderDetails � �������� Orders
ALTER TABLE OrderDetails
ADD
FOREIGN KEY ([OrderID]) REFERENCES Orders([ID])	
	ON UPDATE CASCADE
	ON DELETE CASCADE	--��������� ��������								
GO
-- ��������� ������� OrderDetails � �������� Products
ALTER TABLE OrderDetails
ADD
FOREIGN KEY ([ProductID]) REFERENCES Products([ID])	
	ON UPDATE CASCADE
	ON DELETE SET NULL	--��� �������� �������� �� NULL									
GO
-- ��������� ������� Stocks � �������� Products
ALTER TABLE Stocks
ADD
FOREIGN KEY ([ProductID]) REFERENCES Products([ID])	
	ON UPDATE CASCADE
	ON DELETE CASCADE	--��������� ��������
GO

-- ������� ��� �������� ���������
ALTER TABLE ProductDiscount
ADD
CHECK ([Percentage] >= 0 AND [Percentage] <= 100)
GO
-- ���������� ������ �� ������ �� ������ ���� ������ 0
ALTER TABLE Stocks
ADD
CHECK (Count >= 0)
GO
-- ������ ����� ������ ��������
ALTER TABLE Customers
ADD
CHECK (Phone LIKE '[0-9]([0-9][0-9][0-9])[0-9][0-9][0-9][0-9][0-9][0-9][0-9]')
GO

CREATE TRIGGER trMatchingCtocksOnInsert		-- ������� �� insert � ������� OrderDetails
ON dbo.OrderDetails FOR INSERT
AS
	IF @@ROWCOUNT = 0			-- rowcount ���������, ������� ����� �� ���������� ��������
		RETURN
	SET NOCOUNT ON				-- ��������� ��������� � ����� ������������ �������, ���������� ������������������

	UPDATE Stocks SET Count = s.Count - i.Count	-- inserted - ��������� ������� � ������������ �������
	FROM Stocks s join (SELECT ProductID, sum(Count) Count FROM inserted GROUP BY ProductID) i
	ON s.ProductID = i.ProductID
GO


CREATE TRIGGER trMatchingStocksOnDelete		-- ������� �� delete
ON OrderDetails FOR DELETE
AS
	IF @@ROWCOUNT = 0
		RETURN
	SET NOCOUNT ON

	UPDATE Stocks SET Count = s.Count + d.Count	-- ��������� ������� deleted ������ ������, ������� ����� �������
	FROM Stocks s join (SELECT ProductID, sum(Count) Count FROM deleted GROUP BY ProductID) d
	ON s.ProductID = d.ProductID
GO

CREATE TRIGGER trMatchingStocksOnUpdate		-- ������� �� update
ON OrderDetails FOR UPDATE
AS
	IF @@ROWCOUNT = 0
		RETURN

	IF not UPDATE(Count)			-- ���� �� �� ��������� ���� Count, �� ������� �� ��������
		RETURN

	SET NOCOUNT ON

	UPDATE Stocks SET Count = s.Count - (i.Count - d.Count)
	FROM Stocks s
	join	
	(SELECT ProductID, sum(Count) Count FROM deleted GROUP BY ProductID) d
	ON s.ProductID = d.ProductID
	join
	(SELECT ProductID, sum(Count) Count FROM inserted GROUP BY ProductID) i
	ON s.ProductID = i.ProductID
GO


create trigger trAllowDeleteProdut		-- ���� ����� ��������� ������� �� ������� Products ����� ���������,
ON Products								-- �����-�� �������, ���������� INSTEAD OF DELETE
INSTEAD OF DELETE
AS
	BEGIN
		-- ���� � OrderDetails ���� ���� ���� ������, �������� ��������
		IF exists (SELECT 1 FROM OrderDetails od
				   join deleted d
				   ON od.ProductID = d.ID)
			RAISERROR('����� �� ����� ���� ������', 10, 1)
		-- ���� ������� ���, ��������� ��������
		ELSE
			DELETE Products WHERE ID in (SELECT ID FROM deleted)
	END
GO


USE [SOFP4]
GO

CREATE PROCEDURE [add_Order]
	@CustomerID int = 0,
	@ProductID int = 0,
	@Count int = 0
	 
WITH EXECUTE AS OWNER
AS
	declare @OrderID int = 0
	declare @RPrice money = 0
	declare @WPrice money = 0
	declare @From int = 0
	declare @To int = 0
	declare @Percentage tinyint = 0 
	declare @Price money = 0

	SET NOCOUNT ON;

	SELECT @RPrice = A.RPrice, @WPrice = A.WPrice, @From = B.[From], @To = B.[To], @Percentage = B.[Percentage]
    FROM ProductPrice A INNER JOIN ProductDiscount B
    ON	A.ProductID = @ProductID AND B.ProductID = @ProductID

	IF @Count > 1
		set @Price += @WPrice;
	ELSE 
		set @Price += @RPrice;

	IF (@Count >= @From) AND (@Count <= @To)
		set @Price = (@Price/100) * @Percentage;

	INSERT Orders
	([CustomerID])
	VALUES
	(@CustomerID)

	SELECT @OrderID = MAX([ID]) FROM [Orders]

	INSERT OrderDetails
	([OrderID],[ProductID],[Count],[Price])
	VALUES
	(@OrderID, @ProductID, @Count, @Price)
GO

USE SOFP4;   
GRANT EXECUTE ON OBJECT::add_Order
    TO public;  
GO

CREATE PROCEDURE [add_Product]
	@Name varchar(50) = 0,
	@Description varchar(max) = 0,
	@WPrice money = 0,
	@RPrice money = 0,
	@Percentage tinyint = 0,
	@From int = 0,
	@To int = 0,
	@Count int = 0
	 
WITH EXECUTE AS OWNER
AS
	declare @ProductID int = 0

	INSERT Products
	([Name],[Description])
	VALUES
	(@Name, @Description)

	SELECT @ProductID = MAX([ID]) FROM [Products]

	INSERT ProductPrice
	([ProductID],[WPrice],[RPrice])
	VALUES
	(@ProductID,@WPrice, @RPrice)			

	INSERT ProductDiscount
	([ProductID],[Percentage],[From],[To])
	VALUES
	(@ProductID,@Percentage, @From, @To)

	INSERT Stocks
	([ProductID],[Count])
	VALUES
	(@ProductID,@Count)
GO

USE SOFP4;   
GRANT EXECUTE ON OBJECT::add_Product
    TO public;  
GO



INSERT Customers
([FName],[MName],[LName],[Nname],[Address],[Phone])
VALUES
('������1','����1','��������1','Ivan1','������� 2','8(123)1264567'),
('������2','����2','��������2','Ivan2','������� 3','8(123)1234567'),
('������3','����3','��������3','Ivan3','������� 4','8(123)1234567'),
('������4','����4','��������4','Ivan4','������� 5','8(123)8567367'),
('������5','����5','��������5','Ivan5','������� 5','8(123)1433567'),
('������6','����6','��������6','Ivan6','������� 6','8(123)1122367'),
('������7','����7','��������7','Ivan7','������� 7','8(123)1778237'),
('������8','����8','��������8','Ivan8','������� 8','8(123)1244667'),
('������9','����9','��������9','Ivan9','������� 9','8(123)1266667'),
('������10','����10','��������10','Ivan10','������� 10','8(123)1114567')
GO

INSERT Products
([Name],[Description])
VALUES
('�����1','�������� ������1'),
('�����2','�������� ������2'),
('�����3','�������� ������3'),
('�����4','�������� ������4'),
('�����5','�������� ������5'),
('�����6','�������� ������6'),
('�����7','�������� ������7'),
('�����8','�������� ������8'),
('�����9','�������� ������9'),
('�����10','�������� ������10')
GO

INSERT Stocks
([ProductID],[Count])
VALUES
(1,10),
(2,10),
(3,10),
(4,10),
(5,10),
(6,10),
(7,10),
(8,10),
(9,10),
(10,10)
GO


INSERT ProductPrice
([ProductID],[WPrice],[RPrice])
VALUES
(1,10, 20),
(2,10, 20),
(3,10, 20),
(4,10, 20),
(5,10, 20),
(6,10, 20),
(7,10, 20),
(8,10, 20),
(9,10, 20),
(10,10, 20)
GO				

INSERT ProductDiscount
([ProductID],[Percentage],[From],[To])
VALUES
(1,5, 3, 10),
(2,5, 3, 10),
(3,5, 3, 10),
(4,5, 3, 10),
(5,5, 3, 10),
(6,5, 3, 4),
(7,5, 3, 4),
(8,5, 3, 4),
(9,5, 3, 4),
(10,5, 3, 4)
GO	

/******************************************/


INSERT Orders
([CustomerID])
VALUES
(1),
(2),
(3),
(4),
(5),
(6),
(7),
(8),
(9),
(10)
GO

INSERT OrderDetails
([OrderID],[ProductID],[Count],[Price])
VALUES
(1,1, 5, 20),
(2,2, 5, 20),
(3,3, 5, 20),
(4,4, 5, 20),
(5,5, 5, 20),
(6,6, 5, 20),
(7,7, 5, 20),
(8,8, 5, 20),
(9,9, 5, 20),
(10,10, 5, 20)
GO



EXEC [add_Order] 5, 1, 4
GO


create view ShowProducts
as
select FName + ' ' + LName FullName from Customers where ID > 2
go
select * from ShowProducts

SELECT * FROM Products

SELECT * FROM Stocks

SELECT * FROM Orders

SELECT * FROM OrderDetails

DELETE Orders
WHERE Id=2
GO


CREATE LOGIN [@login] WITH PASSWORD=@HashThis, DEFAULT_DATABASE=[SOFP4], DEFAULT_LANGUAGE=[�������], CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF
	GO
	USE [SOFP4]
	GO
	CREATE USER [@login] FOR LOGIN [@login]
	GO
	USE [SOFP4]
	GO
	ALTER USER [@login] WITH DEFAULT_SCHEMA=[dbo]
	GO


CREATE PROCEDURE [get_Order]
	@ID int = 0
	 
WITH EXECUTE AS OWNER
AS

	SELECT F.ID AS [ID ������], G.[LName] AS [��� ����������],A.[Name] AS [������������ ������],
	A.[Description] AS [�������� ������],E.[Count] AS [���������� �������],B.[Count] AS [���������� �� ������],
	[Percentage] AS ������,C.[From] AS [��],C.[To] AS [��],D.[RPrice] AS [��������� ����],
	D.[WPrice] AS [������� ����], E.[Price] AS [���� � ������ ������],F.[OrderDate] AS [���� ������]
	FROM Products A
	INNER JOIN Stocks B ON A.ID = B.ProductID
	INNER JOIN ProductDiscount C ON A.ID = C.ProductID
	INNER JOIN ProductPrice D ON A.ID = D.ProductID
	INNER JOIN OrderDetails E ON A.ID = E.ProductID
	INNER JOIN Orders F ON F.ID = E.OrderID
	INNER JOIN Customers G ON F.[CustomerID] = G.[ID]
	WHERE not @ID<>0 AND F.ID=@ID;

GO

EXEC [get_Order] 0

GO



SELECT * FROM Products

SELECT * FROM ProductPrice

SELECT * FROM ProductDiscount

SELECT * FROM Stocks

SELECT A.[ID] AS [�������������], A.[Name] AS [��������], A.[Description] AS [��������], 
B.[WPrice] AS [������� ����], B.[RPrice] AS [��������� ����], C.[Percentage] AS [������], 
C.[From] AS [��], C.[To] AS [��], D.[Count] AS [����������]
FROM [Products] A 
INNER JOIN [ProductPrice] B ON A.[ID] = B.[ProductID]
INNER JOIN [ProductDiscount] C ON A.[ID] = C.[ProductID]
INNER JOIN [Stocks] D ON A.[ID] = D.[ProductID]
WHERE A.ID=11

UPDATE [Products] SET [Name] = '�����_1' WHERE ID = 1