using Microsoft.EntityFrameworkCore;
using StudentManagementAPI.Data;
using StudentManagementAPI.Models;

namespace StudentManagementAPI.Repositories
{
    public class StudentRepository : IStudentRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<StudentRepository> _logger;

        public StudentRepository(AppDbContext context, ILogger<StudentRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Student>> GetAllAsync()
        {
            _logger.LogInformation("Fetching all students from database.");
            return await _context.Students
                .OrderByDescending(s => s.CreatedDate)
                .ToListAsync();
        }

        public async Task<Student?> GetByIdAsync(int id)
        {
            _logger.LogInformation("Fetching student with ID {Id}.", id);
            return await _context.Students.FindAsync(id);
        }

        public async Task<Student?> GetByEmailAsync(string email)
        {
            return await _context.Students
                .FirstOrDefaultAsync(s => s.Email.ToLower() == email.ToLower());
        }

        public async Task<Student> CreateAsync(Student student)
        {
            _logger.LogInformation("Creating new student: {Name}", student.Name);
            student.CreatedDate = DateTime.UtcNow;
            _context.Students.Add(student);
            await _context.SaveChangesAsync();
            return student;
        }

        public async Task<Student> UpdateAsync(Student student)
        {
            _logger.LogInformation("Updating student with ID {Id}.", student.Id);
            _context.Students.Update(student);
            await _context.SaveChangesAsync();
            return student;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            _logger.LogInformation("Deleting student with ID {Id}.", id);
            var student = await _context.Students.FindAsync(id);
            if (student == null) return false;

            _context.Students.Remove(student);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Students.AnyAsync(s => s.Id == id);
        }
    }
}
