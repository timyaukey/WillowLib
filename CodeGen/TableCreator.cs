﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using Willowsoft.WillowLib.Data.Misc;

namespace Willowsoft.WillowLib.CodeGen
{
    public class TableCreator : CreatorBase
    {
        public TableCreator(TextWriter output, ErrorList errors)
            : base(output, errors)
        {
        }

        public void OutputTableScript(XmlElement parent)
        {
            OutputTableScript(parent, GetEntities(parent));
        }

        private void OutputTableScript(XmlElement parent, XmlElement[] entities)
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
            if (OutputDrops(entities, role))
                return;
            if (OutputCreates(entities, role))
                return;
        }

        private bool OutputDrops(XmlElement[] entities, string role)
        {
            WriteLine("-- Drop tables and roles");
            WriteLine();
            for (int i = entities.Length - 1; i >= 0; i--)
            {
                string name = entities[i].GetAttribute(DefConstants.EntityClassnameAttrib);
                WriteLine("DROP TABLE dbo.{0}", name);
            }
            WriteLine();
            WriteLine("DROP ROLE {0}", role);
            WriteLine();
            WriteLine("GO");
            WriteLine();
            return false;
        }

        private bool OutputCreates(XmlElement[] entities, string role)
        {
            WriteLine("-- Create tables and roles");
            WriteLine();
            WriteLine("CREATE ROLE {0}", role);
            WriteLine();
            foreach(XmlElement entity in entities)
            {
                if (OutputCreateTable(entity))
                    return true;
            }
            WriteLine("GO");
            WriteLine();
            return false;
        }

        private bool OutputCreateTable(XmlElement entity)
        {
            string classname;
            if (GetClassname(entity, out classname))
                return true;
            WriteLine("CREATE TABLE dbo.{0}", classname);
            WriteLine("(");
            if (OutputTableFields(entity))
                return true;
            if (OutputTableConstraints(entity))
                return true;
            WriteLine(")");
            WriteLine();
            if (OutputIndexes(entity, classname))
                return true;
            return false;
        }

        private bool OutputTableFields(XmlElement entity)
        {
            string idtype;
            if (GetIdtype(entity, out idtype))
                return true;
            WriteLine("    {0} int IDENTITY(1,1) NOT NULL", idtype);
            foreach (XmlElement field in GetFields(entity))
            {
                string fieldName;
                if (GetFieldName(field, out fieldName))
                    return true;
                string sqltype;
                bool isid;
                if (GetSqltype(field, out sqltype, out isid))
                    return true;
                bool nullableFlag;
                string nullable;
                if (GetNullable(field, out nullableFlag))
                    return true;
                if (nullableFlag)
                    nullable = "NULL";
                else
                    nullable = "NOT NULL";
                WriteLine("    ,{0} {1} {2}", fieldName, sqltype, nullable);
            }
            WriteLine("    ,CreateDate smalldatetime NOT NULL");
            WriteLine("    ,ModifyDate smalldatetime NOT NULL");
            return false;
        }

        private bool OutputTableConstraints(XmlElement entity)
        {
            XmlNodeList constraints = entity.SelectNodes(DefConstants.SqlElement + "/" +
                DefConstants.SqlConstraintElement);
            foreach (XmlNode constraintNode in constraints)
            {
                XmlElement constraint = (XmlElement)constraintNode;
                string constraintName = constraint.GetAttribute(DefConstants.SqlNameAttrib);
                if (string.IsNullOrEmpty(constraintName))
                    return SevereError("Missing [{0}] attribute on <{1}> element",
                        DefConstants.SqlNameAttrib, DefConstants.SqlConstraintElement);
                string constraintText = constraint.InnerText.Trim();
                WriteLine("    ,CONSTRAINT {0} {1}", constraintName, constraintText);
            }
            return false;
        }

        private bool OutputIndexes(XmlElement entity, string classname)
        {
            XmlNodeList indexes = entity.SelectNodes(DefConstants.SqlElement + "/" +
                DefConstants.SqlIndexElement);
            foreach (XmlNode indexNode in indexes)
            {
                XmlElement index = (XmlElement)indexNode;
                string indexName = index.GetAttribute(DefConstants.SqlNameAttrib);
                if (string.IsNullOrEmpty(indexName))
                    return SevereError("Missing [{0}] attribute on <{1}> element",
                        DefConstants.SqlNameAttrib, DefConstants.SqlIndexElement);
                string unique = index.GetAttribute(DefConstants.SqlIndexUniqueAttrib);
                string uniqueSql = string.Empty;
                if (unique == "true")
                    uniqueSql = " UNIQUE";
                string indexText = index.InnerText.Trim();
                WriteLine("CREATE{3} INDEX {0} ON dbo.{1}({2})", indexName, classname, indexText, uniqueSql);
            }
            WriteLine();
            return false;
        }

        public void OutputConversionScript(XmlElement parent)
        {
            OutputConversionScript(parent, GetEntities(parent));
        }

        private void OutputConversionScript(XmlElement parent, XmlElement[] entities)
        {
            WriteLine("-- Generated by {0} at {1}", this.GetType().FullName, DateTime.Now);
            WriteLine("-- Transaction is rolled back at end");
            WriteLine("-- Remember to recreate any typed dataset for this table");
            WriteLine("begin tran");
            if (OutputAlters(entities))
                return;
            if (OutputUpdates(entities))
                return;
            WriteLine();
            WriteLine("rollback tran");
            WriteLine("--commit tran");
        }

        private bool OutputAlters(XmlElement[] entities)
        {
            WriteLine();
            WriteLine("-- Add or alter columns");
            WriteLine();
            foreach (XmlElement entity in entities)
            {
                if (OutputAlterTable(entity))
                    return true;
            }
            WriteLine();
            return false;
        }

        private bool OutputAlterTable(XmlElement entity)
        {
            string className;
            int alterColumnCount = 0;
            if (GetClassname(entity, out className))
                return true;
            foreach (XmlElement field in GetFields(entity))
            {
                string fieldName;
                string revision;
                if (GetFieldName(field, out fieldName))
                    return true;
                GetRevision(field, out revision);
                if (!string.IsNullOrEmpty(revision))
                {
                    string sqltype;
                    bool isid;
                    string defaultConstraint;
                    string defaultConstraintName;
                    bool nullableFlag;
                    string nullable;

                    alterColumnCount++;
                    if (alterColumnCount == 1)
                    {
                        WriteLine();
                        WriteLine();
                        WriteLine("-- ALTER TABLE " + className + " ----------------------------------------");
                    }
                    
                    if (GetSqltype(field, out sqltype, out isid))
                        return true;
                    if (GetNullable(field, out nullableFlag))
                        return true;

                    defaultConstraintName = className + "_" + fieldName + "_DF";
                    if (nullableFlag)
                        nullable = "NULL";
                    else
                        nullable = "NOT NULL";
                    if (sqltype.EndsWith("int") || sqltype.EndsWith("money"))
                        defaultConstraint = "0";
                    else if (sqltype.EndsWith("datetime"))
                        defaultConstraint = "getdate()";
                    else if (sqltype.StartsWith("varchar"))
                        defaultConstraint = "''";
                    else
                        defaultConstraint = "";
                    WriteLine();
                    WriteLine("if exists(select * from sys.columns");
                    WriteLine("            where Name = N'" + fieldName + "' and Object_ID = Object_ID(N'" + className + "'))");
                    WriteLine("begin");
                    WriteLine("  print 'alter column {0}.{1}'", className, fieldName);
                    WriteLine("  alter table {0} alter column {1} {2} {3}",
                        className, fieldName, sqltype, nullable);
                    WriteLine("end");
                    WriteLine("else");
                    WriteLine("begin");
                    WriteLine("  print 'add column {0}.{1}'", className, fieldName);
                    WriteLine("  alter table {0} add {1} {2} {3} constraint {4} default {5}", 
                        className, fieldName, sqltype, nullable, defaultConstraintName, defaultConstraint);
                    WriteLine("  alter table {0} drop constraint {1}", className, defaultConstraintName);
                    WriteLine("end");
                    WriteLine("GO");
                }
            }
            return false;
        }

        private bool OutputUpdates(XmlElement[] entities)
        {
            return false;
        }
    }
}
