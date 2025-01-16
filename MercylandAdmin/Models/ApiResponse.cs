using System.Net;

namespace MercylandAdmin.Models
{
    public class ApiResponse<T>
    {
        public T Data { get; set; }
        public T Token { get; set; }
        public string Message { get; set;}
        public HttpStatusCode StatusCode { get; set;}
        public bool Success { get; set;}

        //public Response(Property data, string message, bool success)
        //{
        //    Data = data;
        //    Message = message;
        //    Success = success;
        //}

        //public Response(Property data) : this(data, "Completed Successfully", true) { }
        //public Response(string message) : this(default, message, false) { }
    }
}
