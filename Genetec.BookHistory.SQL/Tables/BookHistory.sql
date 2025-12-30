if exists (select * from sys.objects where name='BookHistory' and type='U')
	drop table BookHistory
GO

CREATE TABLE BookHistory (
	Id					int			NOT NULL	identity(1,1) PRIMARY KEY,
	BookId				int			NOT NULL,
	OperationDate		datetime	NOT NULL	default getdate(),
	OperationId			tinyint		NOT NULL,
	ChangeDescription	json		NULL
)
GO
