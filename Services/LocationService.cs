namespace AttendanceSystem.Services
{
    public class LocationService
    {
        // Your classroom location
        private const double ClassroomLat = 14.748449;
        private const double ClassroomLon = 121.158867;
        private const double AllowedRadius = 50; // 50 meters
        private const double MinimumAccuracy = 300; // Require GPS accuracy better than 20m

        public (bool IsWithinPerimeter, double Distance, string Message)
            ValidateLocation(double userLat, double userLon, double userAccuracy)
        {
            // Check if GPS accuracy is sufficient
            if (userAccuracy > MinimumAccuracy)
            {
                return (false, 0, $"GPS accuracy too low: {userAccuracy:F1}m (required < {MinimumAccuracy}m)");
            }

            // Calculate distance
            var distance = CalculateHaversineDistance(ClassroomLat, ClassroomLon, userLat, userLon);

            if (distance <= AllowedRadius)
            {
                return (true, distance, $"Within The School Premises: {distance:F1}m from classroom");
            }
            else
            {
                return (false, distance, $"Outside The School Premises: {distance:F1}m from classroom");
            }
        }

        // Haversine implementation from above...
        private static double CalculateHaversineDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double earthRadius = 6371000;
            var dLat = ToRadians(lat2 - lat1);
            var dLon = ToRadians(lon2 - lon1);

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return earthRadius * c;
        }

        private static double ToRadians(double degrees) => degrees * Math.PI / 180;
    }
}