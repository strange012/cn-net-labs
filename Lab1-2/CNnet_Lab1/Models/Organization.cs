using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNnet_Lab1.Models
{
    //Оганизация
    public class Organization
    {
        private List<Employee> _employees = new List<Employee>(); 
        public int OrganizationId { get;  private set; }
        public string OrganizationName { get; private set; }
        public IEnumerable<Employee> Employees { get => _employees; }

        public Organization(int organizationID, string organizationName, IEnumerable<Employee> employees=null)
        {
            OrganizationId = organizationID;
            if(string.IsNullOrEmpty(organizationName))
            {
                throw new Exception("Company should have a Name");
            }
            OrganizationName = organizationName;
            _employees = (employees == null) 
                ? new List<Employee>() 
                : employees.ToList();
        }

        public void AddEmployee(int id,string address,string fullName, string telephone, string otherInfo=null)
        {
            var existingEmployee = _employees.SingleOrDefault(x => x.Id == id);
            if(existingEmployee != null)
            {
                throw new Exception("Employee with such Id already exists");
            }
            var employee = new Employee(id, address, fullName, telephone, otherInfo);

            _employees.Add(employee);
        }
    }
}
