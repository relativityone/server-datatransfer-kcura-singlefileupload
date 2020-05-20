namespace kCura.SingleFileUpload.MVC.Models
{
	public class ResponseWithElements<T> : Response
	{
		public T Data { get; set; }
	}
}