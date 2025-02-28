using System.Text;

namespace SonicStore.Areas.SonicStore.Utils
{
    public class RandomInfomation
    {
        private static Random random = new Random();

        public static string MakeEmail()
        {
            string strValues = "abcdefg12345";
            StringBuilder strEmail = new StringBuilder();

            for (int i = 0; i < 10; i++)
            {
                char strTmp = strValues[random.Next(strValues.Length)];
                strEmail.Append(strTmp);
            }

            strEmail.Append("@");

            for (int j = 0; j < 8; j++)
            {
                char strTmp = strValues[random.Next(strValues.Length)];
                strEmail.Append(strTmp);
            }

            strEmail.Append(".com");

            return strEmail.ToString();
        }
        public static string GetRandomPhoneNumber(int length)
        {
            if (length <= 0 || length > 18)
            {
                throw new ArgumentOutOfRangeException("length", "Length must be between 1 and 18.");
            }

            double min = Math.Pow(10, length - 1);
            double max = min * 9;
            double number = min + random.NextDouble() * max;

            return ((long)Math.Floor(number)).ToString();
        }
        public static string GetUserName()
        {
            var possible = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var length = 7;
            var username = new StringBuilder();

            for (int i = 0; i < length; i++)
            {
                int index = random.Next(possible.Length);
                username.Append(possible[index]);
            }

            return username.ToString();
        }
    }
}
