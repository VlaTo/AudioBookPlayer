using System;
using System.Collections.Generic;
using System.Text;

namespace AudioBookPlayer.App.Core
{
    public class DefaultSettings
    {
        public static readonly DefaultSettings Current;

        public string LibraryRootPath
        {
            get
            {
#if __ANDROID__
                return "";
#elif __IOS__
                return "";
#else
                return "//";
#endif
            }
        }

        private DefaultSettings()
        {

        }

        static DefaultSettings()
        {
            Current = new DefaultSettings();
        }
    }
}
