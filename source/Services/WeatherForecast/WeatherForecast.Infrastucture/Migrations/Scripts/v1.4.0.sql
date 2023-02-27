BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230227105922_v1_4')
BEGIN
    ALTER TABLE [dbo].[WeatherForecasts] ADD [ValidFrom] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230227105922_v1_4')
BEGIN
    ALTER TABLE [dbo].[WeatherForecasts] ADD [ValidTo] datetime2 NOT NULL DEFAULT '9999-12-31T23:59:59.9999999';
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230227105922_v1_4')
BEGIN
    EXEC(N'ALTER TABLE [dbo].[WeatherForecasts] ADD PERIOD FOR SYSTEM_TIME ([ValidFrom], [ValidTo])')
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230227105922_v1_4')
BEGIN
    ALTER TABLE [dbo].[WeatherForecasts] ALTER COLUMN [ValidFrom] ADD HIDDEN
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230227105922_v1_4')
BEGIN
    ALTER TABLE [dbo].[WeatherForecasts] ALTER COLUMN [ValidTo] ADD HIDDEN
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230227105922_v1_4')
BEGIN
    ALTER TABLE [dbo].[WeatherForecasts] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [dbo].[WeatherForecastsHistory]))

END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230227105922_v1_4')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20230227105922_v1_4', N'6.0.14');
END;
GO

COMMIT;
GO

