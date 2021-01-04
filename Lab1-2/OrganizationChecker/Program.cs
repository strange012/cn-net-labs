using System;
using CNnet_Lab1.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Microsoft.Office.Interop.Excel;

namespace OrganizationChecker
{
    public class Program
    {
        static void Main(string[] args)
        {
            var organization = new Organization(1,"Sevaastol");

            organization.AddEmployee(0,"street1","Jonny Cash", "+34966899997");
            organization.AddEmployee(2, "street2", "Maily Cert", "+3496545797");
            organization.AddEmployee(-1, "street3", "Peeter P", "+3496545644");

            foreach(var employee in organization.Employees)
            {
                Console.WriteLine(employee.ToString());
            }

            foreach (var id in organization.Employees)
            {
                bool validid = ValidationId(id);
                Console.WriteLine($"Id hass been: {validid}");
            }

            Console.WriteLine("*** Информация о домене приложения ***\n");
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var a in assemblies)
            {
                Console.WriteLine($"{a.FullName}\n");
            }

            //загружаем сбоку мтодом LoadFrom()
            Assembly asm = Assembly.LoadFrom("CNnet_Lab1.dll");

            Console.WriteLine(asm.FullName);
            // получаем все типы из сборки MyApp.dll
            Type[] types = asm.GetTypes();
            foreach (Type t in types)
            {
                Console.WriteLine($"{t.FullName}\n");
            }

            //  Получение информации о конструкторах
            var employeeType = types.Single(t => t.Name.EndsWith("Employee"));

            Console.WriteLine("Конструкторы:");
            foreach (ConstructorInfo ctor in employeeType.GetConstructors())
            {
                Console.Write(employeeType.Name + " (");
                // получаем параметры конструктора
                ParameterInfo[] parameters = ctor.GetParameters();
                for (int i = 0; i < parameters.Length; i++)
                {
                    Console.Write(parameters[i].ParameterType.Name + " " + parameters[i].Name);
                    if (i + 1 < parameters.Length) Console.Write(", ");
                }
                Console.WriteLine(")");
            }

            
            // Используем позднее связывание
            dynamic obj = Activator.CreateInstance(employeeType, new object[] { 1, "address", "John Skirt", "telephone", "otherInfo"});

            Console.WriteLine(obj.FullName);

            Console.WriteLine("Объект создан!");
            //вызов метода
            obj = "CNnet_Lab1.dll";
            obj = new Employee(2, "street2", "Maily Cert", "+3496545797");
            obj.SetName("Maily Second");
            Console.WriteLine(obj.FullName);

            // Start Excel and get Application object.  
            var excel = new Microsoft.Office.Interop.Excel.Application();
            // for making Excel visible  
            excel.Visible = false;
            excel.DisplayAlerts = false;
            // Creation a new Workbook  
            var excelworkBook = excel.Workbooks.Add(Type.Missing);
            // Workk sheet  
            var excelSheet = (Microsoft.Office.Interop.Excel.Worksheet)excelworkBook.ActiveSheet;
            excelSheet.Name = "Test work sheet";

            excelSheet.Cells[1, 1] = obj.FullName;
            excelSheet.Cells[1, 2] = obj.Address;

            excelworkBook.SaveAs(@"D:\project\Lab.xml");
            //  dynamic employee = new System.Dynamic.DynamicObject();

            Console.ReadLine();
        }
        public static bool ValidationId(Employee id)
        {
            Type t = typeof(Employee);
            object[] attrs = t.GetCustomAttributes(false);
            foreach (IdValidationsAttribute attr in attrs)
            {
                if (id.Id >= attr.Id) return true;
                else return false;
            }
            return true;
        }



    }
}
