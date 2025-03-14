using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace packext
{
    class Program
    {
        struct RWFile
        {
            public uint size;
            public uint offset;
            public string fileName;
        }

        static RWFile[] files;

        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("PackExt v0.1 by Derole\n" + 
                    "Syntax: packext [PackFile] [OutputDir]");
                return;
            }
            string dir = "";
            if (args.Length > 1)
            {
                dir = args[1];
            }

            BinaryReader br = new BinaryReader(File.OpenRead(args[0]));
            if (br.ReadUInt32() != 0x0A)
            {
                Console.WriteLine("File is not a valid RW pack!");
                return;
            }
            ReadTOC(ref br);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            for (int i = 0; i < files.Length; i++)
            {
                ExtractFile(ref br, files[i], dir);
            }
            Console.WriteLine("Extraction finished!");
        }

        static void ReadTOC(ref BinaryReader br)
        {
            files = new RWFile[br.ReadUInt32()];
            Console.WriteLine("Amount of files in Pack: {0}", files.Length);
            for (int i = 0; i < files.Length; i++)
            {
                RWFile file = files[i];
                file.size = br.ReadUInt32();
                file.offset = br.ReadUInt32();
                file.fileName = Encoding.ASCII.GetString(br.ReadBytes(0x80)).Trim('\0');
                files[i] = file;
                br.BaseStream.Position += 0x08;
            }
        }

        static void ExtractFile(ref BinaryReader br, RWFile file, string outputDir)
        {
            byte[] fileData = new byte[file.size];
            br.BaseStream.Position = file.offset;
            br.BaseStream.Read(fileData, 0, fileData.Length);
            string directory = outputDir + "\\" + Path.GetDirectoryName(file.fileName);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            File.WriteAllBytes(outputDir + "\\" + file.fileName, fileData);
            Console.WriteLine("Extracted {0}, offset: {1}, size: {2}", file.fileName, file.offset, file.size);
        }
    }
}
