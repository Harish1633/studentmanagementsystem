using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentManagementAPI.DTOs;
using StudentManagementAPI.Services;

namespace StudentManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [Produces("application/json")]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentService _service;
        private readonly ILogger<StudentsController> _logger;

        public StudentsController(IStudentService service, ILogger<StudentsController> logger)
        {
            _service = service;
            _logger = logger;
        }

        /// <summary>Get all students.</summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<StudentDto>>), 200)]
        public async Task<IActionResult> GetAll()
        {
            _logger.LogInformation("GET /api/students called.");
            var students = await _service.GetAllStudentsAsync();
            return Ok(ApiResponse<IEnumerable<StudentDto>>.Ok(students,
                $"Retrieved {students.Count()} student(s)."));
        }

        /// <summary>Get a student by ID.</summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<StudentDto>), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetById(int id)
        {
            _logger.LogInformation("GET /api/students/{Id} called.", id);
            var student = await _service.GetStudentByIdAsync(id);
            if (student == null)
                return NotFound(ApiResponse<StudentDto>.Fail($"Student with ID {id} not found."));

            return Ok(ApiResponse<StudentDto>.Ok(student));
        }

        /// <summary>Create a new student.</summary>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<StudentDto>), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(409)]
        public async Task<IActionResult> Create([FromBody] CreateStudentDto dto)
        {
            _logger.LogInformation("POST /api/students called for {Name}.", dto.Name);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _service.CreateStudentAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id },
                ApiResponse<StudentDto>.Ok(created, "Student created successfully."));
        }

        /// <summary>Update an existing student.</summary>
        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<StudentDto>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateStudentDto dto)
        {
            _logger.LogInformation("PUT /api/students/{Id} called.", id);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _service.UpdateStudentAsync(id, dto);
            if (updated == null)
                return NotFound(ApiResponse<StudentDto>.Fail($"Student with ID {id} not found."));

            return Ok(ApiResponse<StudentDto>.Ok(updated, "Student updated successfully."));
        }

        /// <summary>Delete a student by ID.</summary>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation("DELETE /api/students/{Id} called.", id);
            var deleted = await _service.DeleteStudentAsync(id);

            if (!deleted)
                return NotFound(ApiResponse<bool>.Fail($"Student with ID {id} not found."));

            return Ok(ApiResponse<bool>.Ok(true, "Student deleted successfully."));
        }
    }
}
