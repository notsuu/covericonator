# covericonator
A small program for converting album cover art to folder icons. Now when you open your music library you'll see neat artwork and not folders... with said artworks...

<p align="center"><img src="preview.png" alt="covericonator preview"></p>

# Usage
Make sure you have [.NET Runtime 8](https://dotnet.microsoft.com/download/dotnet/8.0) installed. 
After that, it's very simple to use; just drag your album folders into the executable and it <sub>(should)</sub> work.

This program assumes your albums have either a `cover.jpg`/`cover.png`/etc. file in the root or any of the subfolders; if that doesn't exist, it will try to extract the cover from the first audio file with one.

Once you apply the cover to your folder, you might need to refresh the Explorer window for it to display properly.