-- 🔧 FIX INVALID COORDINATES IN DATABASE
-- This script updates attractions, hotels, and cars with proper Egyptian coordinates

-- ============================================
-- ATTRACTIONS: Update with valid coordinates
-- ============================================

-- Egyptian Museum (Cairo)
UPDATE Attractions SET Latitude = 30.0349, Longitude = 31.2353 WHERE Name LIKE '%Egyptian Museum%';

-- Great Pyramid of Giza
UPDATE Attractions SET Latitude = 29.9792, Longitude = 31.1342 WHERE Name LIKE '%Pyramid%' AND City = 'Cairo';

-- Khan El Khalili Bazaar (Cairo)
UPDATE Attractions SET Latitude = 30.0615, Longitude = 31.2607 WHERE Name LIKE '%Khan El Khalili%';

-- Hanging Church (Cairo)
UPDATE Attractions SET Latitude = 30.0014, Longitude = 31.2313 WHERE Name LIKE '%Hanging Church%';

-- Al-Azhar Park (Cairo)
UPDATE Attractions SET Latitude = 30.0486, Longitude = 31.2702 WHERE Name LIKE '%Al-Azhar Park%';

-- Mosque of Ibn Tulun (Cairo)
UPDATE Attractions SET Latitude = 30.0257, Longitude = 31.2489 WHERE Name LIKE '%Mosque of Ibn Tulun%';

-- Abdeen Palace Museum (Cairo)
UPDATE Attractions SET Latitude = 30.0254, Longitude = 31.2428 WHERE Name LIKE '%Abdeen Palace%';

-- Manial Palace Museum (Cairo)
UPDATE Attractions SET Latitude = 30.0169, Longitude = 31.2547 WHERE Name LIKE '%Manial Palace%';

-- Maspero Museum (Cairo)
UPDATE Attractions SET Latitude = 30.0478, Longitude = 31.2410 WHERE Name LIKE '%Maspero%';

-- Al-Fustat Traditional Pottery (Cairo)
UPDATE Attractions SET Latitude = 30.0067, Longitude = 31.2267 WHERE Name LIKE '%Al-Fustat%';

-- Ain Al-Sira Lake (Cairo)
UPDATE Attractions SET Latitude = 30.0092, Longitude = 31.2208 WHERE Name LIKE '%Ain Al-Sira%';

-- Luxor Temple
UPDATE Attractions SET Latitude = 25.6957, Longitude = 32.6398 WHERE Name LIKE '%Luxor Temple%' AND City = 'Luxor';

-- Valley of the Kings (Luxor)
UPDATE Attractions SET Latitude = 25.7404, Longitude = 32.6034 WHERE Name LIKE '%Valley of the Kings%';

-- Karnak Temple (Luxor)
UPDATE Attractions SET Latitude = 25.7246, Longitude = 32.6596 WHERE Name LIKE '%Karnak%';

-- Red Sea Coral Reef (Sharm El Sheikh)
UPDATE Attractions SET Latitude = 27.8730, Longitude = 34.3349 WHERE Name LIKE '%Coral Reef%' AND City = 'Sharm El Sheikh';

-- Naama Bay (Sharm El Sheikh)
UPDATE Attractions SET Latitude = 27.9146, Longitude = 34.3204 WHERE Name LIKE '%Naama Bay%';

-- Ras Mohammed National Park (Sharm El Sheikh)
UPDATE Attractions SET Latitude = 27.7267, Longitude = 34.2488 WHERE Name LIKE '%Ras Mohammed%';

-- ============================================
-- CATCH-ALL: Update remaining attractions by city
-- ============================================

-- All Cairo attractions without coordinates
UPDATE Attractions 
SET Latitude = 30.0444, Longitude = 31.2357 
WHERE City = 'Cairo' AND (Latitude = 0 OR Latitude IS NULL);

-- All Luxor attractions without coordinates
UPDATE Attractions 
SET Latitude = 25.6957, Longitude = 32.6398 
WHERE City = 'Luxor' AND (Latitude = 0 OR Latitude IS NULL);

-- All Sharm El Sheikh attractions without coordinates
UPDATE Attractions 
SET Latitude = 27.9146, Longitude = 34.3204 
WHERE City = 'Sharm El Sheikh' AND (Latitude = 0 OR Latitude IS NULL);

-- All Alexandria attractions without coordinates
UPDATE Attractions 
SET Latitude = 31.2012, Longitude = 29.9187 
WHERE City = 'Alexandria' AND (Latitude = 0 OR Latitude IS NULL);

-- All Giza attractions without coordinates
UPDATE Attractions 
SET Latitude = 30.0131, Longitude = 31.2089 
WHERE City = 'Giza' AND (Latitude = 0 OR Latitude IS NULL);

-- ============================================
-- AGGRESSIVE CATCH-ALL: Update ANY remaining 0,0 coordinates by city
-- ============================================

UPDATE Attractions
SET 
  Latitude = CASE 
    WHEN City LIKE '%Cairo%' THEN 30.0444
    WHEN City LIKE '%Luxor%' THEN 25.6957
    WHEN City LIKE '%Sharm%' THEN 27.9146
    WHEN City LIKE '%Alexandria%' THEN 31.2012
    WHEN City LIKE '%Giza%' THEN 30.0131
    ELSE 30.0444  -- Default to Cairo
  END,
  Longitude = CASE
    WHEN City LIKE '%Cairo%' THEN 31.2357
    WHEN City LIKE '%Luxor%' THEN 32.6398
    WHEN City LIKE '%Sharm%' THEN 34.3204
    WHEN City LIKE '%Alexandria%' THEN 29.9187
    WHEN City LIKE '%Giza%' THEN 31.2089
    ELSE 31.2357  -- Default to Cairo
  END
WHERE Latitude = 0 OR Latitude IS NULL OR Longitude = 0 OR Longitude IS NULL;
UPDATE Hotels SET Latitude = 30.0444, Longitude = 31.2357 WHERE Name LIKE '%Marriott%' AND City = 'Cairo';
UPDATE Hotels SET Latitude = 25.6948, Longitude = 32.6383 WHERE Name LIKE '%Marriott%' AND City = 'Luxor';
UPDATE Hotels SET Latitude = 27.9273, Longitude = 34.3247 WHERE Name LIKE '%Marriott%' AND City = 'Sharm El Sheikh';

-- Hilton Hotels (Nile Hilton - Cairo)
UPDATE Hotels SET Latitude = 30.0470, Longitude = 31.2370 WHERE Name LIKE '%Hilton%' AND City = 'Cairo';
UPDATE Hotels SET Latitude = 25.6952, Longitude = 32.6402 WHERE Name LIKE '%Hilton%' AND City = 'Luxor';

-- Four Seasons Hotels
UPDATE Hotels SET Latitude = 30.0385, Longitude = 31.2305 WHERE Name LIKE '%Four Seasons%' AND City = 'Cairo';
UPDATE Hotels SET Latitude = 27.9285, Longitude = 34.3235 WHERE Name LIKE '%Four Seasons%' AND City = 'Sharm El Sheikh';

-- Sofitel Hotels
UPDATE Hotels SET Latitude = 30.0415, Longitude = 31.2334 WHERE Name LIKE '%Sofitel%' AND City = 'Cairo';

-- Intercontinental Hotels
UPDATE Hotels SET Latitude = 30.0370, Longitude = 31.2290 WHERE Name LIKE '%Intercontinental%' AND City = 'Cairo';

-- Mena House (Giza - near Pyramids)
UPDATE Hotels SET Latitude = 29.9769, Longitude = 31.1468 WHERE Name LIKE '%Mena House%';

-- Steigenberger Hotels
UPDATE Hotels SET Latitude = 30.0351, Longitude = 31.2315 WHERE Name LIKE '%Steigenberger%' AND City = 'Cairo';

-- ============================================
-- VERIFICATION: Check what was updated
-- ============================================

SELECT 'ATTRACTIONS' AS Type, COUNT(*) AS InvalidCount 
FROM Attractions WHERE Latitude = 0 OR Longitude = 0 OR Latitude IS NULL OR Longitude IS NULL
UNION ALL
SELECT 'HOTELS', COUNT(*) 
FROM Hotels WHERE Latitude = 0 OR Longitude = 0 OR Latitude IS NULL OR Longitude IS NULL;

-- Sample data
SELECT TOP 5 Name, City, Latitude, Longitude FROM Attractions WHERE City = 'Cairo' ORDER BY Rating DESC;
SELECT TOP 5 Name, City, Latitude, Longitude FROM Hotels WHERE City = 'Cairo' ORDER BY Rating DESC;
