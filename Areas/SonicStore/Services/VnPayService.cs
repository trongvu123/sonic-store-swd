using Azure;
using Microsoft.AspNetCore.Mvc;
using NuGet.Common;
using SonicStore.Areas.SonicStore.Helper;
using SonicStore.Areas.SonicStore.Models;
using System.Globalization;
using System.Security.Policy;

namespace SonicStore.Areas.SonicStore.Services
{
    [Area("SonicStore")]

    public class VnPayService : IVnPayService

    {
        private readonly IConfiguration _config;

        public VnPayService(IConfiguration con)
        {
            _config = con;
        }

        public string CreatePaymentURL(HttpContext context, Helper.VnPayRequestModel model)
        {
            string hostName = System.Net.Dns.GetHostName();
            string clientIPAddress = System.Net.Dns.GetHostAddresses(hostName).GetValue(0).ToString();
            var tick = DateTime.Now.Ticks.ToString();
            var vnpay = new VnPayLibrary();
            vnpay.AddRequestData("vnp_Version", _config["VnPay:Version"]);
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", _config["VnPay:TmnCode"]);
            vnpay.AddRequestData("vnp_Amount", (model.Amount * 100).ToString());

            vnpay.AddRequestData("vnp_CreateDate", model.CreatedDate.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", _config["VnPay:CurrCode"]);
            vnpay.AddRequestData("vnp_IpAddr", clientIPAddress);
            vnpay.AddRequestData("vnp_ExpireDate", DateTime.Now.AddMinutes(15).ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_Locale", _config["VnPay:Locale"]);

            vnpay.AddRequestData("vnp_OrderInfo", $"{model.OrderId}");
            vnpay.AddRequestData("vnp_OrderType", "other"); //default value: other
            vnpay.AddRequestData("vnp_ReturnUrl", _config["VnPay:ReturnUrl"]);
            vnpay.AddRequestData("vnp_TxnRef", tick); // Mã tham chiếu của giao dịch tại hệ 

            var paymentUrl = vnpay.CreateRequestUrl(_config["VnPay:BaseUrl"], _config["VnPay:HashSecret"]);

            return paymentUrl;
        }

        //public string CreatePaymentURL(HttpContext context, VnPayRequestModel model)
        //{
        //    throw new NotImplementedException();
        //}

        public VnPaymentResponseModel PaymentExecute(IQueryCollection collection)
        {

            var vnpay = new VnPayLibrary();
            foreach (var (key, value) in collection)
            {
                if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
                {
                    vnpay.AddResponseData(key, value.ToString());
                }
            }

            var vnp_orderId = Convert.ToInt64(vnpay.GetResponseData("vnp_TxnRef"));
            var orderinfo = Convert.ToInt64(vnpay.GetResponseData("vnp_OrderInfo"));
            long vnp_Amount = Convert.ToInt64(vnpay.GetResponseData("vnp_Amount")) / 100;
            long vnpayTranId = Convert.ToInt64(vnpay.GetResponseData("vnp_TransactionNo"));
            string vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
            var vnp_SecureHash = collection.FirstOrDefault(p => p.Key == "vnp_SecureHash").Value;
            string vnp_TransactionStatus = vnpay.GetResponseData("vnp_TransactionStatus");
            bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, _config["VnPay:HashSecret"]);
            if (!checkSignature)
            {
                return new VnPaymentResponseModel
                {
                    Success = false
                };
            }

            return new VnPaymentResponseModel
            {
                Success = true,
                PaymentMethod = "VNPay",
                OrderDescription = orderinfo.ToString(),
                OrderId = vnp_orderId.ToString(),
                TransactionId = vnpayTranId.ToString(),
                Token = vnp_SecureHash,
                VnPayResponseCode = vnp_ResponseCode,
                TransactionStatus = vnp_TransactionStatus

            };
        }
        private (bool IsValid, string ErrorMessage) ValidateVnPayData(VnPayRequestModel model, string ipAddress)
        {
            var errors = new List<string>();

            // Kiểm tra Version
            if (string.IsNullOrEmpty(_config["VnPay:Version"]) || _config["VnPay:Version"].Length > 8)
                errors.Add("Invalid vnp_Version");

            // Kiểm tra Command
            if (_config["VnPay:Command"] != "pay")
                errors.Add("Invalid vnp_Command");

            // Kiểm tra TmnCode
            if (string.IsNullOrEmpty(_config["VnPay:TmnCode"]) || _config["VnPay:TmnCode"].Length != 8)
                errors.Add("Invalid vnp_TmnCode");

            // Kiểm tra Amount
            if (model.Amount <= 0 || model.Amount.ToString().Length > 12)
                errors.Add("Invalid vnp_Amount");

            // Kiểm tra CreateDate
            if (!DateTime.TryParseExact(model.CreatedDate.ToString("yyyyMMddHHmmss"),
                                        "yyyyMMddHHmmss",
                                        CultureInfo.InvariantCulture,
                                        DateTimeStyles.None,
                                        out _))
                errors.Add("Invalid vnp_CreateDate");

            // Kiểm tra CurrCode
            if (_config["VnPay:CurrCode"] != "VND")
                errors.Add("Invalid vnp_CurrCode");

            // Kiểm tra IpAddr
            if (string.IsNullOrEmpty(ipAddress) || ipAddress.Length > 45)
                errors.Add("Invalid vnp_IpAddr");

            // Kiểm tra Locale
            if (_config["VnPay:Locale"] != "vn" && _config["VnPay:Locale"] != "en")
                errors.Add("Invalid vnp_Locale");

            // Kiểm tra OrderInfo
            if (string.IsNullOrEmpty(model.Description) || model.Description.Length > 255)
                errors.Add("Invalid vnp_OrderInfo");

            // Kiểm tra OrderType
            // Kiểm tra OrderType
            if (string.IsNullOrEmpty("other") || "other".Length > 100)
                errors.Add("Invalid vnp_OrderType");

            // Kiểm tra ReturnUrl
            if (string.IsNullOrEmpty(_config["VnPay:ReturnUrl"]) ||
                _config["VnPay:ReturnUrl"].Length < 10 ||
                _config["VnPay:ReturnUrl"].Length > 255)
                errors.Add("Invalid vnp_ReturnUrl");

            // Kiểm tra TxnRef
            if (string.IsNullOrEmpty(model.OrderId.ToString()) || model.OrderId.ToString().Length > 100)
                errors.Add("Invalid vnp_TxnRef");

            if (errors.Any())
                return (false, string.Join(", ", errors));

            return (true, string.Empty);
        }
    }
}
