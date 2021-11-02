using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssemblyBrowserLibrary
{
    public interface IAssemblyBrowser
    {
        List<NamespaceInfo> getAssemblyNamespaces(string path);
    }
}
