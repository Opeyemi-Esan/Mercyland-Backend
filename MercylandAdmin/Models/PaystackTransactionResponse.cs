namespace MercylandAdmin.Models
{
    public class PaystackTransactionResponse
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public PaystackData Data { get; set; }
    }

    public class PaystackData
    {
        public string AuthorizationUrl { get; set; }
        public string AccessCode { get; set; }
        public string Reference { get; set; }
    }
}
