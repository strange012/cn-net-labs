using LabClassLibrary.Models;
using LabWinService.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace LabConsoleClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Введите команду");
            Console.WriteLine("Доступые команды : getall, add");

            while(true)
            {
                var entered = Console.ReadLine();
                if (entered == "getall")
                {
                    WriteEmployee();
                }
                else if (entered == "add")
                {
                    Console.WriteLine("Enter name");
                    var name = Console.ReadLine();
                    Console.WriteLine("Enter address");
                    var address = Console.ReadLine();

                    Employee employee = new Employee{Name=name,Address=address};
                    CreateEmployee(employee).Wait();
                    Console.WriteLine("Employee added!");
                    WriteEmployee();
                }
                else
                {
                    Console.WriteLine("Such command not exists");
                }
            }
        }


        private static IEnumerable<Employee> GetData()
        {
            HttpClient client = new HttpClient();

            var response = client.GetAsync("http://localhost:5555/employees");

            var jsonData = response.Result.Content.ReadAsStringAsync().Result;

            return JsonConvert.DeserializeObject<IEnumerable<Employee>>(jsonData);
        }
        private static void WriteEmployee()
        {
            var data = GetData();
            foreach (Employee emp in data)
            {
                Console.WriteLine("Employee Name: {0}, Employee Address: {1}\n", emp.Name, emp.Address);
            }
        }
    private static async Task CreateEmployee(Employee employee)
        {
            HttpClient client = new HttpClient();

            var json = JsonConvert.SerializeObject(employee);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync(
                "http://localhost:5555/employees/add", content);

            response.EnsureSuccessStatusCode();
        }

        private static void SendData(Employee employee)
        {
           // var tcpClient = new TcpClientHelper<Employee>("127.0.0.1", 8889);
            //var client = new TcpClient();
            //client.Connect("127.0.0.1", 8888);
            //TcpClientHelper<Employee>.SendData(client, employee);

        }
    }
}
