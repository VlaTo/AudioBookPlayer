using System;

namespace AudioBookPlayer.App.Controls
{
    public sealed class MissingTemplatePartException : Exception
    {
        public string Name
        {
            get;
        }

        public Type PartType
        {
            get;
        }

        public MissingTemplatePartException(Type expectedType, string name)
            : base(CreateMessage(expectedType, name))
        {
            PartType = expectedType;
            Name = name;
        }

        private static string CreateMessage(Type expectedType, string name)
        {
            return $"Parn name: '{name}'" + Environment.NewLine + $"Part type: '{expectedType}'";
        }
    }
}
