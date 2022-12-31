using ShaolanTech.Threading;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
namespace ShaolanTech.Data.ManticoreSearch
{
    /// <summary>
    /// Manticore Search Stream Data Writer
    /// </summary>
    public class ManticorePushStreamWriter
    {
        private string host;
        private int port;
       
        public string Current { get; set; }
        private const long CommitLength = 1024 * 1024 * 64;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="host">Server ip or domain name</param>
        /// <param name="port">server http port</param>
        public ManticorePushStreamWriter(string host, int port)
        {
            this.host = host;
            this.port = port;
        }
        /// <summary>
        /// Start to write streamed json to server
        /// </summary>
        /// <param name="readCallback">Call function that could keep reading data</param>
        /// <returns></returns>
        public async Task<ResultInfo> Write(Func<ManticoreBulkModel> readCallback)
        {
            ResultInfo result = new ResultInfo();
            int index = 0;
            ByteLimitDualChannel reader = new ByteLimitDualChannel(CommitLength, (obj) => Encoding.UTF8.GetByteCount(((ManticoreBulkModel)obj).ToHttpJson()), async (channelReader, finishedSignal) =>
            {
                using (var httpClient = new HttpClient() { Timeout = TimeSpan.FromHours(1) })
                {
                    try
                    {
                        HttpRequestMessage msg = new HttpRequestMessage();
                        msg.Method = HttpMethod.Post;
                        msg.RequestUri = new Uri($"http://{this.host}:{this.port}/bulk");
                        var content = new PushStreamContent((stream, httpContent, transportContext) =>
                        {
                            using (var writer = new StreamWriter(stream))
                            {
                                while (channelReader.TryRead(out object current))
                                {
                                    writer.WriteLine(((ManticoreBulkModel)current).ToHttpJson());
                                    index++;
                                }
                                writer.WriteLine();
                            }

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
                            finishedSignal.SetResult(true);
                        }
                        catch (Exception ex)
                        {

                            result.OperationDone = false;
                            result.Message = responseMessage;
                            finishedSignal.SetException(ex);
                        }

                    }
                    catch (Exception ex)
                    {
                        
                        finishedSignal.SetException(ex);
                    }

                }
            });
            try
            {
                await reader.StartRead(readCallback);
            }
            catch (Exception ex)
            {

                result.OperationDone = false;
                result.Message = ex.Message;
            }


            return result;
        }

    }
}
