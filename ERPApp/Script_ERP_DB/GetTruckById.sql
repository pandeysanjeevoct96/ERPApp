CREATE PROCEDURE GetTruckById
    @TruckId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        TruckId, 
        TruckCode, 
        TruckName, 
        Status, 
        Description 
    FROM 
        Trucks 
    WHERE 
        TruckId = @TruckId;
END
