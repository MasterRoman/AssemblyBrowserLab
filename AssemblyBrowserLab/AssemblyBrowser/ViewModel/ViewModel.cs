using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel;
using System.Windows.Input;
using System.Windows;
using Microsoft.Win32;
using AssemblyBrowserLibrary;

using System.Runtime.CompilerServices;

namespace AssemblyBrowser
{
    public class ViewModel : INotifyPropertyChanged
    {

        private IAssemblyBrowser assemblyBrowser; 
        public List<NamespaceInfo> namespaces { get; private set; }
        private string _openedFile;

        public ViewModel()
        {
            this.assemblyBrowser = new AssemblyBrowserLibrary.AssemblyBrowser();
            this.namespaces =  new List<NamespaceInfo>();
        }


        private OpenFileCommand _openCommand;
        public OpenFileCommand OpenCommand

        {
            get
            {
                return _openCommand ??
                       (_openCommand = new OpenFileCommand(obj =>
                       {
                           try
                           {
                               OpenFileDialog openFileDialog = new OpenFileDialog();
                   
                               openFileDialog.Multiselect = false;
                               if (openFileDialog.ShowDialog() == true)
                               {
                                   this.namespaces = this.assemblyBrowser.GetAssemblyNamespaces(openFileDialog.FileName);
                                   OnPropertyChanged(nameof(namespaces));
                               }
                           }
                           catch (Exception e)
                           {
                               MessageBox.Show("Loading is failed");
                           }
                       }));
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

    }
}
