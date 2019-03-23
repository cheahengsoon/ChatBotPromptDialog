using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace QnABot.Components
{
    public static class QNAComponent
    {
        public static async Task<string> MakeQNARequest(string userInput)
        {
            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            var qnaAPIKey = ConfigurationManager.AppSettings["QnAAuthKey"];
            var qnaKbId = ConfigurationManager.AppSettings["QnAKnowledgebaseId"];
            var qnaHostName = ConfigurationManager.AppSettings["QnAEndpointHostName"];

            HttpContent body = new StringContent("{\"question\": \"" + userInput + "\"}");

            body.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

            client.DefaultRequestHeaders.Add("Authorization", "EndpointKey " + qnaAPIKey);

            var uri = $"https://{qnaHostName}/qnamaker/knowledgebases/{qnaKbId}/generateAnswer";

            var response = await client.PostAsync(uri, body);

            var responseContent = await response.Content.ReadAsStringAsync();

            return responseContent.ToString();
        }
    }
}