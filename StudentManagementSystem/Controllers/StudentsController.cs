using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StudentManagementSystem.BLL.Interfaces;
using StudentManagementSystem.BLL.Repositories;
using StudentManagementSystem.Data.Data;
using StudentManagementSystem.Lib.Models;
//using StudentManagementSystem.ViewModels;
using StudentManagementSystem.WEB.ViewModels;

namespace StudentManagementSystem.WEB.Controllers {
    // apply to all methods
    //[Authorize]
    public class StudentsController : Controller {
        private readonly IStudentRepository _studentRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public StudentsController(IStudentRepository studentRepository, IWebHostEnvironment webHostEnvironment) {
            _studentRepository = studentRepository;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<ActionResult> Index(string? firstName, string? lastName, string? gender) {
            IList<Student> students = await _studentRepository.GetAllStudents();

            if (!string.IsNullOrEmpty(firstName)) {
                students = students.Where(s => s.FirstName.ToLower().Contains(firstName.ToLower())).ToList();
            }

            if (!string.IsNullOrEmpty(lastName)) {
                students = students.Where(s => s.LastName.ToLower().Contains(lastName.ToLower())).ToList();
            }

            if (!string.IsNullOrEmpty(gender)) {
                students = students.Where(s => s.Gender.ToString().ToLower() == gender.ToLower()).ToList();
            }

            return View(students);
        }

        // GET: StudentsController/Details/5
        public async Task<ActionResult> Details(int id) {
            Student student = await _studentRepository.GetStudent(id);

            StudentViewModel stuentVM = new StudentViewModel() {
                Student = student,
                Title = "Student Details"
            };

            return View(stuentVM);
        }

        [Route("getstudent/courses/{id}")]
        public async Task<ViewResult> GetStudentCourses(int id) {
            IList<string> courseList = await _studentRepository.GetStudentCourses(id);
            return View(courseList);
        }

        // GET: StudentsController/Create
        [HttpGet]
        public ActionResult Create() {
            return View();
        }

        // POST: StudentsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CreateStudentViewModel studentVM) {
            try {
                if (!ModelState.IsValid) {
                    return View();
                }
                string uniqueFileName = string.Empty;

                if (studentVM.Photo != null) {
                    uniqueFileName = ProcessUploadedFile(studentVM);
                }

                Student student = new Student();
                student.FirstName = studentVM.FirstName;
                student.Initials = studentVM.Initials;
                student.LastName = studentVM.LastName;
                student.Gender = studentVM.Gender;
                student.ImageFile = uniqueFileName;
                student.EnrollmentDate = studentVM.EnrollmentDate;

                Student newStudent = await _studentRepository.AddStudent(student);

                return RedirectToAction("Details", "Students", new { id = newStudent.Id });
            }
            catch {
                return View();
            }
        }

        // GET: StudentsController/Edit/5
        [HttpGet]
        [Authorize]
        public async Task<ActionResult> Edit(int id) {
            Student student = await _studentRepository.GetStudent(id);

            EditStudentViewModel studentVM = new EditStudentViewModel() {
                Id = student.Id,
                FirstName = student.FirstName,
                Initials = student.Initials,
                LastName = student.LastName,
                EnrollmentDate = student.EnrollmentDate,
                Gender = student.Gender,
                ExistingPhoto = student.ImageFile
            };
            return View(studentVM);
        }

        // POST: StudentsController/Edit/5
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(EditStudentViewModel studentVM) {
            try {
                if (!ModelState.IsValid) {
                    return View();
                }

                Student student = new Student() {
                    Id = studentVM.Id,
                    FirstName = studentVM.FirstName,
                    Initials = studentVM.Initials,
                    LastName = studentVM.LastName,
                    Gender = studentVM.Gender,
                    EnrollmentDate = studentVM.EnrollmentDate
                };

                string uniqueFileName = string.Empty;

                if (studentVM.Photo != null) {
                    if (studentVM.ExistingPhoto != null) {
                        string existingPhotoPath = Path.Combine(_webHostEnvironment.WebRootPath, "images", studentVM.ExistingPhoto);
                        System.IO.File.Delete(existingPhotoPath);
                    }

                    student.ImageFile = ProcessUploadedFile(studentVM);
                }
                else {
                    student.ImageFile = studentVM.ExistingPhoto;
                }

                await _studentRepository.UpdateStudent(student);

                return RedirectToAction("Index", "Students");
            }
            catch {
                return View();
            }
        }

        // GET: StudentsController/Delete/5
        [HttpGet]
        [Authorize]
        public async Task<ActionResult> Delete(int id) {
            try {
                Student student = await _studentRepository.GetStudent(id);
                if (student == null) {
                    return NotFound();
                }

                if (student.ImageFile != null) {
                    string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", student.ImageFile);
                    if (System.IO.File.Exists(filePath)) {
                        System.IO.File.Delete(filePath);
                    }
                }

                await _studentRepository.DeleteStudent(id);
                return RedirectToAction("Index", "Students");
            }
            catch {
                return View("Error");
            }
        }

        private string ProcessUploadedFile(CreateStudentViewModel studentVM) {
            string uniqueFileName;
            string imageFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
            uniqueFileName = $"{Guid.NewGuid().ToString()}_{studentVM.Photo.FileName}";
            string filePath = Path.Combine(imageFolder, uniqueFileName);
            studentVM.Photo.CopyTo(new FileStream(filePath, FileMode.Create));
            return uniqueFileName;
        }
    }
}
