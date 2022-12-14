using DatabaseSchemaReader.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DatabaseSchemaReader.DataSchema.SqlServer
{
	/// <summary>
	/// Extension class for Sql Server Index description
	/// </summary>
	[Serializable]
	public partial class DatabaseSqlServerIndex : DatabaseIndex
	{
		#region Fields
		//backing fields
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly List<DatabaseColumn> _includedColumns;
		#endregion


		/// <summary>
		/// Initializes a new instance of the <see cref="DatabaseIndex"/> class.
		/// </summary>
		public DatabaseSqlServerIndex()
		{
			_includedColumns = new List<DatabaseColumn>();
		}

		/// <summary>
		/// Gets the included columns
		/// </summary>
		public List<DatabaseColumn> IncludedColumns => _includedColumns;

		/// <summary>
		/// Gets or sets the fill factor
		/// </summary>
		public int FillFactor { get; set; }

		/// <inheritdoc cref="DatabaseIndex.GetAllColumns"/>
		protected internal override IEnumerable<DatabaseColumn> GetAllColumns()
		{
			return base.GetAllColumns().Concat(IncludedColumns);
		}

		/// <inheritdoc cref="DatabaseIndex.Equals(DatabaseSchemaReader.DataSchema.DatabaseIndex)"/>
		public override bool Equals(DatabaseIndex other)
		{
			if (ReferenceEquals(this, other))
			{
				return true;
			}

			if (!(other is DatabaseSqlServerIndex sqlServerIndex))
			{
				return false;
			}

			if (!base.Equals(sqlServerIndex))
			{
				return false;
			}

			if (FillFactor != sqlServerIndex.FillFactor)
			{
				return false;
			}

			var columns1 = IncludedColumns.OrderBy(x => x.Ordinal).ThenBy(x => x.Name).Select(x => x.Name);
			var columns2 = sqlServerIndex.IncludedColumns.OrderBy(x => x.Ordinal).ThenBy(x => x.Name).Select(x => x.Name);
			return columns1.AreEqual(columns2);
		}
	}
}