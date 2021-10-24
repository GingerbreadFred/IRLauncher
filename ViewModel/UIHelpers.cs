using Microsoft.Win32;
using System.Threading.Tasks;

namespace IRLauncher
{
    class UIHelpers
    {
        public static Task<string> OpenFile(OpenFileDialog ofd)
        {
            if (ofd.ShowDialog() ?? true)
            {

                return Task.FromResult<string>(ofd.FileName);
            }
            else
            {
                return Task.FromResult<string>(string.Empty);
            }
        }
    }
}
