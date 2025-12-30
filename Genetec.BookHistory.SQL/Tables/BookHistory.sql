if exists (select * from sys.objects where name='BookHistory' and type='U')
	drop table BookHistory
GO

CREATE TABLE BookHistory (
	Id					int				NOT NULL	identity(1,1) PRIMARY KEY,
	BookId				int				NOT NULL,
	OperationDate		datetime		NOT NULL	default getdate(),
	OperationId			tinyint			NOT NULL,
	Title				nvarchar(200)	NULL,
	ShortDescription	nvarchar(2000)	NULL,
	PublishDate			date			NULL,
	Authors				json			NULL,
	CONSTRAINT fkBookHistory1 FOREIGN KEY (BookId) REFERENCES Book(Id)
)
GO
