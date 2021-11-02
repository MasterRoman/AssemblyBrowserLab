using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Reflection;

namespace AssemblyBrowserLibrary.Analyzers
{
    public abstract class MemberAnalyzer
    {
        protected abstract string GetAccessModifier();

        protected abstract Attributes GetAttributes();

        public abstract IType GetData(MemberInfo data);

        protected string ConvertTypeNameToString(Type type)
        {
            if (type.IsGenericType)
            {
                var genericArguments = type.GetGenericArguments();
                var res = type.Name.Remove(type.Name.Length - 2, 2) + "<";
                var str = "";
                foreach (var argument in genericArguments)
                {
                    str += ConvertTypeNameToString(argument) + ", ";
                }

                str = str.Length > 0 ? str.Remove(str.Length - 2, 2) : str;
                return res + str + ">";
            }
            else
            {
                return type.Name;
            }
        }

     
    }
}
