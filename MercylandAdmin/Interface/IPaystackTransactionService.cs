using MercylandAdmin.Models;

namespace MercylandAdmin.Interface
{
    public interface IPaystackTransactionService
    {
        Task<PaystackTransactionResponse> Initialization (PaystackTransactionRequest request);
    }
}
