using Catalyst;
using Mosaik.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Data.SQLite;

namespace INPROLA
{
    class Program
    {
        private const string BasePathGenFile = @"..\..\..\..\GeneratedProject\TmpFolder";
        private const string GenFileName = "GenOneClass";
        private const string BasePathInputFile = @"..\..\..";
        private const string InputFileName = "Task.txt";

        struct WordPart
        {
            public string Value;
            public string PartOfSpeech;
        }

        class SingleCommandPart
        {
            public string Verb; /**< This defines the action */
            public SingleCommandPart innerCommandPart;
            public List<string> Noun; /**< This defines the object */

            public SingleCommandPart()
            {
                //Attributes = new List<string>();
                Noun = new List<string>();
            }
        }

        static SQLiteConnection CreateConnection()
        {

            SQLiteConnection sqlite_conn;
            // Create a new database connection:
            sqlite_conn = new SQLiteConnection(@"Data Source=..\..\..\..\..\Base1.sqlite; Version = 3; New = False; Compress = True; ");

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
            string Createsql = "CREATE TABLE SampleTable(Col1 VARCHAR(20), Col2 INT)";
            string Createsql1 = "CREATE TABLE SampleTable1(Col1 VARCHAR(20), Col2 INT)";
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
            sqlite_cmd.CommandText = "INSERT INTO SampleTable(Col1, Col2) VALUES('Test Text ', 1); ";
            sqlite_cmd.ExecuteNonQuery();
            sqlite_cmd.CommandText = "INSERT INTO SampleTable(Col1, Col2) VALUES('Test1 Text1 ', 2); ";
            sqlite_cmd.ExecuteNonQuery();
            sqlite_cmd.CommandText = "INSERT INTO SampleTable(Col1, Col2) VALUES('Test2 Text2 ', 3); ";
            sqlite_cmd.ExecuteNonQuery();


            sqlite_cmd.CommandText = "INSERT INTO SampleTable1(Col1, Col2) VALUES('Test3 Text3 ', 3); ";
            sqlite_cmd.ExecuteNonQuery();

        }

        static void ReadData(SQLiteConnection conn)
        {
            SQLiteDataReader sqlite_datareader;
            SQLiteCommand sqlite_cmd;
            sqlite_cmd = conn.CreateCommand();
            sqlite_cmd.CommandText = "SELECT * FROM Objects";

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
            #region READ INPUT FILE
            Console.WriteLine($"Reading the file {InputFileName}");
            var inputPath = BasePathInputFile + @"\" + InputFileName;
            if (!File.Exists(inputPath))
                return;

            var inputFileContent = File.ReadAllText(inputPath);

            Console.WriteLine("This is the file content: " + inputFileContent);
            #endregion

            #region ACCESS MEANING DATABASE
            SQLiteConnection sqlite_conn;
            sqlite_conn = CreateConnection();
            //CreateTable(sqlite_conn);
            //InsertData(sqlite_conn);
            ReadData(sqlite_conn);
            #endregion

            #region PARSE INPUT AND BUILD MEANING TREE
            Storage.Current = new OnlineRepositoryStorage(new DiskStorage("catalyst-models"));
            var nlp = Pipeline.For(Language.English);
           
            var doc = new Document(inputFileContent, Language.English);
            nlp.ProcessSingle(doc);

            var CompleteStructure = new List<object>();

            /* Go through the whole text */
            foreach(var singleSenctence in doc.TokensData)
            {
                if((singleSenctence is null) || (singleSenctence.Count == 0))
                    continue;

                var newWordList = new List<object>();
                var newCommandList = new List<object>();
                var currentCommand = new SingleCommandPart();

                /* Go through each sentence */
                foreach(var singleWord in singleSenctence)
                {
                    var content = new WordPart();
                    content.Value = inputFileContent.Substring(singleWord.LowerBound, singleWord.UpperBound-singleWord.LowerBound+1);
                    content.PartOfSpeech = singleWord.Tag.ToString();
                    newWordList.Add(content);

                    switch(singleWord.Tag)
                    {
                        case PartOfSpeech.VERB:
                            /* This is the verb of the command. AXIOM: There can always only be one VERB per command */
                            currentCommand.Verb = content.Value;
                            break;

                        case PartOfSpeech.NOUN:
                            /* This is a noun of the command. */
                            currentCommand.Noun.Add(content.Value);
                            break;

                        /* Adposition */
                        case PartOfSpeech.ADP:
                            /* This marks an object -> Find the full size of the object */
                            switch (content.Value)
                            {
                                case "of":
                                    break;

                                case "on":
                                    break;
                            }
                            break;

                        case PartOfSpeech.ADV:
                            switch (content.Value)
                            {
                                case "then":
                                    /* This means the first part of the sentence is finished. */
                                    newCommandList.Add(currentCommand);
                                    currentCommand = new SingleCommandPart();
                                    break;
                            }
                            break;
                    }
                }

                CompleteStructure.Add(newWordList);
                CompleteStructure.Add(newCommandList);
            }
            
            Console.WriteLine("Result:");
            foreach(var singleSentence in CompleteStructure)
            {
                switch(singleSentence)
                {
                    case List<object> aList:
                        foreach(var SingleWord in aList)
                        {
                            switch (SingleWord)
                            {
                                case WordPart singleCommand:
                                    Console.Write($"({singleCommand.Value}|{singleCommand.PartOfSpeech})");
                                    break;
                            }
                        }
                        break;
                }
                Console.WriteLine("");
            }
            #endregion

            #region GENERATE CODE FILE
            Console.WriteLine($"Creating file {GenFileName}.cs");

            /* Make sure the directory exists */
            Directory.CreateDirectory(BasePathGenFile);

            // Create a file to write to.
            using (var genFile = File.CreateText($@"{BasePathGenFile}\{GenFileName}.cs"))
            {
                genFile.WriteLine("using System;");
                genFile.WriteLine("using System.Collections.Generic;");
                genFile.WriteLine("using System.Text;");
                genFile.WriteLine("");
                genFile.WriteLine("namespace GeneratedProject.TmpFolder");
                genFile.WriteLine("{");
                genFile.WriteLine("    class GenOneClass");
                genFile.WriteLine("    {");
                genFile.WriteLine("    }");
                genFile.WriteLine("}");

                genFile.Close();
            }
            #endregion

            Console.WriteLine("Finished");
        }
    }
}
