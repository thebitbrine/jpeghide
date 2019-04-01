using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using SevenZip.Compression.LZMA;

namespace jpeghide
{
    class Program
    {
        static void Main(string[] args)
        {
            Program p = new Program();
            while(1 < 2)
                p.Run();
            Console.ReadKey();
        }

        public void Run()
        {
            var Key = ConsoleKey.A;

            Console.WriteLine("Press [F1] for hiding");
            Console.WriteLine("Press [F2] for discovery");
            Key = Console.ReadKey().Key;
            Console.Clear();

            if (Key == ConsoleKey.F1)
            {
                Console.WriteLine("Enter base image path: ");
                var baseImage = Console.ReadLine().Replace("\"", "");
                Console.WriteLine("Enter quote file path: ");
                var quotePath = Console.ReadLine().Replace("\"", "");
                Console.WriteLine("Enter destination file path: ");
                var destPath = Console.ReadLine().Replace("\"", "");
                try
                {
                    var data = HideFile(baseImage, quotePath);
                    File.WriteAllBytes(destPath, data);
                    Console.WriteLine($"File created:\n{destPath}");
                    Console.ReadKey();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"ERROR: {ex.Message}");
                    Console.ReadKey();
                }
            }

            if (Key == ConsoleKey.F2)
            {
                Console.WriteLine("Enter base image path: ");
                var baseImage = Console.ReadLine().Replace("\"", "");
                Console.WriteLine("Enter destination file path: ");
                var destPath = Console.ReadLine().Replace("\"", "");
                try
                {
                    var data = DiscoverFile(baseImage);
                    File.WriteAllBytes(destPath, data);
                    Console.WriteLine($"File created:\n{destPath}");
                    Console.ReadKey();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"ERROR: {ex.Message}");
                    Console.ReadKey();
                }
            }

            Console.Clear();
        }

        public string Border = @"Wdy7hjmbX&H-3u_BZDFYJ7SQQUva&-4&T^Xdpp+?xbB!Dhq@EM-!z!UfXp2=$7D+";
        public byte[] DiscoverFile(string ImagePath)
        {
            var image = File.ReadAllBytes(ImagePath);
            int Index = IndexOf(image, Encoding.UTF8.GetBytes(Border)) + Border.Length;
            var file = image.Skip(Index);
            var HiddenFile = DecompressFileLZMA(new MemoryStream(file.ToArray())).ToArray();
            return HiddenFile.ToArray();
        }

        public byte[] HideFile(string ImagePath, string FileToHidePath)
        {
            var image = File.ReadAllBytes(ImagePath).ToList();
            var insert = File.ReadAllBytes(FileToHidePath);
            var ci = CompressFileLZMA(new MemoryStream(insert)).ToArray();

            image.AddRange(Encoding.UTF8.GetBytes(Border));
            image.AddRange(ci);

            return image.ToArray();
        }


        private MemoryStream DecompressFileLZMA(MemoryStream inFile)
        {
            SevenZip.Compression.LZMA.Decoder coder = new SevenZip.Compression.LZMA.Decoder();
            MemoryStream output = new MemoryStream();

            // Read the decoder properties
            byte[] properties = new byte[5];
            inFile.Read(properties, 0, 5);

            // Read in the decompress file size.
            byte[] fileLengthBytes = new byte[8];
            inFile.Read(fileLengthBytes, 0, 8);
            long fileLength = BitConverter.ToInt64(fileLengthBytes, 0);

            coder.SetDecoderProperties(properties);
            coder.Code(inFile, output, inFile.Length, fileLength, null);
            output.Flush();
            output.Close();
            return output;
        }

        private MemoryStream CompressFileLZMA(MemoryStream inFile)
        {
            SevenZip.Compression.LZMA.Encoder coder = new SevenZip.Compression.LZMA.Encoder();
            MemoryStream output = new MemoryStream();
            coder.WriteCoderProperties(output);
            output.Write(BitConverter.GetBytes(inFile.Length), 0, 8);
            
            coder.Code(inFile, output, inFile.Length, -1, null);
            output.Flush();
            output.Close();
            return output;
        }



        public static int IndexOf(byte[] arrayToSearchThrough, byte[] patternToFind)
        {
            if (patternToFind.Length > arrayToSearchThrough.Length)
                return -1;
            for (int i = 0; i < arrayToSearchThrough.Length - patternToFind.Length; i++)
            {
                bool found = true;
                for (int j = 0; j < patternToFind.Length; j++)
                {
                    if (arrayToSearchThrough[i + j] != patternToFind[j])
                    {
                        found = false;
                        break;
                    }
                }
                if (found)
                {
                    return i;
                }
            }
            return -1;
        }

        #region Essentials
        public string LogPath = @"data\Logs.txt";
        public bool NoConsolePrint = false;
        public bool NoFilePrint = false;
        public void Print(string String)
        {
            Check();
            if (!NoFilePrint) WaitWrite(Rooter(LogPath), Tag(String.Replace("\r", "")));
            if (!NoConsolePrint) Console.Write(Tag(String));
        }
        public void Print(string String, bool DoTag)
        {
            Check();
            if (DoTag) { if (!NoFilePrint) WaitWrite(Rooter(LogPath), Tag(String.Replace("\r", ""))); if (!NoConsolePrint) Console.Write(Tag(String)); }
            else { if (!NoFilePrint) WaitWrite(Rooter(LogPath), String.Replace("\r", "")); if (!NoConsolePrint) Console.Write(String); }
        }
        public void PrintLine(string String)
        {
            Check();
            if (!NoFilePrint) WaitWrite(Rooter(LogPath), Tag(String.Replace("\r", "") + Environment.NewLine));
            if (!NoConsolePrint) Console.WriteLine(Tag(String));
        }
        public void PrintLine(string String, bool DoTag)
        {
            Check();
            if (DoTag) { if (!NoFilePrint) WaitWrite(Rooter(LogPath), Tag(String.Replace("\r", "") + Environment.NewLine)); if (!NoConsolePrint) Console.WriteLine(Tag(String)); }
            else { if (!NoFilePrint) WaitWrite(Rooter(LogPath), String.Replace("\r", "") + Environment.NewLine); if (!NoConsolePrint) Console.WriteLine(String); }
        }
        public void PrintLine()
        {
            Check();
            if (!NoFilePrint) WaitWrite(Rooter(LogPath), Environment.NewLine);
            if (!NoConsolePrint) Console.WriteLine();
        }
        public void PrintLines(string[] StringArray)
        {
            Check();
            foreach (string String in StringArray)
            {
                if (!NoFilePrint) WaitWrite(Rooter(LogPath), Tag(String.Replace("\r", "") + Environment.NewLine));
                if (!NoConsolePrint) Console.WriteLine(Tag(String));
            }
        }
        public void PrintLines(string[] StringArray, bool DoTag)
        {
            Check();
            foreach (string String in StringArray)
            {
                if (DoTag) { if (!NoFilePrint) WaitWrite(Rooter(LogPath), Tag(String.Replace("\r", "") + Environment.NewLine)); if (!NoConsolePrint) Console.WriteLine(Tag(String)); }
                else { if (!NoFilePrint) WaitWrite(Rooter(LogPath), String.Replace("\r", "") + Environment.NewLine); if (!NoConsolePrint) Console.WriteLine(String); }
            }
        }
        public void Check()
        {
            if (!NoFilePrint && !System.IO.File.Exists(LogPath)) Touch(LogPath);
        }
        private bool WriteLock = false;
        public void WaitWrite(string Path, string Data)
        {
            while (WriteLock) { System.Threading.Thread.Sleep(20); }
            WriteLock = true;
            System.IO.File.AppendAllText(Path, Data);
            WriteLock = false;
        }
        public string[] ReadData(string DataDir)
        {
            if (System.IO.File.Exists(DataDir))
            {
                List<string> Data = System.IO.File.ReadAllLines(DataDir).ToList<string>();
                foreach (var Line in Data)
                {
                    if (Line == "\n" || Line == "\r" || Line == "\t" || string.IsNullOrWhiteSpace(Line))
                        Data.Remove(Line);
                }
                return Data.ToArray();
            }
            else
                return null;
        }
        public string ReadText(string TextDir)
        {
            if (System.IO.File.Exists(TextDir))
            {
                return System.IO.File.ReadAllText(TextDir);
            }
            return null;
        }
        public string SafeJoin(string[] Array)
        {
            if (Array != null && Array.Length != 0)
                return string.Join("\r\n", Array);
            else return "";
        }
        public void CleanLine()
        {
            Console.Write("\r");
            for (int i = 0; i < Console.WindowWidth - 1; i++) Console.Write(" ");
            Console.Write("\r");
        }
        public void CleanLastLine()
        {
            Console.SetCursorPosition(0, Console.CursorTop - 1);
            CleanLine();
        }
        public string Rooter(string RelPath)
        {
            return System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), RelPath);
        }
        public static string StaticRooter(string RelPath)
        {
            return System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), RelPath);
        }
        public string Tag(string Text)
        {
            return "[" + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + "] " + Text;
        }
        public string Tag()
        {
            return "[" + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + "] ";
        }
        public bool Touch(string Path)
        {
            try
            {
                System.Text.StringBuilder PathCheck = new System.Text.StringBuilder();
                string[] Direcories = Path.Split(System.IO.Path.DirectorySeparatorChar);
                foreach (var Directory in Direcories)
                {
                    PathCheck.Append(Directory);
                    string InnerPath = PathCheck.ToString();
                    if (System.IO.Path.HasExtension(InnerPath) == false)
                    {
                        PathCheck.Append("\\");
                        if (System.IO.Directory.Exists(InnerPath) == false) System.IO.Directory.CreateDirectory(InnerPath);
                    }
                    else
                    {
                        System.IO.File.WriteAllText(InnerPath, "");
                    }
                }
                if (IsDirectory(Path) && System.IO.Directory.Exists(PathCheck.ToString())) { return true; }
                if (!IsDirectory(Path) && System.IO.File.Exists(PathCheck.ToString())) { return true; }
            }
            catch (Exception ex) { PrintLine("ERROR: Failed touching \"" + Path + "\". " + ex.Message, true); }
            return false;
        }
        public bool IsDirectory(string Path)
        {
            try
            {
                System.IO.FileAttributes attr = System.IO.File.GetAttributes(Path);
                if ((attr & System.IO.FileAttributes.Directory) == System.IO.FileAttributes.Directory)
                    return true;
                else
                    return false;
            }
            catch
            {
                if (System.IO.Path.HasExtension(Path)) return true;
                else return false;
            }
        }
        #endregion
    }
}
