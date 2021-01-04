using LabClassLibrary.Models;
using LabWinService.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LabWinService
{
    public partial class Service1 : ServiceBase
    {
        private TcpClientHelper<Employee> _tcpClient;

        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            using (var sw = File.CreateText(@"D:\project\servicelog.txt"))
            {
                sw.WriteLine("Service started");
            }

            _tcpClient = new TcpClientHelper<Employee>("127.0.0.1", 8888);
            _tcpClient.DataReceived += HandleData;

            var t = new Thread(new ThreadStart(_tcpClient.StartListening));
            t.Start();
        }

        void HandleData(Employee employee)
        {
            //SERVER read data
            //
            EventLog.WriteEntry(DateTime.Now + " Received:" + employee.Name + " " + employee.Address);

            using (var sw = File.AppendText(@"D:\project\applog.txt"))
            {
                sw.WriteLine(DateTime.Now + " Received:" + employee.Name);
            }
        }

        protected override void OnStop()
        {
            _tcpClient.StopListening();
        }
    }
}
