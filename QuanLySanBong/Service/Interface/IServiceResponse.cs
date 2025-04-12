namespace QuanLySanBong.Service.Interface
{
    public interface IServiceResponse<T>
    {
        bool IsSuccess { get; set; }
        string Message { get; set; }
        T Data { get; set; }
    }
}
