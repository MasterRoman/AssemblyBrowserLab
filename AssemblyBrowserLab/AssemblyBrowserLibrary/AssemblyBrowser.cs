using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Reflection;

using AssemblyBrowserLibrary.Analyzers;

namespace AssemblyBrowserLibrary
{
    public class AssemblyBrowser : IAssemblyBrowser
    {
        public TypeAnalyzer typeAnalyzer { get; }

        public Dictionary<string, List<TypeInfo>> typesDictionary { get; private set; }

        public AssemblyBrowser()
        {
            this.typesDictionary = new Dictionary<string, List<TypeInfo>>();
            this.typeAnalyzer = new TypeAnalyzer();
        }

        public List<NamespaceInfo> GetAssemblyNamespaces(string path)
        {
            this.typesDictionary.Clear();
            var assembly = Assembly.LoadFrom(path);
            var types = assembly.GetTypes();

            foreach (var type in types)
            {
                var typeInfo = typeAnalyzer.GetInfo(type);
                if (this.typesDictionary.TryGetValue(type?.Namespace ?? "No namespace", out List<TypeInfo> namespaceTypes))
                {
                    namespaceTypes.Add(typeInfo);
                }
                else
                {
                    var newList = new List<TypeInfo>();
                    newList.Add(typeInfo);
                    this.typesDictionary[type?.Namespace ?? "No namespace"] = newList;
                }
            }

            AnalyzeExtensions();
            var namespaces = new List<NamespaceInfo>();
            foreach (var type in typesDictionary)
            {
                namespaces.Add(new NamespaceInfo(type.Key, type.Value));
            }

            return namespaces;
        }


        private void AnalyzeExtensions()
        {
            foreach (var keyValue in typesDictionary)
            {
                var types = keyValue.Value;
                for (int index = 0; index < types.Count; index++)
                {
                    var type = types[index];
                    if (!type.isExtension) continue;

                    if (GetMethodsOfExtension(type, out List<MethodInformation> methods))
                    {
                        type.members.RemoveAll(elem => methods.Any(newElem => elem == newElem));
                        foreach (var method in methods)
                        {
                            var extensibleType = FindTypeWithExtExt(method.parameters.Values.First());
                            extensibleType?.members.Add(method);
                        }

                    }
                }
            }
        }

        private bool GetMethodsOfExtension(TypeInfo type, out List<MethodInformation> methods)
        {
            methods = new List<MethodInformation>();
            for (int index = 0; index < type.members.Count; index++)
            {
                var member = type.members[index];
                if (!(member is MethodInformation) || !((MethodInformation)member).isExtension) continue;

                methods.Add((MethodInformation)member);
            }

            if (methods.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private TypeInfo FindTypeWithExtExt(string extensibleType)
        {
            foreach (var keyValue in typesDictionary)
            {
                var types = keyValue.Value;
                foreach (var type in keyValue.Value)
                {
                    if (type.name == extensibleType)
                    {
                        return type;
                    }
                }
            }

            return null;
        }


    }
}
