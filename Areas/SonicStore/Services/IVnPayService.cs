using Microsoft.AspNetCore.Mvc;
using SonicStore.Areas.SonicStore.Helper;

namespace SonicStore.Areas.SonicStore.Services
{

    public interface IVnPayService
    {
        string CreatePaymentURL(HttpContext context, Helper.VnPayRequestModel model);
        VnPaymentResponseModel PaymentExecute(IQueryCollection collection);
    }
}
