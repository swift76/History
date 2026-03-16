if exists (select * from sys.types where name='Author')
	DROP TYPE Author
GO

CREATE TYPE Author AS TABLE
(
	Name	nvarchar(200)
)
GO
