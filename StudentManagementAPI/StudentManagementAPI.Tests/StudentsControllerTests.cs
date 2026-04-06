using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using StudentManagementAPI.Controllers;
using StudentManagementAPI.DTOs;
using StudentManagementAPI.Services;
using Xunit;

namespace StudentManagementAPI.Tests
{
    public class StudentsControllerTests
    {
        private readonly Mock<IStudentService> _serviceMock;
        private readonly Mock<ILogger<StudentsController>> _loggerMock;
        private readonly StudentsController _controller;

        public StudentsControllerTests()
        {
            _serviceMock = new Mock<IStudentService>();
            _loggerMock  = new Mock<ILogger<StudentsController>>();
            _controller  = new StudentsController(_serviceMock.Object, _loggerMock.Object);
        }

        // ── GetAll ───────────────────────────────────────────────────────────

        [Fact]
        public async Task GetAll_Returns200_WithStudentList()
        {
            var students = new List<StudentDto>
            {
                new() { Id = 1, Name = "Alice", Email = "alice@example.com", Age = 21, Course = "CS" }
            };
            _serviceMock.Setup(s => s.GetAllStudentsAsync()).ReturnsAsync(students);

            var result = await _controller.GetAll();

            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.StatusCode.Should().Be(200);

            var response = okResult.Value as ApiResponse<IEnumerable<StudentDto>>;
            response.Should().NotBeNull();
            response!.Success.Should().BeTrue();
            response.Data.Should().HaveCount(1);
        }

        // ── GetById ──────────────────────────────────────────────────────────

        [Fact]
        public async Task GetById_Returns200_WhenStudentFound()
        {
            var student = new StudentDto { Id = 1, Name = "Alice", Email = "alice@example.com", Age = 21, Course = "CS" };
            _serviceMock.Setup(s => s.GetStudentByIdAsync(1)).ReturnsAsync(student);

            var result = await _controller.GetById(1);

            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task GetById_Returns404_WhenStudentNotFound()
        {
            _serviceMock.Setup(s => s.GetStudentByIdAsync(99)).ReturnsAsync((StudentDto?)null);

            var result = await _controller.GetById(99);

            result.Should().BeOfType<NotFoundObjectResult>()
                .Which.StatusCode.Should().Be(404);
        }

        // ── Create ───────────────────────────────────────────────────────────

        [Fact]
        public async Task Create_Returns201_WithCreatedStudent()
        {
            var dto     = new CreateStudentDto { Name = "Carol", Email = "carol@test.com", Age = 22, Course = "Physics" };
            var created = new StudentDto { Id = 3, Name = "Carol", Email = "carol@test.com", Age = 22, Course = "Physics" };

            _serviceMock.Setup(s => s.CreateStudentAsync(dto)).ReturnsAsync(created);

            var result = await _controller.Create(dto);

            var createdResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
            createdResult.StatusCode.Should().Be(201);
        }

        // ── Update ───────────────────────────────────────────────────────────

        [Fact]
        public async Task Update_Returns200_WhenUpdateSuccessful()
        {
            var dto     = new UpdateStudentDto { Name = "Alice Updated", Email = "alice@example.com", Age = 22, Course = "CS" };
            var updated = new StudentDto { Id = 1, Name = "Alice Updated", Email = "alice@example.com", Age = 22, Course = "CS" };

            _serviceMock.Setup(s => s.UpdateStudentAsync(1, dto)).ReturnsAsync(updated);

            var result = await _controller.Update(1, dto);

            result.Should().BeOfType<OkObjectResult>()
                .Which.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task Update_Returns404_WhenStudentNotFound()
        {
            _serviceMock.Setup(s => s.UpdateStudentAsync(99, It.IsAny<UpdateStudentDto>()))
                .ReturnsAsync((StudentDto?)null);

            var result = await _controller.Update(99, new UpdateStudentDto
            {
                Name = "X", Email = "x@x.com", Age = 20, Course = "X"
            });

            result.Should().BeOfType<NotFoundObjectResult>()
                .Which.StatusCode.Should().Be(404);
        }

        // ── Delete ───────────────────────────────────────────────────────────

        [Fact]
        public async Task Delete_Returns200_WhenDeleteSuccessful()
        {
            _serviceMock.Setup(s => s.DeleteStudentAsync(1)).ReturnsAsync(true);

            var result = await _controller.Delete(1);

            result.Should().BeOfType<OkObjectResult>()
                .Which.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task Delete_Returns404_WhenStudentNotFound()
        {
            _serviceMock.Setup(s => s.DeleteStudentAsync(99)).ReturnsAsync(false);

            var result = await _controller.Delete(99);

            result.Should().BeOfType<NotFoundObjectResult>()
                .Which.StatusCode.Should().Be(404);
        }
    }
}
