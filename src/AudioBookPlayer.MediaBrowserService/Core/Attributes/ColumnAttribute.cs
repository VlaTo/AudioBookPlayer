using System;

namespace AudioBookPlayer.MediaBrowserService.Core.Attributes
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    internal sealed class ColumnAttribute : Attribute
    {
        public string Name
        {
            get;
            set;
        }
    }
}