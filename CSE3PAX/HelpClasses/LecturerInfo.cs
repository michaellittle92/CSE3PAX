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
