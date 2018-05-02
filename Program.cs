using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace VersionReport
{
    class Program
    {
        private static SHA1 sha1 = new SHA1Managed();

        static int Main(string[] args)
        {
            if (args.Length == 0)
            {
                return End(ReturnCode.NoSourceDirectory);
            }

            var source = args[0];
            if (!Directory.Exists(source))
            {
                return End(ReturnCode.SourceDirectoryNotFound);
            }

            try
            {
                var filenames = Directory.GetFiles(source, "*", SearchOption.AllDirectories);

                using (var output = new CsvWriter("report.csv"))
                {
                    output.Write("Path", "Hash", "File Version");
                    foreach (var filename in filenames)
                    {
                        output.Write(filename, GetHash(filename), GetFileVersion(filename));
                    }
                }

                return End(ReturnCode.Success);
            }
            catch (Exception ex)
            {
                return End(ex);
            }
        }

        static string GetHash(string filename)
        {
            using (var stream = File.OpenRead(filename))
            {
                var hash = sha1.ComputeHash(stream);
                return HexString.Get(hash);
            }
        }

        static string GetFileVersion(string filename)
        {
            var extension = Path.GetExtension(filename);
            if (extension != ".dll" && extension != ".exe")
            {
                return String.Empty;
            }

            return FileVersionInfo.GetVersionInfo(filename).FileVersion;
        }

        static int End(Exception ex)
        {
            return End(ReturnCode.UnknownError, ex);
        }

        static int End(ReturnCode code, Exception ex = null)
        {
            switch(code)
            {
                case ReturnCode.NoSourceDirectory:
                    Console.WriteLine("Specify directory to report on in first parameter.");
                    break;
                case ReturnCode.SourceDirectoryNotFound:
                    Console.WriteLine("Specified directory does not exist.");
                    break;
                case ReturnCode.UnknownError:
                    Console.WriteLine(ex?.Message);
                    break;
            }

            if (System.Diagnostics.Debugger.IsAttached)
            {
                Console.WriteLine("Press any key to continue . . .");
                Console.ReadKey(true);
            }

            return (int)code;
        }
    }
}
