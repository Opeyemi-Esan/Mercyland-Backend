namespace MercylandAdmin.Models
{
    public class PaystackTransactionRequest
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public int Amount { get; set; } // Amount in kobo, e.g., 1000 = ₦10.00
        public string CallbackUrl { get; set; }
        public string Reference { get; set; }
    }
}
