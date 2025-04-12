namespace QuanLySanBong.Service.Interface
{
    public class ServiceResponse<T> : IServiceResponse<T>
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }

        public ServiceResponse() { }

        public ServiceResponse(bool isSuccess, string message = null, T data = default)
        {
            IsSuccess = isSuccess;
            Message = message;
            Data = data;
        }

        public ServiceResponse(T data)
        {
            IsSuccess = true;
            Data = data;
            Message = null;
        }
    }
}
