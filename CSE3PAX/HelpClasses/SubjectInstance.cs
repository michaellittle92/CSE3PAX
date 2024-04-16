/*
    - InstanceID: Unique identifier for each subject instance.
    - InstanceName: Name or code of the subject instance.
    - SubjectName: Name of the subject associated with the instance.
    - StartDate: Start date of the subject instance.
    - EndDate: End date of the subject instance.
    - LecturerID: Unique identifier of the lecturer teaching the subject instance.
    - UserID: Unique identifier of the user associated with the lecturer.
    - FullStartDate: Full date representation of the start date (optional).
    - FullEndDate: Full date representation of the end date (optional).
*/

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