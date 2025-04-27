using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace covericonator
{
    using MetadataQuery = (Image?, string);

    internal class Program
    {
        static void Process(string path)
        {
            try
            {
                path = Path.GetFullPath(path);
                Console.WriteLine($"Processing: {path}");
                string? cover = Cover.QueryFiles(path, @"(C|c)over\.(jpe?g|png)");
                Image src;
                if (cover == null)
                {
                    Console.WriteLine("Cannot find cover image, searching in audio metadata...");
                    MetadataQuery query = Cover.QueryMetadata(path, @"\.(flac|mp3|m4a|aac|oga|ogg|opus)$");
                    if (query.Item1 == null) throw new FileNotFoundException("Could not find a cover image.");
                    src = query.Item1;
                    cover = query.Item2;
                } else { src = Image.FromFile(cover); }
                Console.WriteLine($"Cover found at: {cover}");
                Console.WriteLine("Converting to icon...");
                Bitmap img = new Bitmap(src, 256, 256);
                Icon icon = IconConvert.IconFromImage(img);
                string icoPath = Environment.ExpandEnvironmentVariables($"%LocalAppData%\\covericonator\\");
                if (!Directory.Exists(icoPath)) Directory.CreateDirectory(icoPath);
                icoPath += $"{Guid.NewGuid()}.ico"; // yes, there's no checking for an existing icon, but who cares
                IconConvert.WriteFile(icon, icoPath);

                Console.WriteLine("Updating directory...");
                Extern.LPSHFOLDERCUSTOMSETTINGS settings = new()
                {
                    dwSize = (uint)Unsafe.SizeOf<Extern.LPSHFOLDERCUSTOMSETTINGS>(),
                    dwMask = 0x10, // FCSM_ICONFILE
                    pszIconFile = icoPath,
                    cchIconFile = 0,
                    iIconIndex = 0,
                };
                uint ret = Extern.SHGetSetFolderCustomSettings(ref settings, path, 0x02); // FCS_FORCEWRITE
                if (ret != 0)
                    throw new Exception(
                        Marshal.GetExceptionForHR((int)ret)?.Message 
                        ?? string.Format("0x{0:X8}", ret)
                    );
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
