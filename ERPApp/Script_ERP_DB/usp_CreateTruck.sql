CREATE PROCEDURE usp_CreateTruck
    @TruckCode NVARCHAR(50),
    @TruckName NVARCHAR(100),
    @Status NVARCHAR(20),
    @Description NVARCHAR(255) = NULL
AS
BEGIN

    IF EXISTS (SELECT 1 FROM Trucks WHERE TruckCode = @TruckCode)
    BEGIN        
        RAISERROR('A truck with this code already exists.', 16, 1);
        RETURN;
    END

    INSERT INTO Trucks (TruckCode, TruckName, Status, Description)
    VALUES (@TruckCode, @TruckName, @Status, @Description);
END;
GO
