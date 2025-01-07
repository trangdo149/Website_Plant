using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;
using System.Text.Json.Nodes;

namespace Website_Plant.Pages
{
    [IgnoreAntiforgeryToken]
    public class CheckoutPaypalModel : PageModel
    {
        public string PaypalClientId { get; set; } = "";
        public string PaypalSecret { get; set; } = "";
        public string PaypalUrl { get; set; } = "";

        public string DeliveryAddress { get; set; } = "";
        public string TotalUsd { get; set; } = "";
        public string ProductIdentifiers { get; set; } = "";
        public CheckoutPaypalModel(IConfiguration configuration) 
        {
            PaypalClientId = configuration["PaypalSettings:ClientId"];
            PaypalSecret = configuration["PaypalSettings:Secret"];
            PaypalUrl = configuration["PaypalSettings:Url"];
        }
        public void OnGet()
        {
            DeliveryAddress = TempData["DeliveryAddress"]?.ToString() ?? "";
            TotalUsd = TempData["TotalUsd"]?.ToString() ?? "";
            ProductIdentifiers = TempData["ProductIdentifiers"]?.ToString() ?? "";
            TempData.Keep();
            if(DeliveryAddress == "" || TotalUsd == "" || ProductIdentifiers == "")
            {
                Response.Redirect("/");
                return;
            }
        }
        public JsonResult OnPostCreateOrder()
        {
            DeliveryAddress = TempData["DeliveryAddress"]?.ToString() ?? "";
            TotalUsd = TempData["TotalUsd"]?.ToString() ?? "";
            ProductIdentifiers = TempData["ProductIdentifiers"]?.ToString() ?? "";

            TempData.Keep();

            if (DeliveryAddress == "" || TotalUsd == "" || ProductIdentifiers == "")
            {
                return new JsonResult("");
            }
            JsonObject createOrderRequest = new JsonObject();
            createOrderRequest.Add("intent", "CAPTURE");

            JsonObject amount = new JsonObject();
            amount.Add("currency_code", "USD");
            amount.Add("value", TotalUsd);

            JsonObject purchaseUnit1 = new JsonObject();
            purchaseUnit1.Add("amount", amount);

            JsonArray purchaseUnits = new JsonArray();
            purchaseUnits.Add(purchaseUnit1);

            createOrderRequest.Add("purchase_units", purchaseUnits);
            string accessToken = GetPaypalAccessToken();
            string url = PaypalUrl + "/v2/checkout/orders";

            string orderId = "";
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);

                var requestMessage = new HttpRequestMessage(HttpMethod.Post, url);
                requestMessage.Content = new StringContent(createOrderRequest.ToString(), null, "application/json");

                var responseTask = client.SendAsync(requestMessage);
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsStringAsync();
                    readTask.Wait();

                    var strResponse = readTask.Result;
                    var jsonResponse = JsonNode.Parse(strResponse);
                    if (jsonResponse != null)
                    {
                        orderId = jsonResponse["id"]?.ToString() ?? "";

                        // save the order in the database
                    }
                }
            }


            var response = new
            {
                Id = orderId
            };
            return new JsonResult(response);
        }
        public JsonResult OnPostCompleteOrder([FromBody] JsonObject data)
        {
            if (data == null || data["orderID"] == null) return new JsonResult("");

            var orderID = data["orderID"]!.ToString();


            // get access token
            string accessToken = GetPaypalAccessToken();


            string url = PaypalUrl + "/v2/checkout/orders/" + orderID + "/capture";

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);

                var requestMessage = new HttpRequestMessage(HttpMethod.Post, url);
                requestMessage.Content = new StringContent("", null, "application/json");

                var responseTask = client.SendAsync(requestMessage);
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsStringAsync();
                    readTask.Wait();

                    var strResponse = readTask.Result;

                    var jsonResponse = JsonNode.Parse(strResponse);
                    if (jsonResponse != null)
                    {
                        string paypalOrderStatus = jsonResponse["status"]?.ToString() ?? "";
                        if (paypalOrderStatus == "COMPLETED")
                        {
                            // clear TempData
                            TempData.Clear();

                            // update payment status in the database => "accepted"

                            // clear cookie


                            return new JsonResult("success");
                        }
                    }
                }
            }

            return new JsonResult("");
        }


        public JsonResult OnPostCancelOrder([FromBody] JsonObject data)
        {
            if (data == null || data["orderID"] == null) return new JsonResult("");

            var orderID = data["orderID"]!.ToString();
            return new JsonResult("");
        }


        private string GetPaypalAccessToken()
        {
            string accessToken = "";

            string url = PaypalUrl + "/v1/oauth2/token";

            using (var client = new HttpClient())
            {
                string credentials64 =
                    Convert.ToBase64String(Encoding.UTF8.GetBytes(PaypalClientId + ":" + PaypalSecret));
                client.DefaultRequestHeaders.Add("Authorization", "Basic " + credentials64);
                var requestMessage = new HttpRequestMessage(HttpMethod.Post, url);
                requestMessage.Content = new StringContent("grant_type=client_credentials", null
                    , "application/x-www-form-urlencoded");

                var responseTask = client.SendAsync(requestMessage);
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsStringAsync();
                    readTask.Wait();

                    var strResponse = readTask.Result;

                    var jsonResponse = JsonNode.Parse(strResponse);
                    if (jsonResponse != null)
                    {
                        accessToken = jsonResponse["access_token"]?.ToString() ?? "";
                    }
                }
            }
            Console.WriteLine("JWT: " + accessToken);
            return accessToken;
        }
    }
}
