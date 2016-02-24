using System;
using System.Linq;
using Mongo.Context.Example;
using Mongo.Context.Example.NameGenerator;
using Mongo.Context.Example.Entities;
using Mongo.Context.AppHost.Extensions;

namespace Mongo.Context.AppHost
{
    public partial class Program
    {
        static string _mongourl = "mongodb://localhost:27017/DataTests";
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

        static void FindFirst()
        {
            using (var db = new Example.Context(_mongourl))
            {
                var contact = db.Contacts.FirstOrDefault();
                Console.WriteLine(contact?.ToJson() ?? $"Unable to a find");
            }
        }

        static void FindOne(string id)
        {
            using (var db = new Example.Context(_mongourl))
            {
                var contact = db.Contacts.FirstOrDefault(x => x.Id == id);
                Console.WriteLine(contact?.ToJson() ?? $"Unable to find Contact with specified Id [{id}]");
            }
        }

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
            var name = NameGenerator.Instance.GenerateFullName(Gender.Male);
            var phone = PhoneGenerator.GenerateRandomPhone();
            var email = name.Replace(' ', '.') + "@gmail.com";
            return CreateContact(name, phone, email);
        }

        static Contact CreateContact(string name, string phone, string email)
        {
            var contact = new Contact
            {
                Name = name,
                Phone = phone,
                Email = email
            };
            return contact;
        }
    }
}
