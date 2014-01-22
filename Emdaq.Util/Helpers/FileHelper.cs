using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ionic.Zip;

namespace Emdaq.Util.Helpers
{
    public static class FileHelper
    {
        /// <summary>
        /// Unzips the first file that matches namePredicate to the baseDirectory.
        /// Uses Ionic.Zip because as of June 2013, System.IO.Compression.ZipFile fails to unzip 5gb file.
        /// </summary>
        public static string ExtractFirstOrDefault(string zipFile, string baseDirectory, Func<string, bool> namePredicate)
        {
            using (var zip1 = ZipFile.Read(zipFile))
            {
                var entryToExtract = zip1.FirstOrDefault(x => namePredicate(x.FileName));

                if (entryToExtract == null)
                {
                    return null;
                }

                entryToExtract.Extract(baseDirectory, ExtractExistingFileAction.OverwriteSilently);

                return Path.Combine(baseDirectory, entryToExtract.FileName);
            }
        }

        /// <summary>
        /// Use this to read massive files.
        /// Enumerates lines one at a time efficiently.
        /// </summary>
        public static IEnumerable<string> ReadLines(string path)
        {
            using (var fs = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var bs = new BufferedStream(fs))
            using (var sr = new StreamReader(bs))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    yield return line;
                }
            }
        }

        public static void WriteLines(string path, IEnumerable<string> lines)
        {
            using (var fs = File.OpenWrite(path))
            using (var bs = new BufferedStream(fs))
            using (var sw = new StreamWriter(bs))
            {
                foreach (var line in lines)
                {
                    sw.WriteLine(line);
                }
            }
        }
    }
}