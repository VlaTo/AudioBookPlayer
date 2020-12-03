using Prism.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace AudioBookPlayer.App.Core
{
    public sealed class PlaybackPositionChanged : PubSubEvent<double>
    {
    }
}
