using tution_service.Dtos;
using tution_service.Models;

namespace tution_service.Services
{
    public interface IStudentService
    {
        public Task<ApiResponse> CreateStudent(StudentDTO studentDTO);
        public Task<ApiResponse> FindById(int id);
        public Task<ApiResponse> FindAll();
        public Task<ApiResponse> FindByStudentCode(string studentCode);

    }
}
