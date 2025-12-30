CREATE OR ALTER PROCEDURE gn0sp_InsertBook(
	@Title				nvarchar(200),
	@ShortDescription	nvarchar(2000),
	@PublishDate		date,
	@Authors			Author readonly
)
AS
	declare @Id int
	declare @RevisionNumber	int = 1

	BEGIN TRANSACTION
	BEGIN TRY
		insert into Book (
			Title,
			ShortDescription,
			PublishDate,
			RevisionNumber
		)
		values (
			@Title,
			@ShortDescription,
			@PublishDate,
			@RevisionNumber
		)

		set @Id = SCOPE_IDENTITY()

		insert into Author (
			BookId,
			Name
		)
		select @Id,
			Name
		from @Authors

		declare @AuthorsSerialized json =
		(
			select *
			from @Authors
			FOR JSON PATH
		)

		insert into BookHistory (
			BookId,
			OperationId,
			Title,
			ShortDescription,
			PublishDate,
			Authors
		)
		values (
			@Id,
			1,
			@Title,
			@ShortDescription,
			@PublishDate,
			@AuthorsSerialized
		)
	END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION
		declare @ErrorMessage varchar(4000)
		set @ErrorMessage = ERROR_MESSAGE()
		raiserror (@ErrorMessage, 17, 1)
		return
	END CATCH
	COMMIT TRANSACTION

	select @Id as Id,@RevisionNumber as RevisionNumber
GO
