using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DomainAdded
{
    class Program
    {
        static void Main(string[] args)
        {
            MakeNewDomain();
            Console.ReadLine();
        }
        static void InfoDomainApp(AppDomain defaultD)
        {
            Console.WriteLine("*** Информация о домене приложения ***\n");
            Console.WriteLine("-> Имя: {0}\n-> ID: {1}\n-> По умолчанию? {2}\n-> Путь: {3}\n",
                defaultD.FriendlyName, defaultD.Id, defaultD.IsDefaultAppDomain(), defaultD.BaseDirectory);

            Console.WriteLine("Загружаемые сборки: \n");
            // Извлекаем информацию о загружаемых сборках с помощью LINQ-запроса
            var infAsm = from asm in defaultD.GetAssemblies()
                         orderby asm.GetName().Name
                         select asm;
            foreach (var a in infAsm)
                Console.WriteLine("-> Имя: \t{0}\n-> Версия: \t{1}", a.GetName().Name, a.GetName().Version);
        }
        static void MakeNewDomain()
        {
            // Создадим новый домен приложения
            AppDomain newD = AppDomain.CreateDomain("OrganizationCheckerDomain");
            var assemblyName = AssemblyName.GetAssemblyName(@"D:\project\CNnet_Lab1\OrganizationChecker\bin\Debug\OrganizationChecker.exe");
            newD.Load(assemblyName);
            newD.ExecuteAssemblyByName(assemblyName);
            InfoDomainApp(newD);
            // Уничтожение домена приложения
          AppDomain.Unload(newD);
        }
    }
}
