using System;
using System.Linq;
using Mongo.Context.Example;
using Mongo.Context.Example.NameGenerator;
using Mongo.Context.Example.Entities;
using Mongo.Context.AppHost.Extensions;
using System.ComponentModel;

namespace Mongo.Context.AppHost
{
    public partial class Program
    {
        static string _mongourl = "mongodb://localhost:27017/DataTests";
        [Description("Adds a new Contact to the Database")]
        static void AddOne()
        {
            using (var db = new Example.Context(_mongourl))
            {
                var contact = CreateRandomContact();
                db.Contacts.Insert(contact);
                var x = db.Contacts.Count();
                Console.WriteLine("Total Contact Count {0}", x);
                Console.WriteLine(contact?.ToJson());
            }
        }
        [Description("Returns all Contacts")]
        static void All()
        {
            using (var db = new Example.Context(_mongourl))
            {
                var contacts = db.Contacts.ToList();
                foreach(var contact in contacts)
                {
                    Console.WriteLine(contact?.ToJson() ?? $"Unable to find record");
                }
            }
        }
        [Description("Returns the first Contact in the Collection")]
        static void FindFirst()
        {
            using (var db = new Example.Context(_mongourl))
            {
                var contact = db.Contacts.FirstOrDefault();
                Console.WriteLine(contact?.ToJson() ?? $"Unable to find any records");
                contact.Phone = PhoneGenerator.GenerateRandomPhone();
                db.Contacts.Save(x => x.Id == contact.Id, contact);
                FindOne(contact.Id);
            }
        }
        [Description("Returns the first Contact that matches the given Id value")]
        static void FindOne(string id)
        {
            using (var db = new Example.Context(_mongourl))
            {
                var contact = db.Contacts.FirstOrDefault(x => x.Id == id);
                Console.WriteLine(contact?.ToJson() ?? $"Unable to find Contact with specified Id [{id}]");
            }
        }
        [Description("Removes a Contact from the collection based on the given Id value")]
        static void RemoveOne(string id)
        {
            using (var db = new Example.Context(_mongourl))
            {
                var contact = db.Contacts.FirstOrDefault(x => x.Id == id);
                Console.WriteLine(contact?.ToJson() ?? $"Unable to find Contact with specified Id [{id}]");
                if (contact == null)
                {
                    return;
                }
                db.Contacts.Remove(contact);
            }
        }
        [Description("Adds 1000 Contacts to the Collection")]
        static void AddLots()
        {
            using (var db = new Example.Context(_mongourl))
            {
                var x = db.Contacts.Count();
                Console.WriteLine("Total Contact Count {0}", x);
            }
            Console.WriteLine("Adding a new Contact to the Collection");
            using (var db = new Example.Context(_mongourl))
            {
                for (int i = 0; i < 1000; i++)
                {
                    var contact = CreateRandomContact();
                    db.Contacts.Insert(contact);
                }
                var x = db.Contacts.Count();
                Console.WriteLine("Total Contact Count {0}", x);
            }
        }
        [Description("Remove All Contacts from the Collection")]
        static void RemoveAll()
        {
            using (var db = new Example.Context(_mongourl))
            {
                var x = db.Contacts.RemoveAll();
                Console.WriteLine("Total Records Affected {0}", x);
                var cnt = db.Contacts.Count();
                Console.WriteLine("Total Records Count {0}", cnt);
            }
        }

        static Contact CreateRandomContact()
        {
            var random = new Random();
            var gender = (random.Next(1,100) % 2 == 0) ? Gender.Male : Gender.Female;
            var fName = NameGenerator.Instance.GenerateFirstName(gender);
            var lName = NameGenerator.Instance.GenerateLastName();
            var phone = PhoneGenerator.GenerateRandomPhone();
            var name = $"{fName} {lName}";
            var email = $"{fName}.{lName}@gmail.com";
            return CreateContact(name, phone, email, gender);
        }

        static Contact CreateContact(string name, string phone, string email, Gender gender)
        {
            var contact = new Contact
            {
                Name = name,
                Phone = phone,
                Email = email,
                Gender = gender,
                Address = new Address()
                {
                    Street1 = "1500 Orange Avenue",
                    City = "Coronado",
                    State = "CA",
                    Postal = "92118"
                }
            };
            return contact;
        }
    }
}
