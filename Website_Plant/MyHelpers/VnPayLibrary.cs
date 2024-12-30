using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace Website_Plant.MyHelpers
{
    public class VnPayLibrary
    {
        private SortedList<string, string> requestData = new SortedList<string, string>();

        public void AddRequestData(string key, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                requestData.Add(key, value);
            }
        }

        public string CreateRequestUrl(string baseUrl, string vnp_HashSecret)
        {
            StringBuilder data = new StringBuilder();
            foreach (KeyValuePair<string, string> kv in requestData)
            {
                data.Append(WebUtility.UrlEncode(kv.Key) + "=" + WebUtility.UrlEncode(kv.Value) + "&");
            }

            string queryString = data.ToString().TrimEnd('&');
            string signData = queryString + vnp_HashSecret;

            using (var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(vnp_HashSecret)))
            {
                string hashValue = BitConverter.ToString(hmac.ComputeHash(Encoding.UTF8.GetBytes(signData))).Replace("-", "");
                queryString += "&vnp_SecureHash=" + hashValue;
            }

            return baseUrl + "?" + queryString;
        }
    }
}
