using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Willowsoft.WillowLib.CodeGen
{
    public static class DefConstants
    {
        public const string EntityElement = "entity";
        public const string EntityNamespaceAttrib = "namespace";
        public const string EntityClassnameAttrib = "classname";
        public const string EntityIdtypeAttrib = "idtype";
        public const string FieldsElement = "fields";
        public const string FieldElement = "field";
        public const string FieldNameAttrib = "name";
        public const string FieldTypeAttrib = "type";
        public const string FieldInitAttrib = "init";
        public const string FieldIsIdAttrib = "isid";
        public const string FieldSqltypeAttrib = "sqltype";
        public const string FieldNullableAttrib = "nullable";
        public const string SqlElement = "sql";
        public const string SqlRoleAttrib = "role";
        public const string SqlNameAttrib = "name";
        public const string SqlConstraintElement = "constraint";
        public const string SqlIndexElement = "index";
        public const string SqlIndexUniqueAttrib = "unique";
        public const string RepositoryElement = "sqlrepository";
        public const string RepositoryNamespaceAttrib = "namespace";
        public const string RepositoryDatasetNameAttrib = "datasetname";
    }
}
