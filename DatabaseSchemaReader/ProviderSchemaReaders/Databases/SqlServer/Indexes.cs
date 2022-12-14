﻿using DatabaseSchemaReader.DataSchema;
using DatabaseSchemaReader.DataSchema.SqlServer;
using DatabaseSchemaReader.ProviderSchemaReaders.ConnectionContext;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace DatabaseSchemaReader.ProviderSchemaReaders.Databases.SqlServer
{
	class Indexes : SqlExecuter<DatabaseIndex>
	{
		private readonly string _tableName;

		public Indexes(int? commandTimeout, string owner, string tableName) : base(commandTimeout, owner)
		{
			_tableName = tableName;
			Owner = owner;
			Sql = @" SELECT 
	 SchemaName = SCHEMA_NAME(t.schema_id),
	 TableName = t.name,
	 IndexName = ind.name,
	 ColumnName = col.name,
	 INDEX_TYPE = ind.type_desc,
	 IsPrimary = is_primary_key,
	 IsUnique = is_unique,
	 Ordinal = ic.key_ordinal,
	 Filter = ind.filter_definition,
	 FILL_FACTOR = ind.fill_factor,
	 IsIncludedColumn = ic.is_included_column,
	 IsDescending = ic.is_descending_key
FROM 
	 sys.indexes ind 
INNER JOIN 
	 sys.index_columns ic ON  ind.object_id = ic.object_id and ind.index_id = ic.index_id 
INNER JOIN 
	 sys.columns col ON ic.object_id = col.object_id and ic.column_id = col.column_id 
INNER JOIN 
	 sys.tables t ON ind.object_id = t.object_id 
WHERE 
	(t.name = @TableName OR @TableName IS NULL) AND 
	(SCHEMA_NAME(t.schema_id) = @schemaOwner OR @schemaOwner IS NULL) AND 
	 t.is_ms_shipped = 0 
ORDER BY 
	 t.name, ind.name, ic.is_included_column, ic.key_ordinal, col.name";

		}

		protected override void AddParameters(DbCommand command)
		{
			AddDbParameter(command, "schemaOwner", Owner);
			AddDbParameter(command, "TableName", _tableName);
		}

		protected override void Mapper(IDataRecord record)
		{
			var schema = record.GetString("SchemaName");
			var tableName = record.GetString("TableName");
			var name = record.GetString("IndexName");
			var index = (DatabaseSqlServerIndex)Result.FirstOrDefault(f => f.Name == name && f.SchemaOwner == schema && f.TableName.Equals(tableName, StringComparison.OrdinalIgnoreCase));
			if (index == null)
			{
				index = new DatabaseSqlServerIndex
				{
					SchemaOwner = schema,
					TableName = tableName,
					Name = name,
					IndexType = record.GetString("INDEX_TYPE"),
					IsUnique = record.GetBoolean("IsUnique"),
					Filter = record.GetString("Filter"),
					FillFactor = record.GetInt("FILL_FACTOR"),
				};
				if (record.GetBoolean("IsPrimary"))
				{
					//by default SqlServer pks have clustered indexes. If they are not, we need to record it.
					index.IndexType = string.Equals("NONCLUSTERED", index.IndexType, StringComparison.OrdinalIgnoreCase) ?
						"PRIMARY NONCLUSTERED" :
						"PRIMARY";
				}
				Result.Add(index);
			}
			var colName = record.GetString("ColumnName");
			if (string.IsNullOrEmpty(colName)) return;



			var isIncludedColumn = record.GetBoolean("IsIncludedColumn");
			if (isIncludedColumn)
			{
				var col = new DatabaseColumn
				{
					Name = colName,
					Ordinal = record.GetInt("Ordinal")
				};
				index.IncludedColumns.Add(col);
			}
			else
			{
				var isDescending = record.GetBoolean("IsDescending");
				var col = new DatabaseIndexedColumn
				{
					Name = colName,
					Ordinal = record.GetInt("Ordinal"),
					ColumnOrder = isDescending ? IndexedColumnOrder.DESC : IndexedColumnOrder.ASC,
				};

				index.Columns.Add(col);
			}
		}

		public IList<DatabaseIndex> Execute(IConnectionAdapter connectionAdapter)
		{
			ExecuteDbReader(connectionAdapter);
			return Result;
		}
	}
}
