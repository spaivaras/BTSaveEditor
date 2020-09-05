using CommandLine;
using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace BTSaveEditor
{
    class Program
    {
        private static string extractedSub = "";
        private static readonly string tmpFolder = "BTSaveEditorTMP";

        public static void DecompressFileTo(string originalFileName, string decompressedFileName)
        {
            using FileStream originalFileStream = File.Open(originalFileName, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            using FileStream decompressedStream = File.Create(decompressedFileName);
            using GZipStream decompressionStream = new GZipStream(originalFileStream, CompressionMode.Decompress);

            decompressionStream.CopyTo(decompressedStream);
        }

        public static void CompressFileTo(string originalFileName, string compressedFileName)
        {
            using FileStream originalFileStream = File.Open(originalFileName, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            using FileStream compressedFileStream = File.Create(compressedFileName);
            using GZipStream compressionStream = new GZipStream(compressedFileStream, CompressionMode.Compress);

            originalFileStream.CopyTo(compressionStream);
        }

        public static bool ExtractFileFromSave(GZipStream compressionStream)
        {
            byte[] bytes = new byte[sizeof(int)];

            int read = compressionStream.Read(bytes, 0, sizeof(int));
            if (read < sizeof(int)) {
                return false;
            }

            int nameLen = BitConverter.ToInt32(bytes, 0);
            if (nameLen > 255) {
                throw new Exception("ERROR: Is this a proper save file?");
            }

            bytes = new byte[sizeof(char)];
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < nameLen; i++) {
                compressionStream.Read(bytes, 0, sizeof(char));
                char c = BitConverter.ToChar(bytes, 0);
                builder.Append(c);
            }
            string fileName = builder.ToString();

            if (fileName.Contains(".sub")) {
                extractedSub = fileName;
            }

            bytes = new byte[sizeof(int)];
            compressionStream.Read(bytes, 0, sizeof(int));
            int fileLen = BitConverter.ToInt32(bytes, 0);

            bytes = new byte[fileLen];
            compressionStream.Read(bytes, 0, bytes.Length);

            using FileStream outFile = File.Create(fileName);
            outFile.Write(bytes, 0, fileLen);

            return true;
        }

        public static void AddFileToSave(string path, GZipStream compressionStream)
        {
            char[] fileName = Path.GetFileName(path).ToCharArray();

            compressionStream.Write(BitConverter.GetBytes(fileName.Length), 0, sizeof(int));
            foreach (char chr in fileName) {
                compressionStream.Write(BitConverter.GetBytes(chr), 0, sizeof(char));
            }
            byte[] data = File.ReadAllBytes(path);

            compressionStream.Write(BitConverter.GetBytes(data.Length), 0, sizeof(int));
            compressionStream.Write(data, 0, data.Length);
        }

        public static void CompressSave(string sessionFilename, string subFilename, string outputFilename)
        {
            using FileStream outputFile = File.Open(outputFilename, System.IO.FileMode.Create, System.IO.FileAccess.Write);
            using GZipStream compressionStream = new GZipStream(outputFile, CompressionMode.Compress);

            AddFileToSave(sessionFilename, compressionStream);
            AddFileToSave(subFilename, compressionStream);
        }

        public static void DecompressSave(string saveFile)
        {
            using FileStream saveFileStream = File.Open(saveFile, FileMode.Open, FileAccess.Read);
            using GZipStream compressionStream = new GZipStream(saveFileStream, CompressionMode.Decompress, false);

            while (ExtractFileFromSave(compressionStream)) {}
        }


        static void Run(Options opts)
        {
            if (!opts.write) {
                DecompressSave(opts.saveName);
                DecompressFileTo(extractedSub, extractedSub + ".xml");

                return;
            }

            if (String.IsNullOrEmpty(opts.sessionFile)) {
                Console.WriteLine("ERROR --session not provided for write option. Refer to --help");
                Environment.Exit(1);
            }

            if (String.IsNullOrEmpty(opts.subFile) && String.IsNullOrEmpty(opts.subXmlFile)) {
                Console.WriteLine("ERROR neither --sub or --xml is provided for write option. Refer to --help");
                Environment.Exit(2);
            }

            if (!String.IsNullOrEmpty(opts.subXmlFile)) {
                string outFileName = Path.GetFileName(opts.subXmlFile.Substring(0, opts.subXmlFile.LastIndexOf(".xml")));
                string outPath = Path.Combine(tmpFolder, outFileName);

                if (!Directory.Exists(tmpFolder)) {
                    Directory.CreateDirectory(tmpFolder);
                }
                    
                CompressFileTo(opts.subXmlFile, outPath);
                opts.subFile = outPath;
            }

            CompressSave(opts.sessionFile, opts.subFile, opts.saveName);

            if (Directory.Exists(tmpFolder)) {
                Directory.Delete(tmpFolder, true);
            }
        }

        static void Main(string[] args)
        {
            //args = new string[6] { "-w", "--session", "gamesession.xml", "--xml", "my_submarine.sub.xml", "output.save"  };
            CommandLine.Parser.Default.ParseArguments<Options>(args).WithParsed(Run);
        }
    }
}
