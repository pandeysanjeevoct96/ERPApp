CREATE PROCEDURE UpdateTruck
    @TruckId INT,
    @TruckCode NVARCHAR(50),
    @TruckName NVARCHAR(100),
    @Status NVARCHAR(20),
    @Description NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Trucks
    SET 
        TruckCode = @TruckCode, 
        TruckName = @TruckName, 
        Status = @Status, 
        Description = @Description
    WHERE 
        TruckId = @TruckId;

    IF @@ROWCOUNT = 0
    BEGIN
        RAISERROR('Update failed. No rows were affected.', 16, 1);
    END
END
