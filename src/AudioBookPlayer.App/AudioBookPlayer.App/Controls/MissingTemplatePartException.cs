using System;
using System.Runtime.Serialization;

namespace AudioBookPlayer.App.Controls
{
    [Serializable]
    public class MissingTemplatePartException : Exception
    {
        public string Name
        {
            get;
        }

        public Type PartType
        {
            get;
        }

        public MissingTemplatePartException(Type partType, string name = null)
            : base(CreateMessage(partType, name))
        {
            Name = name;
            PartType = partType;
        }

        protected MissingTemplatePartException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        
        private static string CreateMessage(Type expectedType, string name)
        {
            return $"Part name: '{name}'" + Environment.NewLine + $"Part type: '{expectedType}'";
        }
    }
}