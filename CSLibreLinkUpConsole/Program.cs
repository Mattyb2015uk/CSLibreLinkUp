using CSLibreLinkUp;
using System;
using System.Threading.Tasks;

namespace CSLibreLinkUpConsole
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var lluClient = new LluClient("<...LibreLinkUp_Email...>", "<...LibreLinkUp_Password...>");
            string output;

            try
            {
                int status = await lluClient.ConnectAsync();

                switch (status)
                {
                    case 0:
                        LluData glucoseData = await lluClient.ReadBasicAsync();

                        output = string.Format("{0}mmol/L", glucoseData.Value.ToString());
                        break;
                    case 2:
                        output = "Invalid Credentials";
                        break;
                    case 4:
                        output = "Please authenticate in Android Application";
                        break;
                    default:
                        output = string.Format("Unknown Error (status code: {0})", status);
                        break;
                }
            }
            catch (Exception ex)
            {
                output = ex.Message;
            }

            Console.WriteLine(output);
            Console.ReadLine();
        }            
    }
}