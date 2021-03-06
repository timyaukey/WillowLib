﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace Willowsoft.WillowLib.CodeGenUI
{
    public class TypeWrapperGenerator
    {
        private string mName;
        private List<Type> mTypes;

        public TypeWrapperGenerator(string name)
        {
            mTypes = new List<Type>();
            mName = name;
        }

        public void Add(Type type)
        {
            mTypes.Add(type);
        }

        protected List<Type> Types
        {
            get { return mTypes; }
        }

        public void Generate(TextWriter writer)
        {
            writer.WriteLine("// Generated by TypeWrapperGenerator");
            writer.WriteLine();
            writer.WriteLine("public partial class {0}{1}", mName, BaseTypeList);
            writer.WriteLine("{");
            
            // Explicit constructor
            string args = string.Empty;
            string argPrefix = "(";
            foreach (Type type in mTypes)
            {
                args += (argPrefix + type.Name + " inner" + type.Name);
                argPrefix = ", ";
            }
            writer.WriteLine("    public {0}{1})", mName, args);
            writer.WriteLine("    {");
            foreach (Type type in mTypes)
            {
                writer.WriteLine("        mInner{0} = inner{1};", type.Name, type.Name);
            }
            writer.WriteLine("    }");

            // Default constructor
            writer.WriteLine("    public {0}()", mName);
            writer.WriteLine("    {");
            foreach (Type type in mTypes)
            {
                writer.WriteLine("        {0} = new {1}();", InnerFieldName(type), type.Name);
            }
            writer.WriteLine("    }");

            // Nested class accessors
            foreach(Type type in mTypes)
            {
                writer.WriteLine();
                writer.WriteLine("    private {0} {1};", type.Name, InnerFieldName(type));
                writer.WriteLine("    public {0} {1} {{ get {{ return {2}; }} }}",
                    type.Name, InnerPropertyName(type), InnerFieldName(type));
            }

            // Expose properties of nested objects
            foreach (Type type in mTypes)
            {
                ExposeProperties(writer, type);
            }

            GenerateAdditional(writer);

            writer.WriteLine("}");
            writer.WriteLine("// end of generated code");
        }

        protected string InnerPropertyName(Type type)
        {
            return "Inner" + type.Name;
        }

        protected string InnerFieldName(Type type)
        {
            return "mInner" + type.Name;
        }

        private void ExposeProperties(TextWriter writer, Type type)
        {
            writer.WriteLine();
            writer.WriteLine("    // {0} properties", type.Name);
            PropertyInfo[] properties = type.GetProperties(
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
            foreach (PropertyInfo property in properties)
            {
                if (IsExposableProperty(property))
                {
                    writer.WriteLine();
                    string propertyTypeName = property.PropertyType.Name;
                    writer.WriteLine("    public {0} {1}_{2}", propertyTypeName, type.Name, property.Name);
                    writer.WriteLine("    {");
                    writer.WriteLine("        get {{ return {0}.{1}; }}", InnerFieldName(type), property.Name);
                    if (property.CanWrite)
                    {
                        writer.WriteLine("        set {{ {0}.{1} = value; }}", InnerFieldName(type), property.Name);
                    }
                    writer.WriteLine("    }");
                }
            }
        }

        protected virtual string BaseTypeList
        {
            get { return string.Empty; }
        }

        protected virtual bool IsExposableProperty(PropertyInfo property)
        {
            return true;
        }

        protected virtual void GenerateAdditional(TextWriter writer)
        {
        }

        public void GenerateToClipboard()
        {
            TextWriter writer = new StringWriter();
            Generate(writer);
            string result = writer.ToString();
            Clipboard.SetText(result, TextDataFormat.Text);
        }
    }

    public class PersistableWrapperGenerator : TypeWrapperGenerator
    {
        public PersistableWrapperGenerator(string name)
            : base(name)
        {
        }

        protected override string BaseTypeList
        {
            get { return " : IPersistable"; }
        }

        protected override bool IsExposableProperty(PropertyInfo property)
        {
            string propName = property.Name;
            if (propName == "IsDirty" || propName == "IsDeleted" || propName == "IsPersisted")
                return false;
            return true;
        }

        protected override void GenerateAdditional(TextWriter writer)
        {
            writer.WriteLine();
            writer.WriteLine("    // IPersistable members");

            GenerateCombinedProperty(writer, "IsDirty", true);
            GenerateCombinedProperty(writer, "IsDeleted", true);
            GenerateCombinedProperty(writer, "IsPersisted", false);

            writer.WriteLine();
            writer.WriteLine("    // You'll have to add your own Validate()...");
        }

        private void GenerateCombinedProperty(TextWriter writer, string propName, bool generateSet)
        {
            string linePrefix;
            string lineEnding;
            writer.WriteLine();
            writer.WriteLine("    public bool {0}", propName);
            writer.WriteLine("    {");
            writer.WriteLine("        get");
            writer.WriteLine("        {");
            linePrefix = "            return ";
            foreach (Type type in Types)
            {
                if (type == Types[Types.Count - 1])
                    lineEnding = ";";
                else
                    lineEnding = string.Empty;
                writer.WriteLine("{0}{1}.{2}{3}", linePrefix, InnerFieldName(type), propName, lineEnding);
                linePrefix = "                |";
            }
            writer.WriteLine("        }");
            if (generateSet)
            {
                writer.WriteLine("        set");
                writer.WriteLine("        {");
                foreach (Type type in Types)
                {
                    writer.WriteLine("            {0}.{1} = value;", InnerFieldName(type), propName);
                }
                writer.WriteLine("        }");
            }
            writer.WriteLine("    }");
        }
    }
}
