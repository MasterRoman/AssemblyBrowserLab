using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Reflection;
using System.Runtime.CompilerServices;

namespace AssemblyBrowserLibrary.Analyzers
{
    public class TypeAnalyzer
    {
        private const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy;

        public MemberAnalyzer fieldAnalyzer { get; }
        public MemberAnalyzer methodAnalyzer { get; }
        public MemberAnalyzer propertyAnalyzer { get; }
    
        public TypeAnalyzer()
        {
            this.propertyAnalyzer = new PropertyAnalyzer();
            this.fieldAnalyzer = new FieldAnalyzer();
            this.methodAnalyzer = new MethodAnalyzer();
        }

        public Type typeInfo { get; private set; }

        private string GetAccessModifier()
        {
            if (typeInfo.IsNotPublic) return "internal";

            return "public";
        }

        private string GetTypeName()
        {
            if (typeInfo.IsClass && typeInfo.BaseType.Name == "MulticastDelegate") return "delegate";
            if (typeInfo.IsClass) return "class";
            if (!typeInfo.IsPrimitive && typeInfo.IsValueType) return "struct";
            if (typeInfo.IsInterface) return "interface";
            if (typeInfo.IsEnum) return "enum";
          
            return null;
        }

        private Attributes GetAttributes()
        {
            Attributes attributes = (Attributes)0;
            if (typeInfo.IsAbstract && typeInfo.IsSealed)
                return attributes |= Attributes.Static;
            if (typeInfo.IsAbstract) attributes |= Attributes.Abstract;

            return attributes;
        }

        public TypeInfo GetInfo(Type type)
        {
            this.typeInfo = type;
            var members = new List<IType>();
            foreach (var method in typeInfo.GetMethods(bindingFlags))
            {
                if (!method.IsSpecialName)
                {
                    members.Add(this.methodAnalyzer.GetInfo(method));
                }
            }

            foreach (var property in typeInfo.GetProperties(bindingFlags)) members.Add(this.propertyAnalyzer.GetInfo(property));

            foreach (var constructor in typeInfo.GetConstructors(bindingFlags)) members.Add(this.methodAnalyzer.GetInfo(constructor));

            foreach (var field in typeInfo.GetFields(bindingFlags)) members.Add(this.fieldAnalyzer.GetInfo(field));


            return new TypeInfo(GetTypeName(), typeInfo.Name, GetAccessModifier(), members, GetAttributes(), typeInfo.IsDefined(typeof(ExtensionAttribute)));
        }
    }

}
