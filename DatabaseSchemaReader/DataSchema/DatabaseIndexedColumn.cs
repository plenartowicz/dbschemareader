using System;

namespace DatabaseSchemaReader.DataSchema
{
	/// <summary>
	/// Describes indexed column of index
	/// </summary>
	[Serializable]
	public partial class DatabaseIndexedColumn : DatabaseColumn
	{
		/// <summary>
		/// Gets or sets the order of sorting or null if default/unknown.
		/// </summary>
		public IndexedColumnOrder? ColumnOrder { get; set; }

		/// <summary>
		/// Creates an empty instance of the object
		/// </summary>
		public DatabaseIndexedColumn()
		{
		}

		/// <summary>
		/// Creates and initializes the instance of the object bases on given <paramref name="databaseColumn"/>
		/// </summary>
		/// <param name="databaseColumn"></param>
		public DatabaseIndexedColumn(DatabaseColumn databaseColumn)
		{
			Name = databaseColumn.Name;
			SchemaOwner = databaseColumn.SchemaOwner;
			Tag = databaseColumn.Tag;
			DbDataType = databaseColumn.DbDataType;
			Length = databaseColumn.Length;
			Nullable = databaseColumn.Nullable;
			Ordinal = databaseColumn.Ordinal;
			DefaultValue = databaseColumn.DefaultValue;
			Precision = databaseColumn.Precision;
			Scale = databaseColumn.Scale;
			DateTimePrecision = databaseColumn.DateTimePrecision;
			TableName = databaseColumn.TableName;
			IdentityDefinition = databaseColumn.IdentityDefinition;
			ComputedDefinition = databaseColumn.ComputedDefinition;
			Description = databaseColumn.Description;
			IsForeignKey = databaseColumn.IsForeignKey;
			IsAutoNumber = databaseColumn.IsAutoNumber;
			IsIndexed = databaseColumn.IsIndexed;
			IsPrimaryKey = databaseColumn.IsPrimaryKey;
			IsUniqueKey = databaseColumn.IsUniqueKey;
			NetName = databaseColumn.NetName;
			Table = databaseColumn.Table;
			DatabaseSchema = databaseColumn.DatabaseSchema;
			DataType = databaseColumn.DataType;
			ForeignKeyTable = databaseColumn.ForeignKeyTable;
			ForeignKeyTableNames.AddRange(databaseColumn.ForeignKeyTableNames);
		}

		/// <summary>
		/// Returns name with order name (if given)
		/// </summary>
		/// <returns></returns>
		public string GetNameWithOrder()
		{
			return Name + (ColumnOrder.HasValue ? " " + ColumnOrder.Value : "");
		}
	}
}