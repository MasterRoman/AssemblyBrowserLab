using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Reflection;
using System.Runtime.CompilerServices;

namespace AssemblyBrowserLibrary.Analyzers
{
    class TypeAnalyzer
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

        public Type typeData { get; private set; }

        private string GetAccessModifier()
        {
            if (typeData.IsNotPublic) return "internal";

            return "public";
        }

        private string GetTypeName()
        {
            if (typeData.IsClass && typeData.BaseType.Name == "MulticastDelegate") return "delegate";
            if (typeData.IsClass) return "class";
            if (!typeData.IsPrimitive && typeData.IsValueType) return "struct";
            if (typeData.IsInterface) return "interface";
            if (typeData.IsEnum) return "enum";
          
            return null;
        }

        private Attributes GetAttributes()
        {
            Attributes attributes = (Attributes)0;
            if (typeData.IsAbstract && typeData.IsSealed)
                return attributes |= Attributes.Static;
            if (typeData.IsAbstract) attributes |= Attributes.Abstract;

            return attributes;
        }

        public TypeInfo GetData(Type type)
        {
            this.typeData = type;
            var members = new List<IType>();
            foreach (var method in typeData.GetMethods(bindingFlags))
            {
                if (!method.IsSpecialName)
                {
                    members.Add(this.methodAnalyzer.GetData(method));
                }
            }

            foreach (var property in typeData.GetProperties(bindingFlags)) members.Add(this.propertyAnalyzer.GetData(property));

            foreach (var constructor in typeData.GetConstructors(bindingFlags)) members.Add(this.methodAnalyzer.GetData(constructor));

            foreach (var field in typeData.GetFields(bindingFlags)) members.Add(this.fieldAnalyzer.GetData(field));


            return new TypeInfo(GetTypeName(), typeData.Name, GetAccessModifier(), members, GetAttributes(), typeData.IsDefined(typeof(ExtensionAttribute)));
        }
    }

}
