if exists (select * from sys.objects where name='Author' and type='U')
	drop table Author
GO

CREATE TABLE Author (
	Id		int				NOT NULL	identity(1,1) PRIMARY KEY,
	BookId	int				NOT NULL,
	Name	nvarchar(200)	NOT NULL	default 0
)
GO
