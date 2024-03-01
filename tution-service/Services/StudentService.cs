using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using tution_service.Data;
using tution_service.Dtos;
using tution_service.Exceptions;
using tution_service.Models;

namespace tution_service.Services
{
    public class StudentService : IStudentService
    {
        private readonly TutionDbContext dbContext;

        public StudentService(TutionDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<ApiResponse> CreateStudent(StudentDTO studentDTO)
        {
            Student student = new Student();
            student.FullName = studentDTO.FullName;
            student.StudentCode = studentDTO.StudentCode;

            await dbContext.Students.AddAsync(student);
            await dbContext.SaveChangesAsync();

            return new ApiResponse(true, "Created", student);
        }

        public async Task<ApiResponse> FindAll()
        {
            List<Student> students = await dbContext.Students.ToListAsync();
            return new ApiResponse(true, "Fetch data successful", students);
        }

        public async Task<ApiResponse> FindById(int id)
        {
            Student student = await dbContext.Students
                .SingleOrDefaultAsync(s => s.Id == id) ??
                    throw new NotFoundException("Not found student with id = " + id);
            return new ApiResponse(true, "Found student", student);
        }

        public async Task<ApiResponse> FindByStudentCode(string studentCode)
        {
            Student student = await dbContext.Students
                .SingleOrDefaultAsync(s => s.StudentCode.Equals(studentCode)) ??
                    throw new NotFoundException("Not found student with code = " + studentCode);
            return new ApiResponse(true, "Found student", student);
        }
    }
}
