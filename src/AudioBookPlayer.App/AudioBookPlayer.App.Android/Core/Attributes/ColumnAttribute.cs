using System;

namespace AudioBookPlayer.App.Android.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class ColumnAttribute : Attribute
    {
        public string Name
        {
            get;
            set;
        }
    }
}