using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace Library
{
    //public abstract class MarkupSingleton : MarkupExtension
    //{

    //}
    public abstract class MarkupSingleton<T> : MarkupExtension where T : MarkupSingleton<T>, new()
    {
        private static MarkupSingleton<T> _instance;

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _instance ?? (_instance = new T());
        }
    }
}
