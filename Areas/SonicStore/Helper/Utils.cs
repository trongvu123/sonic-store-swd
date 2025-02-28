namespace SonicStore.Areas.SonicStore.Helper
{
    public static class Utilss
    {
        public static string GetIpAddress(HttpContext context)
        {
            string ipAddress;
            try
            {
                ipAddress = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
                if (string.IsNullOrEmpty(ipAddress) || (ipAddress.ToLower() == "unknown"))
                {
                    ipAddress = context.Connection.RemoteIpAddress?.ToString();
                }
            }
            catch (Exception ex)
            {
                ipAddress = "Invalid IP:" + ex.Message;
            }

            return ipAddress;
        }
    }
}
