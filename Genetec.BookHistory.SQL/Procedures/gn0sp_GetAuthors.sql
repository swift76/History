CREATE OR ALTER PROCEDURE gn0sp_GetAuthors(
	@BookId int
)
AS
	select Name
	from Author
	where BookId = @BookId
GO
