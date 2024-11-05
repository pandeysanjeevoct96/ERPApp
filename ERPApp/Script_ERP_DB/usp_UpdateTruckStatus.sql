CREATE PROCEDURE usp_UpdateTruckStatus
    @TruckId INT,
    @Status INT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Trucks
    SET Status = @Status
    WHERE TruckId = @TruckId;
END
