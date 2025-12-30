CREATE OR ALTER PROCEDURE gn0sp_UpdateBook(
	@Id					int,
	@Title				nvarchar(200) = null,
	@ShortDescription	nvarchar(2000) = null,
	@PublishDate		date = null,
	@Authors			Author readonly,
	@RevisionNumber		int
)
AS
	BEGIN TRANSACTION
	BEGIN TRY
		declare @CurrentRevisionNumber int,@CurrentIsDeleted bit
		select @CurrentRevisionNumber = RevisionNumber
			,@CurrentIsDeleted = IsDeleted
		from Book with (UPDLOCK)
		where Id = @Id

		if @CurrentIsDeleted is null
			RAISERROR ('The book with the specified ID is not found', 17, 1)

		if @CurrentIsDeleted = 1
			RAISERROR ('The book with the specified ID is already deleted', 17, 1)

		if @CurrentRevisionNumber != @RevisionNumber
			RAISERROR ('The book with the specified ID is already changed', 17, 1)

		set @RevisionNumber = @RevisionNumber + 1

		update Book set Title = isnull(@Title, Title)
			,ShortDescription = isnull(@ShortDescription, ShortDescription)
			,PublishDate = isnull(@PublishDate, PublishDate)
			,RevisionNumber = @RevisionNumber
		where Id = @Id

		declare @AuthorsSerialized json

		if (select count(*) from @Authors) > 0
		begin
			delete from Author where BookId = @Id

			insert into Author (
				BookId,
				Name
			)
			select @Id,
				Name
			from @Authors

			set @AuthorsSerialized =
			(
				select *
				from @Authors
				FOR JSON PATH
			)
		end

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
			2,
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

	select @RevisionNumber as RevisionNumber
GO
