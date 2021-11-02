using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssemblyBrowserLibrary
{
    public abstract class IType
    {
        public string name { get; private set; }
        public string accessModifier { get; private set; }
        public Attributes attributes { get; private set; }
        public string containerDeclaration { get; set; }

        protected abstract string ConvertModifierToString();
        public abstract override string ToString();

        protected IType(string name, string accessModifier, Attributes attributes)
        {
            this.name = name;
            this.accessModifier = accessModifier;
            this.attributes = attributes;
        }
    }
}
