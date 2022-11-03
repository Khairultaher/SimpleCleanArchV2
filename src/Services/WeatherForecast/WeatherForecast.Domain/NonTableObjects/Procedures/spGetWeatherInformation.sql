CREATE OR ALTER PROCEDURE spGetWeatherForecast(@location nvarchar(50))
AS
SELECT 
	Date,
	Location,
	TemperatureC Temperature,
	Summary
FROM WeatherForecasts
WHERE LOWER(Location) = LOWER(@location)