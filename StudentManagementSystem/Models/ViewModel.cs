using System.Collections.Generic;

namespace StudentManagementSystem.WEB.Models {
    public class DashboardViewModel {
        public int TotalStudents { get; set; }
        public int NumberCourses { get; set; }
        public int AbsentStudents { get; set; }
        public List<string> ToDoList { get; set; }
        public List<string> GradeDistributionLabels { get; set; }
        public List<int> GradeDistributionData { get; set; }
        public List<string> CourseNames { get; set; }
        public List<int> StudentsInCourses { get; set; }
    }
}
