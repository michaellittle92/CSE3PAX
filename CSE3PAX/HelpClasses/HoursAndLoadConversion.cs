namespace CSE3PAX.HelpClasses
{
    public class HoursAndLoadConversion
    {

        public static double CalculateWorkHours(double loadCapacity)
        {
            // Calculate the hours based on the load capacity
            double hours = loadCapacity * (38.0 / 6.0);

            // Round to the nearest integer and return as int
            return (double)Math.Round(hours, MidpointRounding.AwayFromZero);
        }

                    /*
            The CalculateLoadCapacity method computes the load capacity of a lecturer based on the provided hours.
            It calculates the load capacity as a fraction of a full-time workload (38 hours per week) and rounds
            it to the nearest tenth place. It then returns the smaller value between the calculated load capacity 
            and the maximum load capacity of 6.
            */
        public static  double CalculateLoadCapacity(double hours)
        {
            double loadCapacity = (6.0 / 38.0) * hours;

            //rounds to the nearest 10th place.
            loadCapacity = Math.Round(loadCapacity, 1, MidpointRounding.AwayFromZero);

            //Returns the smaller value
            return Math.Min(loadCapacity, 6);
        }
    }
}
