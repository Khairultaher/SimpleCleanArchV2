
CREATE OR ALTER FUNCTION fnGetTemperatureByLocation
(

	@location NVARCHAR(50)
)
RETURNS int
AS
BEGIN

	DECLARE @Temperature int

	SELECT TOP(1)
		@Temperature = TemperatureC
	FROM WeatherForecasts
	WHERE LOWER(Location) = LOWER(@location)
	ORDER BY Id DESC

	RETURN @Temperature

END

