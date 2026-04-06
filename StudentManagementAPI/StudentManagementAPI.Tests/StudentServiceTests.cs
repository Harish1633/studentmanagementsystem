using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using StudentManagementAPI.DTOs;
using StudentManagementAPI.Models;
using StudentManagementAPI.Repositories;
using StudentManagementAPI.Services;
using Xunit;

namespace StudentManagementAPI.Tests
{
    public class StudentServiceTests
    {
        private readonly Mock<IStudentRepository> _repoMock;
        private readonly Mock<ILogger<StudentService>> _loggerMock;
        private readonly StudentService _service;

        public StudentServiceTests()
        {
            _repoMock   = new Mock<IStudentRepository>();
            _loggerMock = new Mock<ILogger<StudentService>>();
            _service    = new StudentService(_repoMock.Object, _loggerMock.Object);
        }

        // ── GetAllStudentsAsync ──────────────────────────────────────────────

        [Fact]
        public async Task GetAllStudentsAsync_ReturnsAllStudents()
        {
            // Arrange
            var students = new List<Student>
            {
                new() { Id = 1, Name = "Alice", Email = "alice@example.com", Age = 21, Course = "CS",   CreatedDate = DateTime.UtcNow },
                new() { Id = 2, Name = "Bob",   Email = "bob@example.com",   Age = 23, Course = "Math", CreatedDate = DateTime.UtcNow }
            };
            _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(students);

            // Act
            var result = await _service.GetAllStudentsAsync();

            // Assert
            result.Should().HaveCount(2);
            result.First().Name.Should().Be("Alice");
        }

        [Fact]
        public async Task GetAllStudentsAsync_ReturnsEmptyList_WhenNoStudents()
        {
            _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Student>());

            var result = await _service.GetAllStudentsAsync();

            result.Should().BeEmpty();
        }

        // ── GetStudentByIdAsync ──────────────────────────────────────────────

        [Fact]
        public async Task GetStudentByIdAsync_ReturnsStudent_WhenExists()
        {
            var student = new Student { Id = 1, Name = "Alice", Email = "alice@example.com", Age = 21, Course = "CS", CreatedDate = DateTime.UtcNow };
            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(student);

            var result = await _service.GetStudentByIdAsync(1);

            result.Should().NotBeNull();
            result!.Id.Should().Be(1);
            result.Name.Should().Be("Alice");
        }

        [Fact]
        public async Task GetStudentByIdAsync_ReturnsNull_WhenNotFound()
        {
            _repoMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Student?)null);

            var result = await _service.GetStudentByIdAsync(99);

            result.Should().BeNull();
        }

        // ── CreateStudentAsync ───────────────────────────────────────────────

        [Fact]
        public async Task CreateStudentAsync_CreatesAndReturnsStudent()
        {
            var dto = new CreateStudentDto { Name = "Carol", Email = "carol@example.com", Age = 22, Course = "Physics" };
            var created = new Student { Id = 3, Name = "Carol", Email = "carol@example.com", Age = 22, Course = "Physics", CreatedDate = DateTime.UtcNow };

            _repoMock.Setup(r => r.GetByEmailAsync(dto.Email)).ReturnsAsync((Student?)null);
            _repoMock.Setup(r => r.CreateAsync(It.IsAny<Student>())).ReturnsAsync(created);

            var result = await _service.CreateStudentAsync(dto);

            result.Should().NotBeNull();
            result.Id.Should().Be(3);
            result.Name.Should().Be("Carol");
        }

        [Fact]
        public async Task CreateStudentAsync_ThrowsException_WhenEmailAlreadyExists()
        {
            var dto = new CreateStudentDto { Name = "Dup", Email = "alice@example.com", Age = 20, Course = "CS" };
            var existing = new Student { Id = 1, Email = "alice@example.com" };

            _repoMock.Setup(r => r.GetByEmailAsync(dto.Email)).ReturnsAsync(existing);

            Func<Task> act = async () => await _service.CreateStudentAsync(dto);

            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("*already exists*");
        }

        // ── UpdateStudentAsync ───────────────────────────────────────────────

        [Fact]
        public async Task UpdateStudentAsync_UpdatesAndReturnsStudent_WhenExists()
        {
            var existing = new Student { Id = 1, Name = "Alice", Email = "alice@example.com", Age = 21, Course = "CS", CreatedDate = DateTime.UtcNow };
            var dto = new UpdateStudentDto { Name = "Alice Updated", Email = "alice@example.com", Age = 22, Course = "CS" };
            var updated = new Student { Id = 1, Name = "Alice Updated", Email = "alice@example.com", Age = 22, Course = "CS", CreatedDate = existing.CreatedDate };

            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);
            _repoMock.Setup(r => r.GetByEmailAsync(dto.Email)).ReturnsAsync(existing);
            _repoMock.Setup(r => r.UpdateAsync(It.IsAny<Student>())).ReturnsAsync(updated);

            var result = await _service.UpdateStudentAsync(1, dto);

            result.Should().NotBeNull();
            result!.Name.Should().Be("Alice Updated");
            result.Age.Should().Be(22);
        }

        [Fact]
        public async Task UpdateStudentAsync_ReturnsNull_WhenStudentNotFound()
        {
            _repoMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Student?)null);

            var result = await _service.UpdateStudentAsync(99, new UpdateStudentDto
            {
                Name = "X", Email = "x@x.com", Age = 20, Course = "X"
            });

            result.Should().BeNull();
        }

        [Fact]
        public async Task UpdateStudentAsync_ThrowsException_WhenEmailTakenByOtherStudent()
        {
            var existing  = new Student { Id = 1, Email = "alice@example.com" };
            var emailOwner = new Student { Id = 2, Email = "bob@example.com" };
            var dto = new UpdateStudentDto { Name = "Alice", Email = "bob@example.com", Age = 21, Course = "CS" };

            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);
            _repoMock.Setup(r => r.GetByEmailAsync(dto.Email)).ReturnsAsync(emailOwner);

            Func<Task> act = async () => await _service.UpdateStudentAsync(1, dto);

            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("*already used*");
        }

        // ── DeleteStudentAsync ───────────────────────────────────────────────

        [Fact]
        public async Task DeleteStudentAsync_ReturnsTrue_WhenDeleted()
        {
            _repoMock.Setup(r => r.DeleteAsync(1)).ReturnsAsync(true);

            var result = await _service.DeleteStudentAsync(1);

            result.Should().BeTrue();
        }

        [Fact]
        public async Task DeleteStudentAsync_ReturnsFalse_WhenNotFound()
        {
            _repoMock.Setup(r => r.DeleteAsync(99)).ReturnsAsync(false);

            var result = await _service.DeleteStudentAsync(99);

            result.Should().BeFalse();
        }
    }
}
