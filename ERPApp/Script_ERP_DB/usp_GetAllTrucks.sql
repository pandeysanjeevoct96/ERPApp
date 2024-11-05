CREATE PROCEDURE usp_GetAllTrucks
AS
BEGIN
    SET NOCOUNT ON; 

    SELECT TruckId, 
	       TruckCode, 
		   TruckName, 
		   Status, 
		   Description
    FROM 
		Trucks;
END
