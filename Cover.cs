using System.Drawing;
using System.Text.RegularExpressions;
using ATL;

namespace covericonator
{
    using MetadataQuery = (Image?, string);

    internal class Cover
    {
        internal static string? QueryFiles(string path, string pattern)
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
                string? search = QueryFiles(dir, pattern);
                if (search == null) continue;
                found = search;
                break;
            }

            return found;
        }

        // second return argument is file path
        internal static MetadataQuery QueryMetadata(string path, string pattern)
        {
            if (!Directory.Exists(path))
            {
                if (File.Exists(path))
                    throw new ArgumentException("Directory needed but a file was passed.");
                throw new DirectoryNotFoundException();
            }

            MetadataQuery found = (null, "");
            foreach (string file in Directory.EnumerateFiles(path))
            {
                if (!Regex.IsMatch(file, pattern)) continue;
                Track track = new(file);
                IList<PictureInfo> pictures = track.EmbeddedPictures;
                if (pictures.Count == 0) continue;
                found.Item1 = Image.FromStream(new MemoryStream(pictures[0].PictureData));
                found.Item2 = file;
                break;
            }
            if (found.Item1 != null) return found;

            foreach (string dir in Directory.EnumerateDirectories(path))
            {
                MetadataQuery search = QueryMetadata(dir, pattern);
                if (search.Item1 == null) continue;
                found = search;
                break;
            }

            return found;
        }
    }
}
