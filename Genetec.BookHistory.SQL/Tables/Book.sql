if exists (select * from sys.objects where name='Book' and type='U')
	drop table Book
GO

CREATE TABLE Book (
	Id					int				NOT NULL	identity(1,1) PRIMARY KEY,
	Title				nvarchar(200)	NOT NULL,
	ShortDescription	nvarchar(2000)	NOT NULL,
	PublishDate			date			NOT NULL,
	IsDeleted			bit				NOT NULL	default 0
)
GO
