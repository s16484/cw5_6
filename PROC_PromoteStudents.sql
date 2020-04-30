CREATE PROCEDURE PromoteStudents @Studies NVARCHAR(100), @Semester INT
AS
BEGIN
	SET XACT_ABORT ON;
	BEGIN TRAN

	DECLARE @IdStudies INT = (SELECT IdStudy FROM Studies WHERE Name=@Studies);
	IF @IdStudies IS NULL
	BEGIN
		RAISERROR ('Studia nie istnieja',10,1); 
		RETURN;
	END

	DECLARE @IdEnrollment INT = (SELECT IdEnrollment FROM Enrollment, Studies 
								WHERE Enrollment.idstudy = Studies.idstudy 
									AND Enrollment.Semester=(@Semester+1) 
									AND Studies.Name=@Studies
							);

	IF @IdEnrollment IS NULL
	BEGIN
		SET @IdEnrollment = (SELECT MAX(ISNULL(IdEnrollment,0))+1 FROM Enrollment);
		INSERT INTO Enrollment (IdEnrollment, Semester, IdStudy, StartDate) 
			VALUES (@IdEnrollment, @Semester+1,@IdStudies,GETDATE())
	END

	UPDATE Student
	SET IdEnrollment = @IdEnrollment
	WHERE IdEnrollment = (SELECT IdEnrollment FROM Enrollment, Studies 
							WHERE Enrollment.idstudy = Studies.idstudy 
								AND Enrollment.Semester=@Semester 
								AND Studies.Name=@Studies
							);
	COMMIT
END;

