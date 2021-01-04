using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LabWinService.Common
{
    public class TcpClientHelper<T>
    {
        private int _port;
        private string _host;
        private TcpListener _listener;
        private bool _isRunning = false;

        public event Action<T> DataReceived;

        public TcpClientHelper(string host, int port)
        {
            _host = host;
            _port = port;
            _listener = new TcpListener(IPAddress.Parse(_host), _port);
        }

        public static void SendData(TcpClient client, T model)
        {
            string jsonData = JsonConvert.SerializeObject(model);
            byte[] data = Encoding.Unicode.GetBytes(jsonData);
            client.GetStream().Write(data, 0, data.Length); //передача данных            
        }

        public void StartListening()
        {
            _listener.Start();                
            _isRunning = true;
            UnicodeEncoding byteConverter = new UnicodeEncoding();

            while (true)
            {
                try
                {
                    ProcessReceivedData(byteConverter);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
                //if (!_isRunning)
                //{
                //    break;
                //}


                //finally
                //{
                //    Thread.Sleep(1000);
                //}
            }
        }

        private void ProcessReceivedData(UnicodeEncoding byteConverter)
        {
            var client = _listener.AcceptTcpClient();
            var stream = client.GetStream();

            if (stream.DataAvailable)
            {
                byte[] data = new byte[2048];
                StringBuilder response = new StringBuilder();

                int bytes = stream.Read(data, 0, data.Length);
                response.Append(byteConverter.GetString(data, 0, bytes));

                if (DataReceived != null)
                {
                    var model = JsonConvert.DeserializeObject<T>(response.ToString());
                    DataReceived.Invoke(model);
                }
            }

            client.Close();
        }

        public void StopListening()
        {
            _isRunning = false;
            _listener.Stop();
        }
    }
}
