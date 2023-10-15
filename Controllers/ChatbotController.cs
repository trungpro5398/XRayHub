using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Google.Cloud.Dialogflow.V2;
using Google.Protobuf;
using System.IO;
using Google.Apis.Auth.OAuth2;
using Grpc.Auth;
using Grpc.Core;


namespace XRayHub.Controllers
{
    public class ChatbotController : Controller
    {
        // GET: Chatbot
        public async Task<ActionResult> SendMessageToDialogflow(string userInput)
        {
            // Initialize Dialogflow Configuration
            var dialogflowConfig = new
            {
                type = "service_account",
                project_id = "my-project-382008",
                private_key_id = "3b8c2f95990959058a9f0737457b781ad1aa488a",
                private_key = "-----BEGIN PRIVATE KEY-----\nMIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQDIhFqsaGsk/aLt\nOU5fJCwKg1WqLHxsf0BMNSElOlyBIBwwqT+WCPIi1QjA23q8krk2Us9TIkXBwI/1\nlYpw9jlGb1fsg8kBARC1gJUN2okdOZSTY84M+qV0sXSo+5vNNwWw62Z2S51/3fPT\np65b28w0lC+Nj6cWXKwRsoSeowsFncCoNx4r8YViizDe4isu+gjT3YN9ZQT/Eyw/\nrRPg0aIQ7XPgBQdC6udxLd6k+y5zcLWxch+RRNpclwZgKfDTPOK7ltYvsF1eQnG5\nQH68O8A7cRS9Tf/aqQw8dNEIS91qU9CfIhVkr3cSwS2ppFLlmSs6lCQd6E4d0bd6\nZ5gzorxjAgMBAAECggEACCmwE4CWtkG81UinqdRsFlYkf8rwKKc6LnjqjuckJwTh\nyctXY0aAu59/i+SRq0Wv1GuWtPe90cvJ2yxCkVxZJKou8Nn9rfhJE5J6aZRawdMX\nwiVmQ5n/4cTdnRewpEyJvClWbsPYfkhCcPyFeRYt2OyOTlu3y+MWBX4xILPozzwm\nB0YclnwbIKOBRZeFkd7wajNdwCtOy4Gzuj8DjIH/evF1ez/sbBaiMTqZVA667B8C\nIsLYy96wgUs/+BJeHiv3jpReck+51cuLUlCF0MCXovSD5jC+cAQkRXkkqcl0wqZZ\nhF3Cq/9Vwq41FaVZEM3dYxZOJ3/s+USYRuNYioVHHQKBgQD0zPSRggXBDXKqH9tR\nihCuZzTAWUxyAgJp+TbIgtDkT5+83/kzmquPSSzhG9CbplpusEiFk56xeH7QFXpB\npz4olkos0+YCkDBwT0wu6mR0VQ2EMLYcedTN+8+kJn4B0qXyCLHE8Y33Qx2iFgaw\nJY1FXy3jeIyBxxhfLvILIyp6nQKBgQDRsMKdBntk3IA5UsXqmzrgRbDgA7H7P+4B\novMC+9p6YTNnbc+Lu7MN0NVr+B2S15A11F6gaNuTUlOdRzcyH735OcV8ipgZGeFY\nfADZFQn7Rc+xMO7OD+kxM9EgCqR3D1FlfSu9pFlqbCoNev/XIxwUa7DDqcfh3gB+\n7xNexiTi/wKBgAJrpF9r8bKe92tHNVvxCYkv7A2a1G6sJmn11NzWrkoTn3i2njgs\nZ1XbW14+cCmZ03sUsyLe+sq2bkqQa80KYkr0cXdQAQ41bsUtg4tTNfsQfDm6YZKW\ngcwnmT3+Q3CxLILgIyHXYZf4Seq4XQiQ1nzBHCsLnhgKZ+tJA4uopVrhAoGAHXO7\nUfzYFgmWhPaoT9Rvx5JiToSWWhjXoCk4AsJHJItEghF9Zj3930HmyBx4YuieG8Fx\nbQxtUtrsXZnkGck1kKzZnS2GQXhOG3SHVfZLiZx7mIKr+AtqEHwbsmlsQkDHiP60\nxxUvbfA1U+Fjz97hkciycjkFlybsYOfJi7U+9xcCgYEAgylR4AFG254x94Pv0FBt\nT8UqZ4kaAb9OwLAKLwloFnERA5/Km79cf/joaWtGWjxZJon2NMCZXxwjLEtGaMYd\nZmwwtMaxQjZDRXf3TCBv2hQXZ7Hew5g2mXFwgMlOY/PgPC2Juv7bRyU06kHlriIE\na+E8eFCdUFxPjC07P9+eGKk=\n-----END PRIVATE KEY-----\n",
                client_email = "trung-525@my-project-382008.iam.gserviceaccount.com",
                client_id = "107903523725134976034",
                auth_uri = "https://accounts.google.com/o/oauth2/auth",
                token_uri = "https://oauth2.googleapis.com/token",
                auth_provider_x509_cert_url = "https://www.googleapis.com/oauth2/v1/certs",
                client_x509_cert_url = "https://www.googleapis.com/robot/v1/metadata/x509/trung-525%40my-project-382008.iam.gserviceaccount.com",
                universe_domain = "googleapis.com"
            };

            var builder = new SessionsClientBuilder
            {
                ChannelCredentials = GoogleCredential.FromJson(Newtonsoft.Json.JsonConvert.SerializeObject(dialogflowConfig)).ToChannelCredentials(),
            };
            var client = builder.Build();


            // Create a session with a unique identifier
            var sessionId = Guid.NewGuid().ToString();
            var session = SessionName.FromProjectSession(dialogflowConfig.project_id, sessionId);

            // Create a text input with the user's message
            var textInput = new TextInput
            {
                Text = userInput,
                LanguageCode = "en-US",
            };

            // Send the user's input to Dialogflow and receive a response
            var queryInput = new QueryInput
            {
                Text = textInput,
            };

            var response = client.DetectIntent(session, queryInput);
            var queryResult = response.QueryResult;

            // Extract the bot's response
            var botResponse = queryResult.FulfillmentText;

            // Return the bot's response to the client
            return Content(botResponse);
        }
    }
}
