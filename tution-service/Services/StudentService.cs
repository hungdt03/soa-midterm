using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using tution_service.Data;
using tution_service.Dtos;
using tution_service.Enums;
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

            Student? existedStudent = await dbContext.Students
                .SingleOrDefaultAsync(s => s.StudentCode.Equals(studentDTO.StudentCode));

            if(existedStudent != null) {
                throw new ConflictException("Mã số sinh viên đã tồn tại");
            }

            Student student = new Student();
            student.FullName = studentDTO.FullName;
            student.StudentCode = studentDTO.StudentCode;

            await dbContext.Students.AddAsync(student);
            await dbContext.SaveChangesAsync();

            return new ApiResponse(true, "Tạo sinh viên thành công", student);
        }

        public async Task<ApiResponse> FindAll()
        {
            List<Student> students = await dbContext.Students.ToListAsync();
            return new ApiResponse(true, "Lấy dữ liệu sinh viên thành công", students);
        }

        public async Task<ApiResponse> FindById(int id)
        {
            Student student = await dbContext.Students
                .SingleOrDefaultAsync(s => s.Id == id) ??
                    throw new NotFoundException("Không tìm thấy sinh viên có id = " + id);
            return new ApiResponse(true, "Tìm thấy sinh viên", student);
        }

        public async Task<ApiResponse> FindByStudentCode(string studentCode)
        {
            Student student = await dbContext.Students
                .Include(s => s.Tutions)
                .SingleOrDefaultAsync(s => s.StudentCode.Equals(studentCode)) ??
                    throw new NotFoundException("Không tìm thấy sinh viên có MSSV = " + studentCode);
            return new ApiResponse(true, "Tìm thấy sinh viên", MapStudent(student));
        }

        private object MapStudent(Student student)
        {
            if (student == null) return null;
            return new { 
                Id = student.Id,
                StudentCode = student.StudentCode,
                FullName = student.FullName,
                Tutions = student.Tutions != null ? student.Tutions.Select(
                    t => new {
                        Id = t.Id,
                        Status = t.Status.Equals(TutionStatus.PAID) ? "PAID" : "UNPAID",
                        Amount = t.Amount,
                        TutionCode = t.TutionCode,
                        Description = t.Description,
                    }
                ) : null,

            };

        }
    }
}
