using System;
using System.Collections.Generic;
using System.Xml;
using System.Text;
using System.IO;
using Willowsoft.WillowLib.Data.Misc;

namespace Willowsoft.WillowLib.CodeGen
{
    public class CreatorBase
    {
        protected TextWriter mOutput;
        protected ErrorList mErrors;

        protected CreatorBase(TextWriter output, ErrorList errors)
        {
            mOutput = output;
            mErrors = errors;
        }

        protected bool SevereError(string message, params object[] args)
        {
            mErrors.AddSevere(message, args);
            return true;
        }

        protected void WriteLine()
        {
            mOutput.WriteLine();
        }

        protected void WriteLine(string text)
        {
            mOutput.WriteLine(text);
        }

        protected void WriteLine(string format, params object[] args)
        {
            mOutput.WriteLine(format, args);
        }

        protected XmlElement[] GetEntities(XmlElement parent)
        {
            XmlNodeList nodes = parent.SelectNodes(DefConstants.EntityElement);
            XmlElement[] entityArray = new XmlElement[nodes.Count];
            for (int i = 0; i < nodes.Count; i++)
                entityArray[i] = (XmlElement)nodes[i];
            return entityArray;
        }
        
        protected XmlElement[] GetFields(XmlElement entity)
        {
            XmlNodeList nodes = entity.SelectNodes(DefConstants.FieldsElement + "/" + DefConstants.FieldElement);
            XmlElement[] fieldArray = new XmlElement[nodes.Count];
            for (int i = 0; i < nodes.Count; i++)
                fieldArray[i] = (XmlElement)nodes[i];
            return fieldArray;
        }

        protected bool GetFieldName(XmlElement field, out string fieldName)
        {
            fieldName = field.GetAttribute(DefConstants.FieldNameAttrib);
            if (string.IsNullOrEmpty(fieldName))
            {
                return SevereError("Missing [{0}] attribute on <{1}> element",
                    DefConstants.FieldNameAttrib, DefConstants.FieldElement);
            }
            return false;
        }

        protected bool GetClassname(XmlElement entity, out string classname)
        {
            classname = entity.GetAttribute(DefConstants.EntityClassnameAttrib);
            if (string.IsNullOrEmpty(classname))
            {
                return SevereError("Missing [{0}] attribute on <{1}> element",
                    DefConstants.EntityClassnameAttrib, DefConstants.EntityElement);
            }
            return false;
        }

        protected bool GetIdtype(XmlElement entity, out string idtype)
        {
            idtype = entity.GetAttribute(DefConstants.EntityIdtypeAttrib);
            if (string.IsNullOrEmpty(idtype))
            {
                string classname;
                GetClassname(entity, out classname);
                return SevereError("Missing [{0}] attribute on <{1} {2}=\"{3}\"> element",
                    DefConstants.EntityIdtypeAttrib, DefConstants.EntityElement,
                    DefConstants.EntityClassnameAttrib, classname);
            }
            return false;
        }

        protected bool GetCSharpType(XmlElement field, out string type)
        {
            type = field.GetAttribute(DefConstants.FieldTypeAttrib);
            if (string.IsNullOrEmpty(type))
            {
                string name = field.GetAttribute(DefConstants.FieldNameAttrib);
                return SevereError("Missing [{0}] attribute on <{1}> element [{2}]",
                    DefConstants.FieldTypeAttrib, DefConstants.FieldElement, name);
            }
            return false;
        }

        protected bool GetSqltype(XmlElement field, out string sqltype, out bool isid)
        {
            string type;
            if (GetCSharpType(field, out type))
            {
                sqltype = null;
                isid = false;
                return true;
            }
            string isidstring = field.GetAttribute(DefConstants.FieldIsIdAttrib);
            if (string.IsNullOrEmpty(isidstring))
                isidstring = string.Empty;
            isid = (isidstring == "true");
            sqltype = field.GetAttribute(DefConstants.FieldSqltypeAttrib);
            if (string.IsNullOrEmpty(sqltype))
            {
                if (isid)
                    sqltype = "int";
                else if (type == "bool")
                    sqltype = "tinyint";
                else if (type == "int")
                    sqltype = "int";
                else if (type == "decimal")
                    sqltype = "money";
                else
                    return SevereError("C# type \"{0}\" has no default equivalent SQL type", type);
            }
            else
            {
                if (isid)
                    return SevereError("[{0}] attribute not allowed when [{1}] attribute = \"true\"",
                        DefConstants.FieldSqltypeAttrib, DefConstants.FieldIsIdAttrib);
            }
            return false;
        }
    }
}
