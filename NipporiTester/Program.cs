using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Nippori;

namespace NipporiTester
{
    class Program
    {
        const string FILE = @"d:\Dokumenty\Office\Excel\Vocabulary\NCPR-12 slovíčka - pinyin (nová verze).xlsx";

        static void Main(string[] args)
        {
            TestReadingConfiguration();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        static void TestReadingConfiguration()
        {
            try
            {
                Vocabulary.ReadFile(FILE);

                foreach (string key in Vocabulary.Configuration.Keys)
                    Console.WriteLine("{0}: {1}", key, Vocabulary.Configuration[key]);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
        }


        static void TestReadingVocableTypes()
        {
            try
            {
                Vocabulary.ReadFile(FILE);

                /* pro tento test nutno udělat vocableTypes public */
                //foreach (VocableType type in Vocabulary.vocableTypes)
                //    Console.WriteLine(type.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
        }
    }
}
