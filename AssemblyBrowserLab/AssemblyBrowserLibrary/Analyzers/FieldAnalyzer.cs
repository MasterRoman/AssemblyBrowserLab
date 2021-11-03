using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Reflection;

namespace AssemblyBrowserLibrary.Analyzers
{
    public class FieldAnalyzer : MemberAnalyzer
    {
        public FieldInfo fieldInfo { get; private set; }

        protected override string GetAccessModifier()
        {
            if (fieldInfo.IsPublic) { return "public"; }
            if (fieldInfo.IsPrivate) { return "private"; }
            if (fieldInfo.IsAssembly) { return "internal"; }
            if (fieldInfo.IsFamilyAndAssembly) { return "private protected"; }
            return "protected internal";
        }

        protected override Attributes GetAttributes()
        {
            Attributes attribute = (Attributes)0;
            if (fieldInfo.IsInitOnly) attribute |= Attributes.Readonly;
            if (fieldInfo.IsStatic) attribute |= Attributes.Static;
           
            return attribute;
        }

        public override IType GetInfo(MemberInfo data)
        {
            this.fieldInfo = (FieldInfo)data;
            return new FieldInformation(fieldInfo.Name, GetAccessModifier(), ConvertTypeNameToString(fieldInfo.FieldType), GetAttributes());
        }
    }

    public class FieldInformation : IType
    {
        public string fieldType { get; private set; }

        public FieldInformation(string name, string accessModifier, string fieldType, Attributes attributes) : base(name, accessModifier, attributes)
        {
            this.fieldType = fieldType;
            this.containerDeclaration = this.ToString();
        }

   
        protected override string ConvertAttributesToString()
        {
            string attributes = "";
            if (this.attributes == Attributes.Static) { attributes += "static"; }

            if (this.attributes == Attributes.Readonly) { attributes += "readonly"; }
            return attributes;
        }

        public override string ToString()
        {
            string res = "";
            res += this.accessModifier;
            res += " " + ConvertAttributesToString();
            res += " " + this.fieldType;
            res += " " + this.name;
            return res;
        }
    }

}
