using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace VersionReport
{
    internal class CsvWriter : IDisposable
    {
        private TextWriter output;

        public CsvWriter(string path)
        {
            output = new StreamWriter(path);
        }

        public void Write(params string[] fields)
        {
            output.WriteLine(String.Join(',', Delineate(fields)));
        }

        private IEnumerable<string> Delineate(string[] fields)
        {
            return fields.Select(f => $"\"{f}\"");
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    output.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
