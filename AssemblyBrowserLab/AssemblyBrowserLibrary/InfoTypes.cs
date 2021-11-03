using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using System.Reflection;
using System.Runtime.CompilerServices;

namespace AssemblyBrowserLibrary
{
    public struct NamespaceInfo
    {
        public string name { get; private set; }
        public List<TypeInfo> types { get; private set; }

        public NamespaceInfo(string name, List<TypeInfo> types)
        {
            this.name = name;
            this.types = types;
        }
    }

    public class TypeInfo : IType
    {
        public List<IType> members { get; private set; }

        public bool isExtension { get; private set; }

        public string typeName { get; private set; }

        public TypeInfo(string type, string name, string accessModifier, List<IType> members, Attributes attributes, bool isExtension) : base(name, accessModifier, attributes)
        {
            this.isExtension = isExtension;
            this.typeName = type;
            this.members = members;
            this.containerDeclaration = this.ToString();
        }

        protected override string ConvertAttributesToString()
        {
            if (this.attributes == Attributes.Static) return "static ";
            if (this.attributes == Attributes.Abstract) return "abstract ";
         
            return "";
        }

        public override string ToString()
        {
            string res = "";
            res += this.accessModifier;
            res += " " + ConvertAttributesToString();
            res += " " + this.typeName;
            res += " " + this.name;
            return res;
        }
    }
}
