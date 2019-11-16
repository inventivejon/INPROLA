using System;
using System.Collections.Generic;
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

        static void Main(string[] args)
        {
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
