using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace AudioBookPlayer.App.Controls
{
    public class TemplatedControl : TemplatedView
    {
        protected TPart GetTemplatePart<TPart>(string name, bool throwException = true)
            where TPart : VisualElement
        {
            //this.ControlTemplate
            var element = GetTemplateChild(name) as TPart;

            if (null == element && throwException)
            {
                throw new MissingTemplatePartException(typeof(TPart), name);
            }

            return element;
        }
    }
}
