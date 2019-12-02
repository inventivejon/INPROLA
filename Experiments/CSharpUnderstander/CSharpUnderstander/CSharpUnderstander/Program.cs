using System;
using System.IO;
using System.Globalization;
using System.CodeDom.Compiler;
using System.Text;
using Microsoft.CSharp;

namespace CSharpUnderstander
{
    class Program
    {
        static void Main(string[] args)
        {
            var codeProvider = new CSharpCodeProvider();

            string filename = @"C:/Projects/INPROLA/Experiments/CSharpUnderstander/CodeExamples/CodeExample.cs";
            
            using (TextReader reader = File.OpenText(filename))
            {
                var parseResult = codeProvider.Parse(reader);

                Console.Write(parseResult.Namespaces);
            }
        }
    }
}
