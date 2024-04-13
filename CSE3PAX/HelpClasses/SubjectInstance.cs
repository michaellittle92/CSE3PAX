namespace CSE3PAX.HelpClasses
{
    public class SubjectInstance
    {
        public int InstanceID { get; set; }
        public string InstanceName { get; set; }
        public string SubjectName { get; set; }
        public string StartDate { get; set; } 
        public string EndDate { get; set; }
        public int LecturerID { get; set; }
        public int UserID { get; set; }
        public string? FullStartDate { get; internal set; }
        public string? FullEndDate { get; internal set; }
    }

}

