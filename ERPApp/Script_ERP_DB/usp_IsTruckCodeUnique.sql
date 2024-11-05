CREATE PROCEDURE usp_IsTruckCodeUnique
    @TruckCode NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT COUNT(1) AS TruckCodeCount
    FROM Trucks
    WHERE TruckCode = @TruckCode;
END
