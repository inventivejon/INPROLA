using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Text.Json;

namespace Inprola_Exp2
{
    class Program
    {
        private static List<string> allSentences;

        private static void RestoreSentences()
        {
            try
            {
                // Open the text file using a stream reader.
                using (StreamReader sr = new StreamReader("AllSentences.json"))
                {
                    // Read the stream to a string, and write the string to the console.
                    String line = sr.ReadToEnd();
                    allSentences = JsonSerializer.Deserialize<List<string>>(line);
                }
            }
            catch
            {
                allSentences = new List<string>();
            }
        }

        static SQLiteConnection CreateConnection()
        {

            SQLiteConnection sqlite_conn;
            // Create a new database connection:
            sqlite_conn = new SQLiteConnection("Data Source= database.db; Version = 3; New = True; Compress = False; ");
         // Open the connection:
         try
            {
                sqlite_conn.Open();
            }
            catch (Exception ex)
            {

            }
            return sqlite_conn;
        }

        static void CreateTable(SQLiteConnection conn)
        {

            SQLiteCommand sqlite_cmd;
            string Createsql = "CREATE TABLE SampleTable (Col1 VARCHAR(20), Col2 INT)";
           string Createsql1 = "CREATE TABLE SampleTable1 (Col1 VARCHAR(20), Col2 INT)";
           sqlite_cmd = conn.CreateCommand();
            sqlite_cmd.CommandText = Createsql;
            sqlite_cmd.ExecuteNonQuery();
            sqlite_cmd.CommandText = Createsql1;
            sqlite_cmd.ExecuteNonQuery();

        }

        static void InsertData(SQLiteConnection conn)
        {
            SQLiteCommand sqlite_cmd;
            sqlite_cmd = conn.CreateCommand();
            sqlite_cmd.CommandText = "INSERT INTO SampleTable (Col1, Col2) VALUES('Test Text ', 1); ";
           sqlite_cmd.ExecuteNonQuery();
            sqlite_cmd.CommandText = "INSERT INTO SampleTable (Col1, Col2) VALUES('Test1 Text1 ', 2); ";
           sqlite_cmd.ExecuteNonQuery();
            sqlite_cmd.CommandText = "INSERT INTO SampleTable (Col1, Col2) VALUES('Test2 Text2 ', 3); ";
           sqlite_cmd.ExecuteNonQuery();


            sqlite_cmd.CommandText = "INSERT INTO SampleTable1 (Col1, Col2) VALUES('Test3 Text3 ', 3); ";
           sqlite_cmd.ExecuteNonQuery();

        }

        static void ReadData(SQLiteConnection conn)
        {
            SQLiteDataReader sqlite_datareader;
            SQLiteCommand sqlite_cmd;
            sqlite_cmd = conn.CreateCommand();
            sqlite_cmd.CommandText = "SELECT * FROM SampleTable";

            sqlite_datareader = sqlite_cmd.ExecuteReader();
            while (sqlite_datareader.Read())
            {
                string myreader = sqlite_datareader.GetString(0);
                Console.WriteLine(myreader);
            }
            conn.Close();
        }

        static void Main(string[] args)
        {
            SQLiteConnection sqlite_conn;
            sqlite_conn = CreateConnection();
            CreateTable(sqlite_conn);
            InsertData(sqlite_conn);
            ReadData(sqlite_conn);

            RestoreSentences();

            while (true)
            {
                Console.WriteLine("Nächster Befehl");
                switch(Console.ReadLine())
                {
                    case "i":
                        Console.WriteLine("Inprola Exp2 V0.0.1");
                        Console.WriteLine("Verfügbare Kommandos:");
                        Console.WriteLine("s: Zeige alle bisherigen Sätze an");
                        Console.WriteLine("n: Füge neuen Satz hinzu");
                        Console.WriteLine("w: Speichere aktuelle Sätze");
                        Console.WriteLine("r: Gehe zurück zum letzten Zustand der Sätze");
                        Console.WriteLine("q: Programm schließen");
                        break;

                    case "s":
                        foreach(var singleSentence in allSentences)
                        {
                            Console.WriteLine(singleSentence);
                        }
                        break;

                    case "n":
                        Console.WriteLine("Nächster Satz:");
                        allSentences.Add(Console.ReadLine());
                        break;

                    case "w":
                        //open file stream
                        using (StreamWriter file = File.CreateText("AllSentences.json"))
                        {
                            //serialize object directly into file stream
                            string jsonResult = JsonSerializer.Serialize(allSentences);
                            file.Write(jsonResult);
                        }
                        break;

                    case "r":
                        RestoreSentences();
                        break;

                    case "q":
                        Environment.Exit(-1);
                        break;
                }
            }
        }
    }
}
