CREATE OR ALTER PROCEDURE gn0sp_DeleteBook(
	@Id	int
)
AS
	BEGIN TRANSACTION
	BEGIN TRY
		declare @CurrentIsDeleted bit
		select @CurrentIsDeleted = IsDeleted
		from Book with (UPDLOCK)
		where Id = @Id

		if @CurrentIsDeleted is null
			RAISERROR ('The book with the specified ID is not found', 17, 1)

		if @CurrentIsDeleted = 1
			RAISERROR ('The book with the specified ID is already deleted', 17, 1)

		update Book set IsDeleted = 1
		where Id = @Id

		insert into BookHistory (
			BookId,
			OperationId
		)
		values (
			@Id,
			3
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
GO
