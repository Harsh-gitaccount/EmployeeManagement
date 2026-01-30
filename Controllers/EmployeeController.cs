using EmployeeManagement.Data;
using EmployeeManagement.Models;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly EmployeeRepository _repository;
        private readonly IWebHostEnvironment _environment;

        public EmployeeController(IConfiguration configuration, IWebHostEnvironment environment)
        {
            _repository = new EmployeeRepository(configuration);
            _environment = environment;
        }

        // GET: Employee/Index - Main page
        public IActionResult Index()
        {
            return View();
        }

        // GET: Get all employees (AJAX)
        [HttpGet]
        public JsonResult GetAllEmployees()
        {
            var employees = _repository.GetAllEmployees();
            return Json(employees);
        }

        // GET: Get employee by ID (AJAX)
        [HttpGet]
        public JsonResult GetEmployeeById(int id)
        {
            var employee = _repository.GetEmployeeById(id);
            return Json(employee);
        }

        // POST: Add Employee (AJAX)
        [HttpPost]
        public async Task<JsonResult> AddEmployee([FromForm] Employee employee, IFormFile? profileImage)
        {
            try
            {
                // Handle image upload
                if (profileImage != null && profileImage.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
                    
                    // Create uploads folder if it doesn't exist
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + profileImage.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await profileImage.CopyToAsync(fileStream);
                    }

                    employee.Picture = "/uploads/" + uniqueFileName;
                }

                int newId = _repository.AddEmployee(employee);
                
                return Json(new { success = true, message = "Employee added successfully!", id = newId });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }

        // POST: Update Employee (AJAX)
        [HttpPost]
        public async Task<JsonResult> UpdateEmployee([FromForm] Employee employee, IFormFile? profileImage)
        {
            try
            {
                // Get existing employee to preserve picture if not updated
                var existingEmployee = _repository.GetEmployeeById(employee.ID);

                // Handle image upload
                if (profileImage != null && profileImage.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
                    
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    // Delete old image if exists
                    if (!string.IsNullOrEmpty(existingEmployee.Picture))
                    {
                        string oldFilePath = Path.Combine(_environment.WebRootPath, existingEmployee.Picture.TrimStart('/'));
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                    }

                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + profileImage.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await profileImage.CopyToAsync(fileStream);
                    }

                    employee.Picture = "/uploads/" + uniqueFileName;
                }
                else
                {
                    // Keep existing picture
                    employee.Picture = existingEmployee.Picture;
                }

                _repository.UpdateEmployee(employee);
                
                return Json(new { success = true, message = "Employee updated successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }

        // POST: Delete Employee (AJAX)
[HttpPost]
public JsonResult DeleteEmployee(int id)
{
    try
    {
        // Get employee to delete image file
        var employee = _repository.GetEmployeeById(id);
        
        if (employee != null)
        {
            // Delete image file if exists
            if (!string.IsNullOrEmpty(employee.Picture))
            {
                string filePath = Path.Combine(_environment.WebRootPath, employee.Picture.TrimStart('/'));
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }
            
            // Delete from database
            _repository.DeleteEmployee(id);
            
            return Json(new { success = true, message = "Employee deleted successfully!" });
        }
        
        return Json(new { success = false, message = "Employee not found!" });
    }
    catch (Exception ex)
    {
        return Json(new { success = false, message = "Error: " + ex.Message });
    }
}

    }
}
