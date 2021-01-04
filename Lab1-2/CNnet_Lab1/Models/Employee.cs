using Newtonsoft.Json;
using System;
using System.Dynamic;

namespace CNnet_Lab1.Models
{
    [IdValidations(0)]
    //Общая информация о сотруднике
    public class Employee : DynamicObject
    {
        [JsonProperty]
        public int Id { get; private set; }
        [JsonProperty]
        public string FullName { get; private set; }
        [JsonProperty]
        public string Address { get; private set; }
        [JsonProperty]
        public string Telephone { get; private set; }
        [JsonProperty]
        public string OtherInfo { get; private set; }

        public Employee() { }

        public Employee(int id, string address, string fullname, string telephone, string otherInfo=null)
        {
            Id = id;
            Address = address;
            if (string.IsNullOrEmpty(fullname))
            {
                throw new Exception("Employee should have a Name");
            }
            FullName = fullname;
            if (string.IsNullOrEmpty(telephone))
            {
                throw new Exception("Employee should have a Telephone");
            }
            Telephone = telephone;
            OtherInfo = otherInfo ?? string.Empty;
        }

        public override string ToString()
        {
            return $"Id: {Id},  FullName: {FullName}, Address: {Address}, Telephone: {Telephone}, OtherInfo: {OtherInfo}";
        }

        public void SetName(string name)
        {
            FullName = name;
        }

    }
}
