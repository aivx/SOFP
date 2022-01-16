USE [master]
GO

GO
IF DB_ID('SOFP4') IS NULL
	CREATE DATABASE SOFP4
	-- настройка сортировки
	COLLATE Cyrillic_General_CI_AS
ELSE
	PRINT 'SOFP4 ALREADY EXISTS!!!'
GO

USE [SOFP4]
GO

CREATE TABLE Customers(							-- Таблица покупателей
	[ID] int NOT NULL IDENTITY,					-- Идентификатор
	[FName] nvarchar(20) NULL,					-- Имя
	[MName] nvarchar(20) NULL,					-- Фамилия
	[LName] nvarchar(20) NULL,					-- Отчество
	[Nname] nvarchar(20) NULL,					-- Контактное лицо (nickname)
	[Address] nvarchar(50) NULL,				-- Адрес
	[Phone] char(13) NULL						-- Номер телефона
)
GO

ALTER TABLE Customers 
ADD
PRIMARY KEY(ID)
GO

-- ограничение на ввода номера телефона
ALTER TABLE Customers
ADD
CHECK (Phone LIKE '[0-9]([0-9][0-9][0-9])[0-9][0-9][0-9][0-9][0-9][0-9][0-9]')
GO

CREATE TABLE Orders(							-- Таблица сделок
	[ID] int NOT NULL IDENTITY,					-- Идентификатор сделки
	[CustomerID] int NULL,						-- Идентификатор покупателя
	[OrderDate] date DEFAULT GETDATE()			-- Дата сделки
)
GO

ALTER TABLE Orders
ADD
PRIMARY KEY([ID])
GO

-- связываем таблицу Orders с таблицей Cuscomers
ALTER TABLE Orders
ADD
FOREIGN KEY ([CustomerID]) REFERENCES Customers([ID])	
	ON DELETE SET NULL							
GO

CREATE TABLE Products(							-- Таблица товаров
	[ID] int NOT NULL IDENTITY,					-- Идентификатор
	[Name] nvarchar(50) NOT NULL,				-- Наименование продукта
	[Description] nvarchar(max) NULL			-- Описание
)
GO

ALTER TABLE Products
ADD
PRIMARY KEY(ID)
GO


CREATE TABLE ProductPrice(					
	[ProductID] int NOT NULL,					
	[WPrice] money NOT NULL,					
	[RPrice] money NOT NULL,
)
GO

ALTER TABLE ProductPrice
ADD
UNIQUE(ProductID)
GO

ALTER TABLE ProductPrice
ADD
FOREIGN KEY ([ProductID]) REFERENCES Products([ID])	
	ON DELETE CASCADE								
GO

CREATE TABLE ProductDiscount(					
	[ProductID] int NOT NULL,						
	[Percentage] tinyint NOT NULL,			
	[From] int NOT NULL,						
	[To] int NOT NULL
)
GO

ALTER TABLE ProductDiscount
ADD
CHECK ([Percentage] >= 0 AND [Percentage] <= 100)
GO

ALTER TABLE ProductDiscount
ADD
UNIQUE(ProductID)
GO

ALTER TABLE ProductDiscount
ADD
FOREIGN KEY ([ProductID]) REFERENCES Products([ID])	
	ON DELETE CASCADE								
GO

CREATE TABLE OrderDetails(						-- Таблица информации о сделках
	[OrderID] int NOT NULL,						-- Идентификатор сделки
	[ProductID] int NULL,
	[Count] int DEFAULT 0,						-- Количество товара
	[Price] money NULL
)
GO

ALTER TABLE OrderDetails
ADD
UNIQUE(OrderID)
GO

ALTER TABLE OrderDetails
ADD
FOREIGN KEY ([OrderID]) REFERENCES Orders([ID])	
	ON DELETE CASCADE								
GO

ALTER TABLE OrderDetails
ADD
FOREIGN KEY ([ProductID]) REFERENCES Products([ID])	
	ON DELETE SET NULL									
GO

CREATE TABLE Stocks
(
	[ProductID] int NOT NULL,
	[Count] int DEFAULT 0
)

ALTER TABLE Stocks
ADD
CHECK (Count >= 0)
GO

ALTER TABLE Stocks
ADD
UNIQUE(ProductID)
GO

ALTER TABLE Stocks
ADD
FOREIGN KEY ([ProductID]) REFERENCES Products([ID])	
	ON DELETE CASCADE	
GO

create trigger trMatchingCtocksOnInsert		-- триггер на insert в таблицу OrderDetails
on dbo.OrderDetails for insert
as
	if @@ROWCOUNT = 0			-- rowcount показыает, сколько строк мы собираемся вставить
		return
	set nocount on				-- отключаем сообщения о числе обработанных записей, увеличивая производительность

	update Stocks set Count = s.Count - i.Count	-- inserted - служебная таблица с вставляемыми данными
	from Stocks s join (select ProductID, sum(Count) Count from inserted group by ProductID) i
	on s.ProductID = i.ProductID
go


create trigger trMatchingStocksOnDelete		-- триггер на delete
on OrderDetails for delete
as
	if @@ROWCOUNT = 0
		return
	set nocount on

	update Stocks set Count = s.Count + d.Count	-- служебная таблица deleted хранит записи, которые будут удалены
	from Stocks s join (select ProductID, sum(Count) Count from deleted group by ProductID) d
	on s.ProductID = d.ProductID
go

create trigger trMatchingStocksOnUpdate		-- триггер на update
on OrderDetails for update
as
	if @@ROWCOUNT = 0
		return

	if not UPDATE(Count)			-- если мы не обновляем поле Count, то выходим из триггера
		return

	set nocount on

	update Stocks set Count = s.Count - (i.Count - d.Count)
	from Stocks s
	join	
	(select ProductID, sum(Count) Count from deleted group by ProductID) d
	on s.ProductID = d.ProductID
	join
	(select ProductID, sum(Count) Count from inserted group by ProductID) i
	on s.ProductID = i.ProductID
go


create trigger trAllowDeleteProdut		-- если перед удалением записей из таблицы Products нужно проверить,
on Products					-- можно-ли удалять, используем instead of delete
instead of delete
as
	begin
		-- если в OrderDetails есть хоть одна запись, отменяем удаление
		if exists (select 1 from OrderDetails od
				   join deleted d
				   on od.ProductID = d.ID)
			raiserror('Товар не может быть удален', 10, 1)
		-- если записей нет, выполняем удаление
		else
			delete Products where ID in (select ID from deleted)
	end
go


INSERT Customers
([FName],[MName],[LName],[Nname],[Address],[Phone])
VALUES
('Иванов1','Иван1','Иванович1','Ivan1','Медовая 2','8(123)1264567'),
('Иванов2','Иван2','Иванович2','Ivan2','Медовая 3','8(123)1234567'),
('Иванов3','Иван3','Иванович3','Ivan3','Медовая 4','8(123)1234567'),
('Иванов4','Иван4','Иванович4','Ivan4','Медовая 5','8(123)8567367'),
('Иванов5','Иван5','Иванович5','Ivan5','Медовая 5','8(123)1433567'),
('Иванов6','Иван6','Иванович6','Ivan6','Медовая 6','8(123)1122367'),
('Иванов7','Иван7','Иванович7','Ivan7','Медовая 7','8(123)1778237'),
('Иванов8','Иван8','Иванович8','Ivan8','Медовая 8','8(123)1244667'),
('Иванов9','Иван9','Иванович9','Ivan9','Медовая 9','8(123)1266667'),
('Иванов10','Иван10','Иванович10','Ivan10','Медовая 10','8(123)1114567')
GO

INSERT Products
([Name],[Description])
VALUES
('товар1','Описание товара1'),
('товар2','Описание товара2'),
('товар3','Описание товара3'),
('товар4','Описание товара4'),
('товар5','Описание товара5'),
('товар6','Описание товара6'),
('товар7','Описание товара7'),
('товар8','Описание товара8'),
('товар9','Описание товара9'),
('товар10','Описание товара10')
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


CREATE LOGIN [@login] WITH PASSWORD=@HashThis, DEFAULT_DATABASE=[SOFP4], DEFAULT_LANGUAGE=[русский], CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF
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

	SELECT F.ID AS [ID сделки], G.[LName] AS [Имя покупателя],A.[Name] AS [Наименование товара],
	A.[Description] AS [Описание товара],E.[Count] AS [Количество куплено],B.[Count] AS [Количество на складе],
	[Percentage] AS скидка,C.[From] AS [От],C.[To] AS [До],D.[RPrice] AS [Розничная цена],
	D.[WPrice] AS [Оптовая цена], E.[Price] AS [цена с учетом скидки],F.[OrderDate] AS [дата сделки]
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

EXEC [add_Product] 'test1', 'test1_описание', 10, 20, 5, 3, 20, 10

GO

SELECT * FROM Products

SELECT * FROM ProductPrice

SELECT * FROM ProductDiscount

SELECT * FROM Stocks

SELECT A.[ID] AS [Идентификатор], A.[Name] AS [Название], A.[Description] AS [Описание], 
B.[WPrice] AS [Оптовая цена], B.[RPrice] AS [Розничная цена], C.[Percentage] AS [Скидка], 
C.[From] AS [От], C.[To] AS [До], D.[Count] AS [Количество]
FROM [Products] A 
INNER JOIN [ProductPrice] B ON A.[ID] = B.[ProductID]
INNER JOIN [ProductDiscount] C ON A.[ID] = C.[ProductID]
INNER JOIN [Stocks] D ON A.[ID] = D.[ProductID]
WHERE A.ID=11

UPDATE [Products] SET [Name] = 'товар_1' WHERE ID = 1