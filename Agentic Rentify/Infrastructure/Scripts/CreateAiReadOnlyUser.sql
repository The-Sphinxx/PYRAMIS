-- =============================================
-- Create AI Agent Read-Only Database User
-- =============================================
-- This script creates a dedicated SQL Server login and user
-- with SELECT-only permissions for AI agent database queries.
-- 
-- Security: Prevents AI from modifying data (INSERT/UPDATE/DELETE)
-- Usage: Run this on your SQL Server instance before deploying to production
-- =============================================

USE [master]
GO

-- =============================================
-- Step 1: Create Login (Server-level)
-- =============================================
-- Change the password to a strong value in production
IF NOT EXISTS (SELECT * FROM sys.server_principals WHERE name = 'ai_readonly_user')
BEGIN
    CREATE LOGIN [ai_readonly_user] 
    WITH PASSWORD = N'AiAgent@ReadOnly2025!',
         DEFAULT_DATABASE = [PyramisDb],
         CHECK_EXPIRATION = OFF,
         CHECK_POLICY = ON;
    
    PRINT 'Login [ai_readonly_user] created successfully.';
END
ELSE
BEGIN
    PRINT 'Login [ai_readonly_user] already exists.';
END
GO

-- =============================================
-- Step 2: Switch to Target Database
-- =============================================
USE [PyramisDb]
GO

-- =============================================
-- Step 3: Create User (Database-level)
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = 'ai_readonly_user')
BEGIN
    CREATE USER [ai_readonly_user] FOR LOGIN [ai_readonly_user];
    PRINT 'User [ai_readonly_user] created in database [PyramisDb].';
END
ELSE
BEGIN
    PRINT 'User [ai_readonly_user] already exists in database [PyramisDb].';
END
GO

-- =============================================
-- Step 4: Grant SELECT Permissions on Allowed Tables
-- =============================================

-- Hotels table (accommodation data)
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Hotels')
BEGIN
    GRANT SELECT ON [dbo].[Hotels] TO [ai_readonly_user];
    PRINT 'SELECT permission granted on [Hotels]';
END

-- Cars table (vehicle rental data)
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Cars')
BEGIN
    GRANT SELECT ON [dbo].[Cars] TO [ai_readonly_user];
    PRINT 'SELECT permission granted on [Cars]';
END

-- Attractions table (tourist sites data)
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Attractions')
BEGIN
    GRANT SELECT ON [dbo].[Attractions] TO [ai_readonly_user];
    PRINT 'SELECT permission granted on [Attractions]';
END

-- Trips table (organized tours data)
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Trips')
BEGIN
    GRANT SELECT ON [dbo].[Trips] TO [ai_readonly_user];
    PRINT 'SELECT permission granted on [Trips]';
END

-- Bookings table (reservation data - for availability checks)
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Bookings')
BEGIN
    GRANT SELECT ON [dbo].[Bookings] TO [ai_readonly_user];
    PRINT 'SELECT permission granted on [Bookings]';
END

-- Reviews table (user feedback data)
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Reviews')
BEGIN
    GRANT SELECT ON [dbo].[Reviews] TO [ai_readonly_user];
    PRINT 'SELECT permission granted on [Reviews]';
END

-- CarReviews table (vehicle feedback)
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'CarReviews')
BEGIN
    GRANT SELECT ON [dbo].[CarReviews] TO [ai_readonly_user];
    PRINT 'SELECT permission granted on [CarReviews]';
END

-- HotelReviews table (accommodation feedback)
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'HotelReviews')
BEGIN
    GRANT SELECT ON [dbo].[HotelReviews] TO [ai_readonly_user];
    PRINT 'SELECT permission granted on [HotelReviews]';
END

-- AttractionReviews table (site feedback)
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'AttractionReviews')
BEGIN
    GRANT SELECT ON [dbo].[AttractionReviews] TO [ai_readonly_user];
    PRINT 'SELECT permission granted on [AttractionReviews]';
END

GO

-- =============================================
-- Step 5: Grant INFORMATION_SCHEMA Access (for schema queries)
-- =============================================
-- This allows the AI to query table structures via get_table_schema()
GRANT VIEW DEFINITION TO [ai_readonly_user];
PRINT 'VIEW DEFINITION permission granted (for schema introspection)';
GO

-- =============================================
-- Step 6: Explicitly DENY Dangerous Permissions
-- =============================================
-- Extra safety: explicitly deny write operations
DENY INSERT TO [ai_readonly_user];
DENY UPDATE TO [ai_readonly_user];
DENY DELETE TO [ai_readonly_user];
DENY ALTER TO [ai_readonly_user];
DENY CREATE TABLE TO [ai_readonly_user];
DENY EXECUTE TO [ai_readonly_user];

PRINT 'Write permissions explicitly denied for safety.';
GO

-- =============================================
-- Step 7: Verification Query
-- =============================================
PRINT '';
PRINT '================================================';
PRINT 'VERIFICATION: Permissions Summary';
PRINT '================================================';

SELECT 
    dp.name AS [User],
    o.name AS [Table],
    p.permission_name AS [Permission],
    p.state_desc AS [State]
FROM sys.database_permissions p
INNER JOIN sys.database_principals dp ON p.grantee_principal_id = dp.principal_id
INNER JOIN sys.objects o ON p.major_id = o.object_id
WHERE dp.name = 'ai_readonly_user'
ORDER BY o.name, p.permission_name;

PRINT '';
PRINT '================================================';
PRINT 'Setup Complete!';
PRINT '================================================';
PRINT 'Next Steps:';
PRINT '1. Update appsettings.json with connection string:';
PRINT '   "AiReadOnlyConnection": "Server=YOUR_SERVER;Database=PyramisDb;User Id=ai_readonly_user;Password=AiAgent@ReadOnly2025!;TrustServerCertificate=True;"';
PRINT '2. Test connection from application';
PRINT '3. Monitor SQL Server logs for AI queries';
PRINT '================================================';
GO

-- =============================================
-- Optional: Test Query (Safe to run)
-- =============================================
-- Uncomment to test if the user can query data

/*
EXECUTE AS USER = 'ai_readonly_user';
    SELECT TOP 5 Name, City FROM Hotels WHERE City = 'Cairo';
REVERT;
PRINT 'Test query executed successfully as ai_readonly_user';
*/
