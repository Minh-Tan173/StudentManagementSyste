using Microsoft.AspNetCore.Mvc;
using StudentManagementSystem.Data.Data;
using StudentManagementSystem.Lib.Models;
using StudentManagementSystem.WEB.Models;
using System.Diagnostics;
using System.Linq;

namespace StudentManagementSystem.WEB.Controllers {
    public class HomeController : Controller {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context) {
            _logger = logger;
            _context = context;
        }

        // Action for Teacher's Dashboard
        public IActionResult Index() {
            var gradeDistributionData = _context.Enrollments
                .GroupBy(e => e.Grade)
                .Select(g => new { Grade = g.Key, Count = g.Count() })
                .ToList();

            var courseNames = _context.Courses.Select(c => c.Title).ToList();
            var studentsInCourses = _context.Courses
                .Select(c => c.Enrollments.Count)
                .ToList();

            var dashboardData = new DashboardViewModel {
                TotalStudents = _context.Students.Count(), // Dữ liệu động
                NumberCourses = _context.Courses.Count(),
                AbsentStudents = 7, // Bạn có thể cập nhật logic để lấy dữ liệu động cho số học sinh vắng mặt
                ToDoList = _context.ToDoItems.Select(t => t.Description).ToList(),
                GradeDistributionLabels = gradeDistributionData.Select(g => g.Grade.ToString()).ToList(),
                GradeDistributionData = gradeDistributionData.Select(g => g.Count).ToList(),
                CourseNames = courseNames,
                StudentsInCourses = studentsInCourses
            };

            return View(dashboardData);
        }

        [HttpPost]
        public IActionResult AddToDoItem(string newToDoItem) {
            if (!string.IsNullOrEmpty(newToDoItem)) {
                var toDoItem = new ToDoItem { Description = newToDoItem };
                _context.ToDoItems.Add(toDoItem);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult RemoveToDoItem(string item) {
            var toDoItem = _context.ToDoItems.FirstOrDefault(t => t.Description == item);
            if (toDoItem != null) {
                _context.ToDoItems.Remove(toDoItem);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        public IActionResult Privacy() {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
