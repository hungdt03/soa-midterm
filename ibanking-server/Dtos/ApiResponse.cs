namespace ibanking_server.Dtos
{
    public class ApiResponse
    {
        public ApiResponse(bool success, string msg, object data)
        {
            this.Success = success;
            this.Message = msg;
            this.Data = data;
        }

        public bool Success { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
    }
}
