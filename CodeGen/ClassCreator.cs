﻿using System;
using System.Collections.Generic;
using System.Xml;
using System.Text;
using System.IO;
using Willowsoft.WillowLib.Data.Misc;

namespace Willowsoft.WillowLib.CodeGen
{
    /// <summary>
    /// Create a partial class definition for an entity whose properties
    /// are defined in an external XML file. The result subclasses "Entity"
    /// and is written to a TextWriter, along with an accompanying "EntityId"
    /// subclass. There is generally another hand coded partial class file for
    /// the same entity with additional class members.
    /// </summary>
    public class ClassCreator : CreatorBase
    {
        public ClassCreator(TextWriter output, ErrorList errors)
            : base(output, errors)
        {
        }

        /// <summary>
        /// Output a partial class and Id class for the specified "entity" element.
        /// Errors will be added to an ErrorList for all problems found in the entity
        /// definition.
        /// </summary>
        /// <param name="entity">An XML element named "entity" with all the definitions.</param>
        /// <param name="output">The TextWriter to output to.</param>
        /// <param name="errors">The ErrorList to add errors to.</param>
        public void OutputPartialClasses(XmlElement entity)
        {
            string classname;
            if (GetClassname(entity, out classname))
                return;
            string idtype;
            if (GetIdtype(entity, out idtype))
                return;
            string nmspc = entity.GetAttribute(DefConstants.EntityNamespaceAttrib);
            if (string.IsNullOrEmpty(nmspc))
            {
                SevereError("Missing [{0}] attribute on <{1} {2}=\"{3}\"> element",
                    DefConstants.EntityNamespaceAttrib, DefConstants.EntityElement,
                    DefConstants.EntityClassnameAttrib, classname);
                return;
            }
            WriteLine("// Generated by {0} at {1}", this.GetType().FullName, DateTime.Now);
            WriteLine("using System;");
            WriteLine("using System.Diagnostics;");
            WriteLine("using System.Collections.Generic;");
            WriteLine("using System.Data.SqlClient;");
            WriteLine("using Willowsoft.WillowLib.Data.Entity;");
            WriteLine("using Willowsoft.WillowLib.Data.Sql;");
            WriteLine("using {0};", nmspc);
            WriteLine();
            WriteLine("namespace {0}", nmspc);
            WriteLine("{");
            if (OutputIdClass(idtype))
                return;
            if (OutputEntityClass(entity, classname, idtype))
                return;
            WriteLine("}");
            XmlElement repository = (XmlElement)entity.SelectSingleNode(DefConstants.RepositoryElement);
            if (repository != null)
            {
                WriteLine();
                nmspc = repository.GetAttribute(DefConstants.RepositoryNamespaceAttrib);
                WriteLine("namespace {0}", nmspc);
                WriteLine("{");
                if (OutputSqlRepositoryClass(entity, classname, idtype, repository))
                    return;
                WriteLine("}");
            }
        }

        private bool OutputIdClass(string idtype)
        {
            WriteLine("    [DebuggerStepThrough]");
            WriteLine("    public class {0} : EntityId", idtype);
            WriteLine("    {");
            WriteLine("        public {0}() {{}}", idtype);
            WriteLine("        public {0}(int value) : base(value) {{}}", idtype);
            WriteLine("    }");
            WriteLine();
            return false;
        }

        private bool OutputEntityClass(XmlElement entity, string classname, string idtype)
        {
            WriteLine("    public partial class {0} : Entity<{1}>", classname, idtype);
            WriteLine("    {");
            if (OutputPrivateFields(entity))
                return true;
            if (OutputConstructors(entity, classname, idtype))
                return true;
            if (OutputProperties(entity))
                return true;
            WriteLine("    }");
            return false;
        }

        private bool OutputPrivateFields(XmlElement entity)
        {
            WriteLine("        #region Private property fields");
            WriteLine();
            foreach (XmlElement field in GetFields(entity))
            {
                string name;
                if (GetFieldName(field, out name))
                    return true;
                string type;
                if (GetCSharpType(field, out type))
                    return true;
                WriteLine("        private {0} m{1};", type, name);
            }
            WriteLine();
            WriteLine("        #endregion");
            WriteLine();
            return false;
        }

        private bool OutputConstructors(XmlElement entity, string classname, string idtype)
        {
            WriteLine("        #region Constructors");
            WriteLine();
            WriteLine("        [DebuggerStepThrough]");
            WriteLine("        public {0}({1} Id_,", classname, idtype);
            foreach (XmlElement field in GetFields(entity))
            {
                string name;
                if (GetFieldName(field, out name))
                    return true;
                string type;
                if (GetCSharpType(field, out type))
                    return true;
                WriteLine("            {0} {1}_,", type, name);
            }
            WriteLine("            DateTime CreateDate_,");
            WriteLine("            DateTime ModifyDate_)");
            WriteLine("            : base(Id_, CreateDate_, ModifyDate_)");
            WriteLine("        {");
            foreach (XmlElement field in GetFields(entity))
            {
                string name;
                if (GetFieldName(field, out name))
                    return true;
                WriteLine("            m{0} = {0}_;", name);
            }
            WriteLine("        }");

            WriteLine();
            WriteLine("        [DebuggerStepThrough]");
            WriteLine("        public {0}()", classname);
            WriteLine("            : this(new {0}(),", idtype);
            foreach (XmlElement field in GetFields(entity))
            {
                string type;
                if (GetCSharpType(field, out type))
                    return true;
                string sqltype;
                bool isid;
                if (GetSqltype(field, out sqltype, out isid))
                    return true;
                string init = field.GetAttribute(DefConstants.FieldInitAttrib);
                string value;
                if (type == "string")
                {
                    if (string.IsNullOrEmpty(init))
                        value = "string.Empty";
                    else
                        value = init;
                }
                else if (type == "decimal")
                {
                    if (string.IsNullOrEmpty(init))
                        value = "0m";
                    else
                        value = init;
                }
                else if (type == "int")
                {
                    if (string.IsNullOrEmpty(init))
                        value = "0";
                    else
                        value = init;
                }
                else if (isid)
                {
                    value = "new " + type + "()";
                }
                else
                {
                    if (string.IsNullOrEmpty(init))
                    {
                        string name;
                        if (GetFieldName(field, out name))
                            return true;
                        return SevereError("<{0}> [{1}] of type [{2}] requires [{3}] attribute",
                            DefConstants.FieldElement, name, type, DefConstants.FieldInitAttrib);
                    }
                    else
                        value = init;
                }
                WriteLine("            {0},", value);
            }
            WriteLine("            DateTime.Now, DateTime.Now)");
            WriteLine("        {");
            WriteLine("        }");
            WriteLine();
            WriteLine("        #endregion");
            return false;
        }

        private bool OutputProperties(XmlElement entity)
        {
            WriteLine();
            WriteLine("        #region Encapsulated fields");
            foreach (XmlElement field in GetFields(entity))
            {
                WriteLine();
                string name = field.GetAttribute(DefConstants.FieldNameAttrib);
                string type;
                if (GetCSharpType(field, out type))
                    return true;
                WriteLine("        public {0} {1}", type, name);
                WriteLine("        {");
                WriteLine("            [DebuggerStepThrough]");
                WriteLine("            get {{ return m{0}; }}", name);
                WriteLine("            [DebuggerStepThrough]");
                WriteLine("            set {{ PropertySet(ref m{0}, value); }}", name);
                WriteLine("        }");
            }
            WriteLine();
            WriteLine("        #endregion");
            return false;
        }

        private bool OutputSqlRepositoryClass(XmlElement entity, string classname,
            string idtype, XmlElement repository)
        {
            string datasetname = repository.GetAttribute(DefConstants.RepositoryDatasetNameAttrib);
            if (string.IsNullOrEmpty(datasetname))
                return SevereError("Missing [{0}] attribute on <{1}> element",
                    DefConstants.RepositoryDatasetNameAttrib, DefConstants.RepositoryElement);
            WriteLine("    public partial class Sql{0}Repository", classname);
            WriteLine("        : SqlEntityRepository<{0}, {1}, {2}.{0}Row>,",
                classname, idtype, datasetname);
            WriteLine("        I{0}Repository", classname);
            WriteLine("    {");
            WriteLine("        #region SqlEntityRepository Members");
            if (OutputRepCreateEntity(entity, classname, idtype, datasetname))
                return true;
            if (OutputRepCreateEntities(classname, datasetname))
                return true;
            if (OutputRepAddInsUpdParams(entity, classname))
                return true;
            if (OutputRepEntityName(classname))
                return true;
            WriteLine();
            WriteLine("        #endregion");
            WriteLine("    }");
            return false;
        }

        private bool OutputRepCreateEntity(XmlElement entity, string classname,
            string idtype, string datasetname)
        {
            WriteLine();
            WriteLine("        [DebuggerStepThrough]");
            WriteLine("        protected override {0} CreateEntity({1}.{0}Row dataRow)",
                classname, datasetname);
            WriteLine("        {");
            WriteLine("            {0} entity = new {0}(new {1}(dataRow.{1}),", classname, idtype);
            foreach (XmlElement field in GetFields(entity))
            {
                string type;
                if (GetCSharpType(field, out type))
                    return true;
                string sqltype;
                bool isid;
                if (GetSqltype(field, out sqltype, out isid))
                    return true;
                string fieldName;
                if (GetFieldName(field, out fieldName))
                    return true;
                string value;
                if (isid)
                    value = "new " + type + "(dataRow." + fieldName + ")";
                else if (type == "bool")
                    value = "dataRow." + fieldName + ">0?true:false";
                else if (type == "string" || type=="int" || type=="DateTime")
                    value = "dataRow." + fieldName;
                else
                    value = "(" + type + ")dataRow." + fieldName;
                WriteLine("                {0},", value);
            }
            WriteLine("                dataRow.CreateDate, dataRow.ModifyDate);");
            WriteLine("            return entity;");
            WriteLine("        }");
            return false;
        }

        private bool OutputRepCreateEntities(string classname, string datasetname)
        {
            WriteLine();
            WriteLine("        [DebuggerStepThrough]");
            WriteLine("        public override List<{0}> CreateEntities(SqlDataAdapter adapter)",
                classname);
            WriteLine("        {");
            WriteLine("            {0} dataSet = new {0}();", datasetname);
            WriteLine("            adapter.Fill(dataSet.{0});", classname);
            WriteLine("            return CreateEntities(dataSet.{0});", classname);
            WriteLine("        }");
            return false;
        }

        private bool OutputRepAddInsUpdParams(XmlElement entity, string classname)
        {
            WriteLine();
            WriteLine("        [DebuggerStepThrough]");
            WriteLine("        protected override void AddInsertUpdateParams(SqlCommand cmd, {0} entity)",
                classname);
            WriteLine("        {");
            foreach (XmlElement field in GetFields(entity))
            {
                string fieldName;
                if (GetFieldName(field, out fieldName))
                    return true;
                string type;
                if (GetCSharpType(field, out type))
                    return true;
                string sqltype;
                bool isid;
                if (GetSqltype(field, out sqltype, out isid))
                    return true;
                string addFunction;
                string value;
                if (isid)
                {
                    addFunction = "AddParamInputId";
                    value = "entity." + fieldName + ".Value";
                }
                else if (sqltype.ToLower().StartsWith("varchar("))
                {
                    addFunction = "AddParamVarchar";
                    value = "entity." + fieldName + " ?? string.Empty";
                }
                else if (sqltype.ToLower() == "tinyint")
                {
                    addFunction = "AddParamTinyint";
                    if (type == "int" || type == "bool")
                        value = "entity." + fieldName;
                    else
                        value = "(int)entity." + fieldName;
                }
                else if (sqltype.ToLower() == "int")
                {
                    addFunction = "AddParamInt";
                    value = "entity." + fieldName;
                }
                else if (sqltype.ToLower() == "money")
                {
                    addFunction = "AddParamMoney";
                    value = "entity." + fieldName;
                }
                else if (sqltype.ToLower().EndsWith("datetime"))
                {
                    addFunction = "AddParamDatetime";
                    value = "entity." + fieldName;
                }
                else
                    return SevereError("Unsupported data type");
                WriteLine("            SqlHelper.{0}(cmd, \"@{1}\", {2});",
                    addFunction, fieldName, value);
            }
            WriteLine("        }");
            return false;
        }

        private bool OutputRepEntityName(string classname)
        {
            WriteLine();
            WriteLine("        protected override string EntityName");
            WriteLine("        {");
            WriteLine("            [DebuggerStepThrough]");
            WriteLine("            get {{ return \"{0}\"; }}", classname);
            WriteLine("        }");
            return false;
        }
    }
}
