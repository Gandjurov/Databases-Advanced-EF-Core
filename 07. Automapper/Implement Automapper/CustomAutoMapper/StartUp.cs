using System;
using Newtonsoft.Json;

namespace CustomAutoMapper
{
    public class StartUp
    {
        public static void Main()
        {
            var person = new Person()
            {
                FirstName = "Pesho",
                LastName = "Peshov",
                address = new Address()
                {
                    City = "Sofia",
                    Street = "Vitosha",
                    Number = 1
                }
            };

            var mapper = new Mapper();

            var student = mapper.Map<Student>(person);

            Console.WriteLine(JsonConvert.SerializeObject(student));
        }
    }
}
