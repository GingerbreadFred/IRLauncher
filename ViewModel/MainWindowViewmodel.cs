using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;

namespace IRLauncher
{
    internal class MainWindowViewModel : ViewModelBase
    {
        private ObservableCollection<string> cars;
        private string selectedCar;
        private string newCar;
        private IRacingExeChecker exeChecker;
        private IDialogCoordinator dialogCoordinator;
        private Config config;
        private bool uiRunning;
        private bool simRunning;

        public MainWindowViewModel(IDialogCoordinator dialogCoordinator, Config config, IRacingExeChecker exeChecker)
        {
            PopulateCarList(config);
            this.NewCarCommand = new RelayCommand<object>(CreateNewCar);
            this.DeleteCarCommand = new RelayCommand<object>(DeleteCar);
            this.LaunchUICommand = new RelayCommand<object>(LaunchUI);
            this.exeChecker = exeChecker;
            this.dialogCoordinator = dialogCoordinator;
            this.config = config;
            this.exeChecker.Start();
            this.exeChecker.SimStateChanged += ExeChecker_SimStateChanged;
            this.exeChecker.UIStateChanged += ExeChecker_UIStateChanged;
            this.simRunning = this.exeChecker.SimActive;
            this.uiRunning = this.exeChecker.UIActive;
        }

        private async void LaunchUI(object obj)
        {
            if (this.UIRunning)
            {
                await dialogCoordinator.ShowMessageAsync(this, "Error", "UI Already Running");
                return;
            }

            if (!System.IO.File.Exists(config.UIPath))
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.DefaultExt = ".exe";
                ofd.Filter = "Executable Files (.exe)|*.exe";
                string file = await UIHelpers.OpenFile(ofd);

                if (!System.IO.File.Exists(file))
                {
                    return;
                }

                config.UIPath = file;
                ConfigIO.WriteToDisk(config);
            }

            Process.Start(config.UIPath);
        }

        private void ExeChecker_UIStateChanged(bool state)
        {
            this.UIRunning = state;
        }

        private async void ExeChecker_SimStateChanged(bool state)
        {
            this.SimRunning = state;
            if (!state && this.selectedCar != null)
            {
                MessageDialogResult result = await dialogCoordinator.ShowMessageAsync(this, "Update", string.Format("Update config for {0} from latest iRacing config?", this.SelectedCar), MessageDialogStyle.AffirmativeAndNegative);
                if (result == MessageDialogResult.Affirmative)
                {
                    IRacingConfigFile.CopyFromAppIni(this.config, this.selectedCar);
                }
            }
        }

        private void PopulateCarList(Config config)
        {
            var carList = config.Cars.Select((c) => c.CarName).OrderBy((name) => name);
            this.Cars = new ObservableCollection<string>(carList);
        }

        public ObservableCollection<string> Cars
        {
            get => cars;
            set
            {
                cars = value;
                OnPropertyChanged(nameof(Cars));
            }
        }

        public string NewCar
        {
            get => newCar;
            set
            {
                newCar = value;
                OnPropertyChanged(nameof(NewCar));
            }
        }

        public string SelectedCar
        {
            get => selectedCar;
            set
            {
                selectedCar = value;
                OnPropertyChanged(nameof(SelectedCar));
                if (value != null)
                {
                    SelectCar(value);
                }
            }
        }

        public bool SimRunning
        {
            get => simRunning;
            set
            {
                simRunning = value;
                OnPropertyChanged(nameof(SimRunning));
            }
        }

        public bool UIRunning
        {
            get => uiRunning;
            set
            {
                uiRunning = value;
                OnPropertyChanged(nameof(UIRunning));
            }
        }
        public ICommand NewCarCommand { get; }
        public ICommand DeleteCarCommand { get; }
        public ICommand LaunchUICommand { get; }


        private async void CreateNewCar(object param)
        {
            string carName = await dialogCoordinator.ShowInputAsync(this, "New Car", "Specify Car Name");
            if (carName != null)
            {
                if (config.Cars.Any((c) => c.CarName.ToLower() == carName.ToLower()))
                {
                    await dialogCoordinator.ShowMessageAsync(this, "Error", "Car Already Exists", MessageDialogStyle.Affirmative);
                    return;
                }

                IRacingConfigFile.CopyFromAppIni(this.config, carName);
                PopulateCarList(config);
                SelectedCar = carName;
            }
        }

        private async void DeleteCar(object param)
        {
            MessageDialogResult result = await dialogCoordinator.ShowMessageAsync(this, "Delete Car", string.Format("Delete {0}?", SelectedCar), MessageDialogStyle.AffirmativeAndNegative);
            if (result == MessageDialogResult.Affirmative)
            {
                config.DeleteCar(SelectedCar);
                ConfigIO.WriteToDisk(config);
                PopulateCarList(config);
            }
        }

        private void SelectCar(object param)
        {
            IRacingConfigFile.CopyToAppIni(this.config, this.SelectedCar);
        }
    }
}
