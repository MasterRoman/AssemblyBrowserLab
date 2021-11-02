using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Reflection;
using System.Runtime.CompilerServices;

namespace AssemblyBrowserLibrary.Analyzers
{
    public class MethodAnalyzer : MemberAnalyzer
    {
        public MethodBase methodInfo { get; private set; }

        protected override string GetAccessModifier()
        {
            if (methodInfo.IsPublic) return "public";
            if (methodInfo.IsPrivate) return "private";
            if (methodInfo.IsAssembly) return "internal";
            if (methodInfo.IsFamilyAndAssembly) return "private protected";

            return "protected internal";
        }

        protected override Attributes GetAttributes()
        {
            Attributes attribute = (Attributes)0;
            if (methodInfo.IsStatic) attribute |= Attributes.Static;
            if (methodInfo.IsAbstract) attribute |= Attributes.Abstract;
            else if (methodInfo.IsVirtual) attribute |= Attributes.Virtual;

            return attribute;
        }

        private Dictionary<string, string> GetParameters()
        {
            var parameters = new Dictionary<string, string>();
            try
            {
                foreach (ParameterInfo parameterInfo in this.methodInfo.GetParameters())
                {
                    parameters.Add(parameterInfo.Name, ConvertTypeNameToString(parameterInfo.ParameterType));
                }

                return parameters;
            }
            catch (Exception e)
            {
                return parameters;
            }
        }

        public override IType GetData(MemberInfo data)
        {
            this.methodInfo = (MethodBase)data;
            string returnType = string.Empty;
            var isExtension = false;
            if (data is MethodInfo)
            {
                var method = ((MethodInfo)methodInfo);
                returnType = ConvertTypeNameToString(method.ReturnType);
                isExtension = (method.GetBaseDefinition().DeclaringType == method.DeclaringType) &&
                                  methodInfo.IsDefined(typeof(ExtensionAttribute));
            }

            return new MethodInformation(methodInfo.Name, GetAccessModifier(),
                returnType, GetParameters(), GetAttributes(), isExtension);
        }
    }

    public class MethodInformation : IType
    {
 
        public bool isExtension { get; private set; }
        public string returnType { get; private set; }

        public Dictionary<string, string> parameters { get; private set; }

        public MethodInformation(string name, string accessModifier, string returnType, Dictionary<string, string> parameters, Attributes attributes, bool isExtension) : base(name, accessModifier, attributes)
        {
            this.isExtension = isExtension;
            this.returnType = returnType;
            this.parameters = parameters;
            this.containerDeclaration = this.ToString();
        }

        protected override string ConvertAttributesToString()
        {
            string attributes = "";
            if (this.attributes == Attributes.Static) { attributes += "static"; }
            if (this.attributes == Attributes.Sealed)  { attributes += "sealed"; }
            if (this.attributes == Attributes.Abstract) { attributes += "abstract"; }
            if (this.attributes == Attributes.Virtual) { attributes += "virtual"; }
            return attributes;
        }

        public override string ToString()
        {
            string res = string.Empty;
            res += this.accessModifier;
            res += " " + ConvertAttributesToString();
            res += " " + this.returnType;
            res += " " + this.name;
            res += "(";
            foreach (var param in this.parameters)
            {
                res += param.Value + " " + param.Key + ", ";
            }

            if (this.parameters.Count > 0) res = res.Remove(res.Length - 2, 2);
            res += ")";
            if (this.isExtension) res += "(extension method)";

            return res;
        }
    }
}
