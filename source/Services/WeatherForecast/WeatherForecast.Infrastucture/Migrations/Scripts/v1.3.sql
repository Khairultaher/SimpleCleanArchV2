BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20221031051328_v1_3')
BEGIN
EXEC(N'    CREATE OR ALTER VIEW vwLocationTemperatureSummery
    AS
    SELECT 
    	Date,
    	Location,
    	TemperatureC Temperature,
    	Summary
    FROM WeatherForecasts')

END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20221031051328_v1_3')
BEGIN
	EXEC(N'    CREATE OR ALTER PROCEDURE spGetWeatherForecast(@location nvarchar(50))
		AS
		SELECT 
    		Date,
    		Location,
    		TemperatureC Temperature,
    		Summary
		FROM WeatherForecasts
		WHERE LOWER(Location) = LOWER(@location)')
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20221031051328_v1_3')
BEGIN

   EXEC(N' CREATE OR ALTER FUNCTION fnGetTemperatureByLocation
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

    END')

END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20221031051328_v1_3')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20221031051328_v1_3', N'6.0.4');
END;
GO

COMMIT;
GO

