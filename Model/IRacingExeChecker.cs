
using System.Diagnostics;
using System.Timers;

namespace IRLauncher
{
    class IRacingExeChecker
    {
        private static Timer timer;
        private bool simActive;
        private bool uiActive;

        public delegate void AppActiveStateChanged(bool state);
        public event AppActiveStateChanged SimStateChanged;
        public event AppActiveStateChanged UIStateChanged;
        public bool SimActive { get => simActive; set { if (simActive != value) { simActive = value; SimStateChanged?.Invoke(simActive); } } }
        public bool UIActive { get => uiActive; set { if (uiActive != value) { uiActive = value; UIStateChanged?.Invoke(uiActive); } } }

        public IRacingExeChecker()
        {
            SimActive = false;
            UIActive = false;
        }

        public void Start()
        {
            timer = new Timer(2000);
            timer.Elapsed += (source, e) => UpdateProcesses();
            timer.Enabled = true;
        }

        public void Stop()
        {
            timer.Enabled = false;
        }

        private void UpdateProcesses()
        {
            var processes = Process.GetProcesses();

            bool simFound = false;
            bool uiFound = false;

            foreach (var process in processes)
            {
                if (process.ProcessName == "iRacingSim64DX11")
                {
                    simFound = true;
                }
                if (process.ProcessName =="iRacingUI")
                {
                    uiFound = true;
                }

                if (simFound && uiFound)
                {
                    break;
                }
            }

            SimActive = simFound;
            UIActive = uiFound;
        }
    }
}
