using Android.OS;
using AudioBookPlayer.App.Models;

namespace AudioBookPlayer.App.Android.Models
{
    internal sealed class SectionMetadata : ISectionMetadata
    {
        private readonly Bundle bundle;

        public string Name => bundle.GetString("Name");

        public int Index => bundle.GetInt("Index");

        public string ContentUri => bundle.GetString("ContentUri");

        public SectionMetadata(Bundle bundle)
        {
            this.bundle = bundle;
        }
    }
}