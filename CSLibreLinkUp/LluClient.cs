using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CSLibreLinkUp
{
    public sealed class LluClient
    {
        private const string LIBRE_API_VERSION = "4.11.0";
        private const string LIBRE_LOGIN_URL = "llu/auth/login";
        private const string LIBRE_CONNECTIONS_URL = "llu/connections";
        private const string LIBRE_GRAPH_URL = "llu/connections/{0}/graph";

        private string _lluEndpoint;
        private string _token = string.Empty;
        private string _accountIdHashed = string.Empty;

        public bool IsConnected { get; private set; } = false;
        public string Email { get; private set; }
        public string Password { get; private set; }

        public LluClient(string email, string password, string lluEndpoint = "https://api-eu2.libreview.io")
        {
            _lluEndpoint = lluEndpoint;

            Email = email;
            Password = password;
        }

        public async Task<int> ConnectAsync()
        {
            dynamic lluAuthentication = await ConnectToLlu(Email, Password, _lluEndpoint);

            if (lluAuthentication.status == 0)
            {
                _token = Convert.ToString(lluAuthentication.data.authTicket.token);
                _accountIdHashed = EncodeStringSha256(Convert.ToString(lluAuthentication.data.user.id));

                IsConnected = true;
            }

            return lluAuthentication.status;
        }

        public void Disconnect()
        {
            _token = string.Empty;
            _accountIdHashed = string.Empty;
            IsConnected = false;
        }

        public async Task<dynamic> ReadAsync()
        {
            if (!IsConnected)
            {
                throw new InvalidOperationException("LLU Client not connected");
            }

            dynamic connections = await GetLluRequest(
                _token,
                _accountIdHashed,
                _lluEndpoint,
                LIBRE_CONNECTIONS_URL);

            return connections;
        }

        public async Task<LluData> ReadBasicAsync()
        {
            if (!IsConnected)
            {
                throw new InvalidOperationException("LLU Client not connected");
            }

            dynamic connections = await GetLluRequest(
                _token,
                _accountIdHashed,
                _lluEndpoint,
                LIBRE_CONNECTIONS_URL);

            return new LluData(
                Convert.ToString(connections.data[0].patientId),
                Convert.ToDouble(connections.data[0].glucoseMeasurement.Value),
                Convert.ToInt32(connections.data[0].glucoseMeasurement.ValueInMgPerDl),
                Convert.ToBoolean(connections.data[0].glucoseMeasurement.isHigh),
                Convert.ToBoolean(connections.data[0].glucoseMeasurement.isLow),
                (LluData.Trend)connections.data[0].glucoseMeasurement.TrendArrow,
                Convert.ToDateTime(connections.data[0].glucoseMeasurement.Timestamp));
        }

        public async Task<dynamic> ReadGraphAsync(string patientId)
        {
            if (!IsConnected)
            {
                throw new InvalidOperationException("LLU Client not connected");
            }

            dynamic graph = await GetLluRequest(
                _token,
                _accountIdHashed,
                _lluEndpoint,
                string.Format(LIBRE_GRAPH_URL, patientId));

            return graph;
        }

        private static async Task<dynamic> ConnectToLlu(string email, string password, string endpoint)
        {
            using (var lluClient = new HttpClient())
            {
                lluClient.DefaultRequestHeaders.Add("User-Agent", "Other");
                lluClient.DefaultRequestHeaders.Add("cache-control", "Keep-Alive");
                lluClient.DefaultRequestHeaders.Add("connection", "no-cache");
                lluClient.DefaultRequestHeaders.Add("product", "llu.android");
                lluClient.DefaultRequestHeaders.Add("version", LIBRE_API_VERSION);

                using (var contents = new StringContent(
                    string.Format("{{\"email\":\"{0}\",\"password\":\"{1}\"}}", email, password),
                    Encoding.UTF8, "application/json"))
                {
                    contents.Headers.ContentEncoding.Add("gzip");

                    using (var result = await lluClient.PostAsync(string.Format("{0}/{1}", endpoint, LIBRE_LOGIN_URL), contents))
                    {
                        string content = await result.Content.ReadAsStringAsync();
                        return JObject.Parse(content);
                    }
                }
            }
        }


        private static async Task<dynamic> GetLluRequest(string token, string accountIdHash, string endpoint, string urlMap)
        {
            using (var lluDataClient = new HttpClient())
            {
                lluDataClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                lluDataClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                lluDataClient.DefaultRequestHeaders.Add("User-Agent", "Other");
                lluDataClient.DefaultRequestHeaders.Add("Account-Id", accountIdHash);
                lluDataClient.DefaultRequestHeaders.Add("product", "llu.android");
                lluDataClient.DefaultRequestHeaders.Add("version", LIBRE_API_VERSION);

                using (var lluData = await lluDataClient.GetAsync(string.Format("{0}/{1}", endpoint, urlMap)))
                {
                    string containerDataString = await lluData.Content.ReadAsStringAsync();
                    return JObject.Parse(containerDataString);
                }
            }
        }

        private static string EncodeStringSha256(string text)
        {
            using (var sha256 = new SHA256Managed())
            {
                return BitConverter.ToString(sha256.ComputeHash(Encoding.UTF8.GetBytes(text))).Replace("-", "");
            }
        }
    }
}