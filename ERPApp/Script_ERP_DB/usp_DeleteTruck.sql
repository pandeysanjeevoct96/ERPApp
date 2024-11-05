CREATE PROCEDURE usp_DeleteTruck
    @TruckId INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM Trucks WHERE TruckId = @TruckId;
END
