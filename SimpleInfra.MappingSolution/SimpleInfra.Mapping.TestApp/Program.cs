using System;
using System.Diagnostics;
using SimpleInfra.Mapping;

namespace SimpleInfra.Mapping.TestApp
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var student = new Student
            {
                Id = 10,
                IdentityNumber = "389457934598734",
                Name = "John",
                Surname = "Azalod",
                Father = "Douglas",
                Mother = "Elizabeth",
                BirthDate = new DateTime(1995, 11, 23)
            };
            Worker worker;
            Stopwatch sw = new Stopwatch();

            var count = 100;
            long tick;
            long tickSum = 0;

            for (int counter = 0; counter < count; counter++)
            {
                sw.Start();
                var w = SimpleMapper.Map<Student, Worker>(student);
                sw.Stop();
                tick = sw.ElapsedTicks;
                tickSum += tick;
                Console.WriteLine("Elapsed Time(ticks): {0}", tick);
                PrintPerson(w);
                sw.Reset();
            }

            Console.WriteLine("Total tick for Maaping: {0}", tickSum);
            Console.WriteLine("Average tick for Maaping: {0}", (double)(tickSum / count));

            Console.ReadKey();
            tickSum = 0;
            for (int counter = 0; counter < count; counter++)
            {
                worker = new Worker();
                sw.Start();
                SimpleMapper.MapTo(student, worker);
                sw.Stop();
                tick = sw.ElapsedTicks;
                tickSum += tick;
                Console.WriteLine("Elapsed Time(ticks): {0}", tick);
                PrintPerson(worker);
                sw.Reset();
            }

            Console.WriteLine("Total tick for Maaping: {0}", tickSum);
            Console.WriteLine("Average tick for Maaping: {0}", (double)(tickSum / count));

            Console.ReadKey();
        }

        private static void PrintPerson(Person person)
        {
            Console.WriteLine(person.Id);
            Console.WriteLine(person.IdentityNumber);
            Console.WriteLine(person.Name);
            Console.WriteLine(person.Surname);
            Console.WriteLine(person.Father);
            Console.WriteLine(person.Mother);
            Console.WriteLine(person.BirthDate);
        }
    }

    internal class Student : Person
    {
        public int StudentId
        { get; set; }
    }

    internal class Worker : Person
    {
        public int DepartmentId
        { get; set; }
    }

    internal class Person
    {
        public int Id
        { get; set; }

        public string IdentityNumber
        { get; set; }

        public string Name
        { get; set; }

        public string Surname
        { get; set; }

        public string Father
        { get; set; }

        public string Mother
        { get; set; }

        public DateTime BirthDate
        { get; set; }
    }
}