﻿using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

namespace LibraProgramming.Xamarin.Popups.Platforms.Android
{
    internal static class VisualElementExtensions
    {
        public static IVisualElementRenderer GetOrCreateRenderer(this VisualElement element)
        {
            var renderer = Platform.GetRenderer(element);

            if (null == renderer)
            {
                renderer = Platform.CreateRendererWithContext(element, Popups.Context);
                Platform.SetRenderer(element, renderer);
            }

            return renderer;
        }
    }
}
