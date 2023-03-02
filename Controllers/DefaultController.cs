using App2.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace App2.Controllers
{
    public class DefaultController : ApiController
    {
        [Route("validation")]
        [HttpPost]

        public async Task<string> Validate(Msisdn num)
        {
            try
            {
                Token credentials = new Token()
                {
                    client_id = ConfigurationManager.AppSettings["client_id"],
                    client_secret = ConfigurationManager.AppSettings["client_secret"],
                    grant_type = "client_credentials"
                };

                HttpClient client = new HttpClient();

                string validationUrl = "https://openapiuat.airtel.africa/auth/oauth2/token";
                dynamic serializeToken = JsonConvert.SerializeObject(credentials);

                StringContent stringContent = new StringContent(serializeToken, Encoding.UTF8, "application/json");

                var validationResponse = client.PostAsync(validationUrl, stringContent).Result;
                var data = await validationResponse.Content.ReadAsStringAsync();
                dynamic result = JsonConvert.DeserializeObject(data);

                string token = result["access_token"];
                string token_type = result["token_type"];

                if (token_type == "bearer")
                {
                    string kycUrl = $"https://openapiuat.airtel.africa/standard/v1/users/{num.msisdn}";

                    client.DefaultRequestHeaders.Add("X-Country", "UG");
                    client.DefaultRequestHeaders.Add("X-Currency", "UGX");
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    var kycResponse = client.GetAsync(kycUrl).Result;

                    if (kycResponse.IsSuccessStatusCode)
                    {
                        var data1 = await kycResponse.Content.ReadAsStringAsync();
                        dynamic result1 = JsonConvert.DeserializeObject(data1);
                        return "Phone Number successfully validated";
                    }
                    return "Unable to validate phone number";
                }
                return "Unable to get access token";

            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return msg;
            }
        }

        [Route("payment")]
        [HttpPost]

        public async Task<string> Payment(Payment pay)
        {
            try
            {
                var json = 
                    @"{
                       ""payee"": {
                            ""msisdn"":""" + pay.msisdn + @"""
                      },
                    ""reference"": """ + pay.reference + @""",
                         ""pin"":""" + pay.pin + @""",
                         ""transaction"" : {
                    ""amount"":""" + pay.amount + @""",
                    ""id"":""" + pay.id + @"""
                            }
                    }";

                string paymentUrl = "https://openapiuat.airtel.africa/standard/v1/disbursements/";

                HttpClient client = new HttpClient();

                StringContent stringContent = new StringContent(json, Encoding.UTF8, "application/json");

                client.DefaultRequestHeaders.Add("X-Country", "UG");
                client.DefaultRequestHeaders.Add("X-Currency", "UGX");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "HSqaQ93GB13PqFGOdCcjxVLKwK9t50dD");

                var payResponse = client.PostAsync(paymentUrl, stringContent).Result;
                var data = await payResponse.Content.ReadAsStringAsync();

                dynamic result = JsonConvert.DeserializeObject(data);

                bool success = result["success"];
                string message = result["status"]["message"];

                if (success)
                {
                    return message;
                }
                else
                {
                    return "Unable to make payment";
                }
            } catch(Exception ex)
            {
                string msg = ex.Message;
                return msg;
            }
        }
    }
}
