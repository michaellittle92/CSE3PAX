/*
    - UserID: Represents the unique identifier of the user.
    - Email: Represents the email address of the lecturer.
    - FirstName: Represents the first name of the lecturer.
    - LastName: Represents the last name of the lecturer.
    - IsLecturer: Indicates whether the user is a lecturer.
    - LecturerID: Represents the unique identifier of the lecturer.
    - Expertise01 to Expertise06: Represent the areas of expertise of the lecturer.
    - ConcurrentLoadCapacity: Represents the concurrent load capacity of the lecturer.
    - AdjustedRating: Represents the adjusted rating of the lecturer.
    - LoadCapacityPercentage: Represents the load capacity percentage of the lecturer.
*/

namespace CSE3PAX.HelpClasses
{
    public class LecturerInfo
    {
        public int UserID { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsLecturer { get; set; }
        public int LecturerID { get; set; }
        public string Expertise01 { get; set; }
        public string Expertise02 { get; set; }
        public string Expertise03 { get; set; }
        public string Expertise04 { get; set; }
        public string Expertise05 { get; set; }
        public string Expertise06 { get; set; }
        public double ConcurrentLoadCapacity { get; set; }
        public int AdjustedRating { get; set; }
        public int LoadCapacityPercentage { get; set; }
    }
}
