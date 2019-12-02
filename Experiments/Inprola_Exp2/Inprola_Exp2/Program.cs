using InprolaDBExp1;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Inprola_Exp2
{
    class Program
    {
        private static List<string> allRequirements;
        private const string _RequirementFilename = "AllRequirements.json";
        private static InDB _inDB;

        private static void DeleteRequirements()
        {
            allRequirements = new List<string>();
        }

        private static void RestoreRequirements()
        {
            try
            {
                // Open the text file using a stream reader.
                using (StreamReader sr = new StreamReader(_RequirementFilename))
                {
                    // Read the stream to a string, and write the string to the console.
                    String line = sr.ReadToEnd();
                    allRequirements = JsonSerializer.Deserialize<List<string>>(line);
                }
            }
            catch
            {
                allRequirements = new List<string>();
            }
        }

        static List<string> SplitSentences(string requirement)
        {
            return new List<string>(requirement.TrimEnd('.').Split(". ", StringSplitOptions.RemoveEmptyEntries));
        }

        static List<string> SplitWords(string sentence)
        {
            return new List<string>(sentence.Split(' ', StringSplitOptions.RemoveEmptyEntries));
        }

        static string CreateRequirementResult(string requirement)
        {
            var result = "";

            foreach(var singleSentence in SplitSentences(requirement))
            {
                foreach(var singleWord in SplitWords(singleSentence))
                {
                    string match = _inDB.FindMatch(singleWord);
                    if (string.IsNullOrEmpty(match))
                    {
                        Console.WriteLine("Verarbeitung abgebrochen");
                        return result;
                    }

                    Console.WriteLine($"Wort:{singleWord} - {match}");
                }
            }

            return result;
        }

        static void Main(string[] args)
        {
            _inDB = new InDB(@"C:\Projects\inDB_Data_Exp2", new string[] { "Deutsch" });
            bool setupStatus = true;
            setupStatus = setupStatus && _inDB.StartEngine();
            setupStatus = setupStatus && _inDB.RegisterFullDBNode(@"C:\Projects\INPROLA-Web-React-Express\Exp1\InDB4React");
            setupStatus = setupStatus && _inDB.RegisterFullDBNode(@"C:\Projects\INPROLA-Web-React-Express\Exp1\InDB4Express");

            if (!setupStatus)
            {
                Console.WriteLine("Failed to initialize base InDB Parts");
                Console.ReadLine();
                Environment.Exit(0);
            }
            
            RestoreRequirements();

            while (true)
            {
                Console.WriteLine("Nächster Befehl (i für Hinweise)");
                switch(Console.ReadLine())
                {
                    case "i":
                        Console.WriteLine("Inprola Exp2 V0.0.1");
                        Console.WriteLine("Verfügbare Kommandos:");
                        Console.WriteLine("s: Zeige alle bisherigen Anforderungen an");
                        Console.WriteLine("n: Füge neue Anforderung hinzu");
                        Console.WriteLine("w: Speichere aktuelle Anforderungen");
                        Console.WriteLine("r: Gehe zurück zum letzten Zustand der Anforderungen");
                        Console.WriteLine("d: Lösche alle Anforderungen");
                        Console.WriteLine("c: Führe Anforderungen aus");
                        Console.WriteLine("q: Programm schließen");
                        break;

                    case "s":
                        foreach (var singleRequirement in allRequirements.Select((x, i) => new { Value = x, Index = i }))
                        {
                            Console.WriteLine($"[{singleRequirement.Index}] {singleRequirement.Value}");
                        }
                        break;

                    case "n":
                        Console.WriteLine("Nächstes Requirement:");
                        allRequirements.Add(Console.ReadLine());
                        break;

                    case "w":
                        //open file stream
                        using (StreamWriter file = File.CreateText(_RequirementFilename))
                        {
                            //serialize object directly into file stream
                            string jsonResult = JsonSerializer.Serialize(allRequirements);
                            file.Write(jsonResult);
                        }
                        break;

                    case "d":
                        DeleteRequirements();
                        break;

                    case "c":
                        try
                        {
                            Console.WriteLine("Wähle ein Requirement per Index aus:");
                            int index = int.Parse(Console.ReadLine());
                            var result = CreateRequirementResult(allRequirements[index]);
                            Console.Write(result);
                            Console.WriteLine("");
                        }
                        catch(Exception ex)
                        {
                            Console.WriteLine(ex);
                        }
                        break;

                    case "r":
                        RestoreRequirements();
                        break;

                    case "q":
                        Environment.Exit(-1);
                        break;
                }
            }
        }
    }
}
