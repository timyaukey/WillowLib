﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using Willowsoft.WillowLib.Data.Misc;

namespace Willowsoft.WillowLib.CodeGen
{
    public class StoredProcCreator : CreatorBase
    {
        public StoredProcCreator(TextWriter output, ErrorList errors)
            : base(output, errors)
        {
        }

        public void OutputProcScript(XmlElement parent)
        {
            OutputProcScript(parent, GetEntities(parent));
        }

        private void OutputProcScript(XmlElement parent, XmlElement[] entities)
        {
            XmlElement sqlElement = (XmlElement)parent.SelectSingleNode(DefConstants.SqlElement);
            if (sqlElement == null)
            {
                SevereError("Missing <{0}> element", DefConstants.SqlElement);
                return;
            }
            string role = sqlElement.GetAttribute(DefConstants.SqlRoleAttrib);
            if (string.IsNullOrEmpty(role))
            {
                SevereError("Missing <{0}> [{1}] attribute", DefConstants.SqlElement, DefConstants.SqlRoleAttrib);
                return;
            }
            WriteLine("-- Generated by {0} at {1}", this.GetType().FullName, DateTime.Now);
            WriteLine();
            foreach (XmlElement entity in GetEntities(parent))
            {
                if (OutputTableProcScript(entity, role))
                    return;
            }
        }

        private bool OutputTableProcScript(XmlElement entity, string role)
        {
            string classname;
            if (GetClassname(entity, out classname))
                return true;
            WriteLine("-- {0} Stored Procedures", classname);
            WriteLine();
            if (OutputCRUDDrops(classname))
                return true;
            if (OutputCreateProc(entity, "Get" + classname, classname, role, OutputGetProcBody))
                return true;
            if (OutputCreateProc(entity, "Insert" + classname, classname, role, OutputInsertProcBody))
                return true;
            if (OutputCreateProc(entity, "Update" + classname, classname, role, OutputUpdateProcBody))
                return true;
            if (OutputCreateProc(entity, "Delete" + classname, classname, role, OutputDeleteProcBody))
                return true;
            return false;
        }

        private bool OutputCRUDDrops(string classname)
        {
            WriteLine("DROP PROC dbo.Get{0}", classname);
            WriteLine("DROP PROC dbo.Insert{0}", classname);
            WriteLine("DROP PROC dbo.Update{0}", classname);
            WriteLine("DROP PROC dbo.Delete{0}", classname);
            WriteLine();
            WriteLine("GO");
            WriteLine();
            return false;
        }

        private bool OutputGetProcBody(XmlElement entity, string classname)
        {
            string idtype;
            if (GetIdtype(entity, out idtype))
                return true;
            WriteLine("  @{0} int", idtype);
            WriteLine("AS");
            WriteLine();
            WriteLine("SELECT *");
            WriteLine("FROM   {0}", classname);
            WriteLine("WHERE  {0}=@{0}", idtype);
            return false;
        }

        private bool OutputInsertProcBody(XmlElement entity, string classname)
        {
            string idtype;
            if (GetIdtype(entity, out idtype))
                return true;
            if (OutputAllFieldsAsArgs(entity, idtype, " out"))
                return true;
            string linePrefix = string.Format("INSERT {0} (", classname);
            foreach (XmlElement field in GetFields(entity))
            {
                string name;
                if (GetFieldName(field, out name))
                    return true;
                WriteLine("{0}{1}", linePrefix, name);
                linePrefix = "    ,";
            }
            WriteLine("    ,CreateDate, ModifyDate)");
            linePrefix = "VALUES(";
            foreach (XmlElement field in GetFields(entity))
            {
                string name;
                if (GetFieldName(field, out name))
                    return true;
                WriteLine("{0}@{1}", linePrefix, name);
                linePrefix = "    ,";
            }
            WriteLine("    ,GETDATE(),GETDATE())");
            WriteLine();
            WriteLine("SELECT @{0}=@@IDENTITY", idtype);
            return false;
        }

        private bool OutputUpdateProcBody(XmlElement entity, string classname)
        {
            string idtype;
            if (GetIdtype(entity, out idtype))
                return true;
            if (OutputAllFieldsAsArgs(entity, idtype, ""))
                return true;
            WriteLine("UPDATE {0}", classname);
            string linePrefix = "SET ";
            foreach (XmlElement field in GetFields(entity))
            {
                string name;
                if (GetFieldName(field, out name))
                    return true;
                WriteLine("    {0}{1}=@{1}", linePrefix, name);
                linePrefix = ",";
            }
            WriteLine("    ,ModifyDate=GETDATE()");
            WriteLine("WHERE {0}=@{0}", idtype);
            return false;
        }

        private bool OutputAllFieldsAsArgs(XmlElement entity, string idtype, string idextra)
        {
            WriteLine("    @{0} int{1}", idtype, idextra);
            foreach (XmlElement field in GetFields(entity))
            {
                string name;
                if (GetFieldName(field, out name))
                    return true;
                string sqltype;
                bool isid;
                if (GetSqltype(field, out sqltype, out isid))
                    return true;
                WriteLine("    ,@{0} {1}", name, sqltype);
            }
            WriteLine("AS");
            WriteLine();
            return false;
        }

        private bool OutputDeleteProcBody(XmlElement entity, string classname)
        {
            string idtype;
            if (GetIdtype(entity, out idtype))
                return true;
            WriteLine("  @{0} int", idtype);
            WriteLine("AS");
            WriteLine();
            WriteLine("DELETE {0}", classname);
            WriteLine("WHERE  {0}=@{0}", idtype);
            return false;
        }

        private delegate bool ProcBodyWriter(XmlElement entity, string classname);

        private bool OutputCreateProc(XmlElement entity, string procname, string classname,
            string role, ProcBodyWriter bodyWriter)
        {
            WriteLine("---------------");
            WriteLine();
            WriteLine("CREATE PROC dbo.{0}", procname);
            if (bodyWriter(entity, classname))
                return true;
            WriteLine();
            WriteLine("GO");
            WriteLine();
            WriteLine("GRANT EXECUTE ON dbo.{0} TO {1}", procname, role);
            WriteLine();
            WriteLine("GO");
            WriteLine();
            return false;
        }
    }
}