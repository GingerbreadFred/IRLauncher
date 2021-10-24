using System.ComponentModel;

namespace IRLauncher
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public void OnPropertyChanged(string prop)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
