using System;

namespace AudioBookPlayer.App.Core.Attributes
{
    public sealed class ViewModelAttribute : Attribute
    {
        public Type Type
        {
            get;
        }

        public ViewModelAttribute(Type type)
        {
            Type = type;
        }
    }
}
