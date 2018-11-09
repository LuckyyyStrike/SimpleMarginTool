using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SimpleMarginTool.ViewModels
{
    public interface IViewModel : INotifyPropertyChanged
    {
        void OnPropertyChanged([CallerMemberName] string propertyName = null);
    }
}