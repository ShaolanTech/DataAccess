using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ShaolanTech.Threading;
namespace ShaolanTech.Data.ManticoreSearch
{
    /// <summary>
    /// useless class
    /// </summary>
    internal class PushStreamWriter
    {
        private ByteLimitDualChannel reader;
        private string host;
        private int port;

        public PushStreamWriter(string host, int port, ByteLimitDualChannel reader)
        {
            this.reader = reader;
            this.host = host;
            this.port = port;
        }
        public async Task<ResultInfo> Write()
        {
            ResultInfo result = new ResultInfo();
            int index = 0;
            using (var httpClient = new HttpClient() { Timeout = TimeSpan.FromHours(1) })
            {
                try
                {

                    HttpRequestMessage msg = new HttpRequestMessage();
                    msg.Method = HttpMethod.Post;
                    msg.RequestUri = new Uri($"http://{this.host}:{this.port}/bulk");
                    var content = new PushStreamContent((stream, httpContent, transportContext) =>
                    {
                        //using (var writer = new StreamWriter(stream))
                        //{
                        //    string last = "";
                        //    while (this.reader.)
                        //    {
                        //        writer.WriteLine(this.reader.CurrentItem.ToString());
                        //        index++;
                        //    }
                        //    writer.WriteLine();
                        //}
                        //this.reader.SetReadComplete();
                    });
                    msg.Content = content;
                    msg.Headers.Add("connection", "keep-alive");
                    msg.Content.Headers.Add("keep-alive", "timeout=100000,max=10000000");
                    msg.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-ndjson");
                    string responseMessage = "";
                    try
                    {
                        var response = await httpClient.SendAsync(msg);
                        responseMessage = await response.Content.ReadAsStringAsync();

                        response.EnsureSuccessStatusCode();
                    }
                    catch (Exception ex)
                    {

                        result.OperationDone = false;
                        result.Message = responseMessage;
                    }
                }
                catch (Exception ex)
                {
                    result.OperationDone = false;
                    result.Message = ex.Message;
                }

            }
            return result;
        }
    }
}
