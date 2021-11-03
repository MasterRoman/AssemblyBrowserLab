using System;
using Xunit;
using System.Collections.Generic;

using AssemblyBrowserLibrary;
using AssemblyBrowserLibrary.Analyzers;

namespace AssemblyBrowserTests
{
    public class AssemblyBrowserLibraryTest
    {
        private const string path = "D:\\SPPLABS\\AssemblyBrowserLab\\AssemblyBrowserLab\\testlibs\\TracerLibrary.dll";
        private IAssemblyBrowser assemblyBrowser;

        public AssemblyBrowserLibraryTest()
        {
            this.assemblyBrowser = new AssemblyBrowser();
        }

        private IType GetInfo(int typeIndex,int memberIndex)
        {
            var data = assemblyBrowser.GetAssemblyNamespaces(path);
            return data[0].types[typeIndex].members[memberIndex];
        }


        [Fact]
        public void TestFieldAnalyzer()
        {
            var field = (FieldInformation)GetInfo(2,11);
            string name = field.name;
            Assert.Equal("filePath", name);
        }

        [Fact]
        public void TestMethodAnalyzer()
        {
            var method = (MethodInformation)GetInfo(2,10);
            string name = method.name;
            Assert.Equal(".ctor", name);
        }

        [Fact]
        public void TestPropertyAnalyzer()
        {
            var property = (PropertyInformation)GetInfo(7, 11);
            string name = property.name;
            Assert.Equal("time", name);
        }

        [Fact]
        public void TestNamespace()
        {
            var namespaces = assemblyBrowser.GetAssemblyNamespaces(path);
            Assert.Equal(namespaces[0].name, "TracerLibrary");
        }

        [Fact]
        public void TextExtensionMethod()
        {
            var extMethod = (MethodInformation)GetInfo(0, 10);
            Assert.Equal(extMethod.isExtension, true);
            Assert.Equal(extMethod.name, "MethodA");

        }
    }
}
