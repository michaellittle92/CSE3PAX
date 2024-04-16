/*
    - SubjectCode: Represents the code of the subject.
    - SubjectName: Represents the name of the subject.
    - SubjectClassification: Represents the classification/category of the subject.
    - YearLevel: Represents the year level associated with the subject.
*/

namespace CSE3PAX.HelpClasses
{
    public class ListSubjects
    {
        public string SubjectCode { get; set; }
        public string SubjectName { get; set; }
        public string SubjectClassification { get; set; }
        public string YearLevel { get; set; }
    }
}