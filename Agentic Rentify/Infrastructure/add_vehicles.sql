-- 🚗 FIX VEHICLE DATA FOR CAIRO
-- Ensure cars in Cairo are Available and have correct status

-- Fix status inconsistencies (normalize to 'Available')
UPDATE Cars SET Status = 'Available' WHERE City = 'Cairo' AND Status IN ('available', 'pending');

-- Ensure AvailableNow is > 0 for Cairo cars marked Available
UPDATE Cars
SET AvailableNow = CASE
  WHEN AvailableNow IS NULL OR AvailableNow = 0 THEN
    CASE WHEN TotalFleet IS NULL OR TotalFleet = 0 THEN 5 ELSE TotalFleet END
  ELSE AvailableNow
END
WHERE City = 'Cairo' AND Status = 'Available';

-- Verify we have mid-range cars in Cairo (Price 50-150)
SELECT 'Cars in Cairo:' AS Status;
SELECT Id, Name, City, Seats, Price, Status FROM Cars WHERE City = 'Cairo' ORDER BY Price;

SELECT 'Mid-Range Cars in Cairo (50-150):' AS Status;
SELECT Id, Name, City, Seats, Price, Status FROM Cars 
WHERE City = 'Cairo' 
  AND Price BETWEEN 50 AND 150 
  AND Status = 'Available'
ORDER BY Price;

-- Show availability counters to confirm
SELECT 'Availability Snapshot:' AS Status;
SELECT Id, Name, City, Price, Status, TotalFleet, AvailableNow FROM Cars WHERE City = 'Cairo' ORDER BY Price;
