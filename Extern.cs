using System.Runtime.InteropServices;

namespace covericonator
{
    internal class Extern
    {
        [DllImport("shell32", CharSet = CharSet.Unicode)]
        internal static extern uint SHGetSetFolderCustomSettings(ref LPSHFOLDERCUSTOMSETTINGS pfcs, string pszPath, uint dwReadWrite);
        [DllImport("shell32", CharSet = CharSet.Unicode)]
        public static extern void SHChangeNotify(uint eventID, uint flags, string path, string path2);
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct LPSHFOLDERCUSTOMSETTINGS
        {
            public uint dwSize;
            public uint dwMask;
            public IntPtr pvid;
            public string pszWebViewTemplate;
            public uint cchWebViewTemplate;
            public string pszWebViewTemplateVersion;
            public string pszInfoTip;
            public uint cchInfoTip;
            public IntPtr pclsid;
            public uint dwFlags;
            public string pszIconFile;
            public uint cchIconFile;
            public int iIconIndex;
            public string pszLogo;
            public uint cchLogo;
        }
    }
}
