using System.Drawing;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace covericonator
{
    internal class Program
    {
        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        static extern bool WritePrivateProfileString(string Section, string Key, string Value, string FilePath);

        static string? RecursiveSearch(string path, string pattern)
        {
            if (!Directory.Exists(path))
            {
                if (File.Exists(path))
                    throw new ArgumentException("Directory needed but a file was passed.");
                throw new DirectoryNotFoundException();
            }

            string? found = null;
            foreach (string file in Directory.EnumerateFiles(path))
            {
                if (!Regex.IsMatch(file, pattern)) continue;
                found = file;
                break;
            }
            if (found != null) return found;

            foreach (string dir in Directory.EnumerateDirectories(path))
            {
                string? search = RecursiveSearch(dir, pattern);
                if (search == null) continue;
                found = search;
                break;
            }

            return found;
        }

        static void Process(string path)
        {
            try
            {
                Console.WriteLine($"Processing: {path}");
                string? cover = RecursiveSearch(path, @"(C|c)over\.(jpe?g|png)");
                if (cover == null)
                    throw new FileNotFoundException("Could not find a cover image.");
                Console.WriteLine($"Cover found at: {cover}");

                Image src = Image.FromFile(cover);
                Console.WriteLine("Converting to icon...");
                Bitmap img = new Bitmap(src, 256, 256);
                Icon icon = IconConvert.IconFromImage(img);
                string icoPath = Environment.ExpandEnvironmentVariables($"%LocalAppData%\\covericonator\\");
                if (!Directory.Exists(icoPath)) Directory.CreateDirectory(icoPath);
                icoPath += $"{Guid.NewGuid()}.ico"; // yes, there's no checking for an existing icon, but who cares
                IconConvert.WriteFile(icon, icoPath);

                Console.WriteLine("Updating directory...");
                string confPath = path + "\\desktop.ini";
                bool ret = WritePrivateProfileString(".ShellClassInfo", "IconResource", icoPath + ",0", confPath);
                if (!ret)
                    throw new Exception($"desktop.ini write failed.");

                // TODO: something regarding icon caching?
                // right now, upon assigning the IconResource windows will display a
                // broken version of the icon, and you need to open folder properties,
                // go to the icon tab, and click OK. Yes, you did nothing, but you did
                // something, and now your icon works properly. What a great OS!
                Console.WriteLine("Success!");
            } catch (Exception e)
            {
                Console.Error.WriteLine($"{e.GetType().Name}: {e.Message}");
            }
        }

        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                foreach (string arg in args) Process(arg);
            }
            else Console.Error.WriteLine("You must pass at least one directory argument.");
            Console.Write("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
