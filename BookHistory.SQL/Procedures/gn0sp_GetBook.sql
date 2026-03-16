CREATE OR ALTER PROCEDURE gn0sp_GetBook(
	@Id int
)
AS
	select Title
		,ShortDescription
		,PublishDate
		,IsDeleted
		,RevisionNumber
	from Book
	where Id = @Id
GO
