using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ProjectElection.Controllers
{
    public class HomeController : Controller
    {
        private readonly string PayPalBaseUrl = "https://api.sandbox.paypal.com/";
        private readonly string ClientId = "AU_wHCaDKF9kJvQalvNaTMYV4tTbir1zvw2sjuDHnoJ3M1X6eDNyvsoqFlTOMjr9MyJFYUSsGTFrJ35M";
        private readonly string Secret = "ENt_IhfolANharyiNnFdBiSHYGOWt_Htae4OZFVISDx6p97OwB_hzpoLQYV6XZulUzRGCheq_G6btavQ";


        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Payment()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Payment(FormCollection form)
        {
            var pay = form["paymentOption"];
            if (pay == "visa")
            {
                return RedirectToAction("Create", "payments1");
            }
            else
            {
                return RedirectToAction("Index", "PaymentPaypalController");
            }

        }

        //public ActionResult PaymentPaypal()
        //{
        //    ViewBag.Message = "Your contact page.";

        //    return View();
        //}


        //public ActionResult PaymentVisa()
        //{
        //    ViewBag.Message = "Your contact page.";

        //    return View();
        //}
        public async Task<ActionResult> Checkout()
        {
            try
            {
                var accessToken = await GetAccessToken();

                var paymentPayload = new
                {
                    intent = "sale",
                    payer = new
                    {
                        payment_method = "paypal"
                    },
                    transactions = new[]
                    {
                        new
                        {
                            amount = new
                            {
                                total = "1.1",
                                currency = "USD"
                            },
                            description = "Payment description"
                        }
                    },
                    redirect_urls = new
                    {
                        return_url = "https://example.com/returnUrl",
                        cancel_url = "https://example.com/cancelUrl"
                    }
                };

                var paymentJson = Newtonsoft.Json.JsonConvert.SerializeObject(paymentPayload);

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(PayPalBaseUrl);
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    var content = new StringContent(paymentJson, Encoding.UTF8, "application/json");

                    var response = await client.PostAsync("/v1/payments/payment", content);

                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        var responseObject = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(responseContent);

                        string approvalUrl = null;
                        var linksArray = responseObject.links as Newtonsoft.Json.Linq.JArray;
                        if (linksArray != null)
                        {
                            var approvalLink = linksArray.FirstOrDefault(l => (string)l["rel"] == "approval_url");
                            if (approvalLink != null)
                            {
                                approvalUrl = approvalLink["href"].ToString();
                            }
                        }

                        if (!string.IsNullOrEmpty(approvalUrl))
                        {
                            return Redirect(approvalUrl);
                        }
                        else
                        {
                            ViewBag.ErrorMessage = "Approval URL not found in PayPal response.";
                            return View("Error");
                        }
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "Failed to initiate PayPal payment: " + response.ReasonPhrase;
                        return View("Error");
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An error occurred: " + ex.Message;
                return View("Error");
            }
        }
        private async Task<string> GetAccessToken()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(PayPalBaseUrl);
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($"{ClientId}:{Secret}")));
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                var requestData = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("grant_type", "client_credentials")
                };

                var requestContent = new FormUrlEncodedContent(requestData);
                var response = await client.PostAsync("/v1/oauth2/token", requestContent);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var responseObject = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(responseContent);
                    return responseObject.access_token;
                }
                else
                {
                    throw new Exception("Failed to retrieve PayPal access token: " + response.ReasonPhrase);
                }
            }
        }
    }

}