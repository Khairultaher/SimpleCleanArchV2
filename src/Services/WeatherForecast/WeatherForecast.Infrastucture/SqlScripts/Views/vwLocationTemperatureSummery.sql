CREATE OR ALTER VIEW vwLocationTemperatureSummery
AS
SELECT 
	Date,
	Location,
	TemperatureC Temperature,
	Summary
FROM WeatherForecasts
