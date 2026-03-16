if exists (select * from sys.objects where name='Author' and type='U')
	drop table Author
GO

CREATE TABLE Author (
	Id		int				NOT NULL	identity(1,1),
	BookId	int				NOT NULL,
	Name	nvarchar(200)	NOT NULL	default 0,
	CONSTRAINT fkAuthor1 FOREIGN KEY (BookId) REFERENCES Book(Id)
)
GO

CREATE UNIQUE CLUSTERED INDEX iAuthor1 ON Author(BookId, Id)
GO
