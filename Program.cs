using System;
using System.IO;
using System.Text.Json;
using Npgsql;
using System.Text.Json.Serialization;
using NpgsqlTypes;

namespace JSON_Parse
{
    class Program
    {
        static void Main(string[] args)
        {
            
            // create dotnet object
    	    matchingObject obj = new matchingObject();
            obj.Id = Int32.MaxValue;
            obj.Type = "Gold";

            /*  Serialization Process */
            
            // take dotnet obj and create json
            string jsonString = JsonSerializer.Serialize(obj);
            
            // sync
            File.WriteAllText("test.json", jsonString);
            // async
            AsyncSerialize("other.json", obj);

            // async method for serializing dotnet objects
            static async void AsyncSerialize(string path, matchingObject obj)
            {
                using (FileStream fs = File.Create(path))
                {
                    await JsonSerializer.SerializeAsync(fs, obj);
                }
            }
            
            /* Deserialization Process */
            
            // translate json to dotnet object
            string jsonOtherString = File.ReadAllText("test.json");
            matchingObject something = JsonSerializer.Deserialize<matchingObject>(jsonOtherString);
            
            // testing to see if we successfully read in the json
            Console.WriteLine(something.Id);
            Console.WriteLine(something.Type);
            
            // Example
            string exampleString = File.ReadAllText("example.json");
            Example stateObj = JsonSerializer.Deserialize<Example>(exampleString);
            
            Console.WriteLine("USERNAME: ");
            var username = Console.ReadLine();
            Console.WriteLine("PASSWORD: ");
            var password= Console.ReadLine();

            if (username == stateObj.name && password == stateObj.password )
            {
                Console.WriteLine("Welcome to Basic Auth!");
            }
            else
            {
                Console.WriteLine("Username or Password was Invalid");
            }
            
            /* Databases */

			Console.WriteLine("Enter a connection string for your database: ");
            
            // create database connection enter your own stuff lol
            const string connString = Console.ReadLine();

            // while using the database connection
            using (var conn = new NpgsqlConnection(connString))
            {
                Console.WriteLine("Opening connection");
                conn.Open();
                
				const string sqlCommand = Console.ReadLine();
                // run sql command to insert into db
                var command = new NpgsqlCommand(sqlCommand, conn);
                command.ExecuteNonQuery();
                
               // run sql command to read from db 
               var read_command = new NpgsqlCommand("select * where true", conn);
               // execute read command
               var reader = read_command.ExecuteReader();
			   // modify based on your database
               while (reader.Read())
               {
                   Console.WriteLine("reading from table ({0}, {1}, {2})"
                       , reader.GetInt32(0)
                       , reader.GetString(1).ToString()
                       , reader.GetString(2).ToString());
               }
            }
        }
    }

    class matchingObject
    {
        private int _id;
        private string _type;
        
        public int Id { get => _id; set => _id = value; }
        public string Type { get => _type; set => _type = value; }
    }
}
