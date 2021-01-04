using LabClassLibrary.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabClassLibrary.BusinessLogic
{
    public class EmployeeServer
    {
        public IEnumerable<Employee> InitializeCollection()
        {
            var collection = new List<Employee>
            {
                new Employee { Name = "Alex", Address = "New York, Central street 12"},
                new Employee { Name = "Raychel", Address = "Boston, Washington street 123"}
            };

            return collection;
        }

        public void SaveCollection(IEnumerable<Employee> employees)
        {
            using (var sw = File.CreateText(@"D:\project\employees.json"))
            {
                var json = JsonConvert.SerializeObject(employees);
                sw.WriteLine(json);
            }
        }

        public IEnumerable<Employee> GetStoredCollection()
        {
            var json = File.ReadAllText(@"D:\project\employees.json");
            var employees = JsonConvert.DeserializeObject<IEnumerable<Employee>>(json);
            return employees;
        }

        public void AddToCollection(Employee employee)
        {
            var existing = GetStoredCollection().ToList();
            existing.Add(employee);
            SaveCollection(existing);
        }
    }
}
