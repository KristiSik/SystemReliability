using System.ComponentModel;
using System.Threading.Tasks;

namespace SystemReliability
{
    public class MainWindowViewModel: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string Time { get; set; }

        public string Lambda0 { get; set; }

        public string Lambda1 { get; set; }

        public string Lambda2 { get; set; }

        public string Lambda3 { get; set; }

        public string Beta { get; set; }

        public MainWindowViewModel()
        {

        }
    }
}
