using Caliburn.Micro;

namespace DorisApp.PosDesktop.ViewModels
{

    public class ShellViewModel : Conductor<object>
    {
        public ShellViewModel(LoginViewModel vm)
        {
            ActiveItem = vm;
        }

    }
}
