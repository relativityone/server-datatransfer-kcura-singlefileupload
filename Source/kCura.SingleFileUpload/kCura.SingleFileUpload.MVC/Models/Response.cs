namespace kCura.SingleFileUpload.MVC.Models
{
    public class ResponseWithElements<T> : Response
    {
        public T Data { get; set; }
    }

    public class Response
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}