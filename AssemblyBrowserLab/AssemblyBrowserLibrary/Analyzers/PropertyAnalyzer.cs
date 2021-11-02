using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Reflection;

namespace AssemblyBrowserLibrary.Analyzers
{
    public class PropertyAnalyzer : MemberAnalyzer
    {
        public PropertyInfo propertyInfo { get; private set; }

        protected override string GetAccessModifier()
        {
            var accessor = propertyInfo.GetAccessors(true)[0];
            if (accessor.IsPrivate) return "private";
            if (accessor.IsPublic) return "public";
            if (accessor.IsAssembly) return "internal";
            if (accessor.IsFamilyAndAssembly) return "private protected";

            return "protected internal";
        }

        protected override Attributes GetAttributes()
        {
            var accessor = propertyInfo.GetAccessors(true)[0];
            Attributes attributes = (Attributes)0;
            if (accessor.IsAbstract) attributes |= Attributes.Abstract;
            else if (accessor.IsVirtual) attributes |= Attributes.Virtual;
            if (accessor.IsStatic) attributes |= Attributes.Static;

            return attributes;
        }

        public override IType GetData(MemberInfo data)
        {
            this.propertyInfo = (PropertyInfo)data;
            return new PropertyInformation(propertyInfo.Name, GetAccessModifier(),
                ConvertTypeNameToString(propertyInfo.PropertyType), GetAttributes(), propertyInfo.GetAccessors(true));
        }
    }

    public class PropertyInformation : IType
    {
        public string propertyType { get; private set; }

        public MethodInfo[] accessors { get; private set; }

        public PropertyInformation(string name, string accessModifier, string propertyType, Attributes attributes, MethodInfo[] accessors) : base(
            name, accessModifier, attributes)
        {
            this.propertyType = propertyType;
            this.accessors = accessors;
            this.containerDeclaration = this.ToString();
        }

        protected override string ConvertAttributesToString()
        {
            string attributes = "";
            if (this.attributes == Attributes.Static) { attributes += "static"; }
            if (this.attributes == Attributes.Sealed) { attributes += "sealed"; }
            if (this.attributes == Attributes.Abstract) { attributes += "abstract"; }
            if (this.attributes == Attributes.Virtual) { attributes += "virtual"; }
            return attributes;
        }

        public override string ToString()
        {
            string res = "";
            res += this.accessModifier + " ";
            res += ConvertAttributesToString();
            res += this.propertyType + " ";
            res += this.name;
            res += " { ";
            string accessors = "";
            foreach (var accessor in this.accessors)
            {
                if (accessor.IsSpecialName)
                {
                    if (accessor.IsPrivate) accessors += "private ";
                    accessors += accessor.Name +", ";
                }
            }

            if (accessors.Length > 0) accessors = accessors.Remove(accessors.Length - 2, 2);
            res += accessors + " } ";
            return res;
        }

    }
}
