/*
    - SubjectInstanceID: Represents the unique identifier of the subject instance.
    - SubjectInstanceName: Represents the name of the subject instance.
    - SubjectInstanceCode: Represents the code of the subject instance.
    - SubjectName: Represents the name of the subject associated with the instance.
    - LecturerFirstName: Represents the first name of the lecturer associated with the instance.
    - LecturerLastName: Represents the last name of the lecturer associated with the instance.
    - LecturerEmail: Represents the email address of the lecturer associated with the instance.
    - StartDate: Represents the start date of the subject instance.
    - EndDate: Represents the end date of the subject instance.
*/

namespace CSE3PAX.HelpClasses
{
    public class ListSubjectInstances
    {
        public int SubjectInstanceID { get; set; }
        public string SubjectInstanceName { get; set; }
        public string SubjectInstanceCode { get; set; }
        public string SubjectName { get; set; }
        public string LecturerFirstName { get; set; }
        public string LecturerLastName { get; set; }
        public string LecturerEmail { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
