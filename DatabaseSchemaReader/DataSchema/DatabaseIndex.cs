﻿using DatabaseSchemaReader.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DatabaseSchemaReader.DataSchema
{
	/// <summary>
	/// Represents an index in the database.
	/// </summary>
	/// <remarks>
	/// We don't capture if this is a NONCLUSTERED index.
	/// </remarks>
	[Serializable]
	public partial class DatabaseIndex : NamedSchemaObject<DatabaseIndex>
	{
		#region Fields
		//backing fields
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly List<DatabaseColumn> _columns;
		#endregion


		/// <summary>
		/// Initializes a new instance of the <see cref="DatabaseIndex"/> class.
		/// </summary>
		public DatabaseIndex()
		{
			_columns = new List<DatabaseColumn>();
		}


		/// <summary>
		/// Gets or sets the name of the table.
		/// </summary>
		/// <value>
		/// The name of the table.
		/// </value>
		public string TableName { get; set; }

		/// <summary>
		/// Gets or sets the type of the index.
		/// </summary>
		/// <value>
		/// The type of the index.
		/// </value>
		public string IndexType { get; set; }

		/// <summary>
		/// Gets or sets whether this is a unique index
		/// </summary>
		/// <value>
		///   <c>true</c> if this index is unique; otherwise, <c>false</c>.
		/// </value>
		public bool IsUnique { get; set; }

		/// <summary>
		/// Gets the indexed columns.
		/// </summary>
		public List<DatabaseColumn> Columns { get { return _columns; } }

		/// <summary>
		/// Filtered index definition (where ...)
		/// </summary>
		public string Filter { get; set; }

		/// <summary>
		/// Returns the columns of the parent table that are indexed.
		/// </summary>
		/// <param name="parentTable">The parent table.</param>
		/// <returns></returns>
		public IEnumerable<DatabaseColumn> IndexedColumns(DatabaseTable parentTable)
		{
			var query = from indexedColumn in Columns
						join tableColumn in parentTable.Columns
							on indexedColumn.Name equals tableColumn.Name
						orderby indexedColumn.Ordinal
						select tableColumn;
			return query;
		}

		/// <summary>
		/// Determines whether this index is the same as any unique key (including the primary key) for the table
		/// </summary>
		/// <param name="parentTable">The parent table.</param>
		/// <returns>
		///   <c>true</c> if is same as unique key; otherwise, <c>false</c>.
		/// </returns>
		public bool IsUniqueKeyIndex(DatabaseTable parentTable)
		{
			var columnNames = Columns.Select(c => c.Name).ToList();
			//if this the same as the primary key?
			if (parentTable.PrimaryKey != null &&
				parentTable.PrimaryKey.Columns.SequenceEqual(columnNames))
				return true;
			return parentTable.UniqueKeys.Any(
				uniqueKey => uniqueKey.Name == Name ||
					uniqueKey.Columns.SequenceEqual(columnNames));
		}

		/// <summary>
		/// Gets all columns indexed and non-indexed
		/// </summary>
		/// <returns></returns>
		protected internal virtual IEnumerable<DatabaseColumn> GetAllColumns()
		{
			return Columns;
		}

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents this instance.
		/// </summary>
		/// <returns>
		/// A <see cref="System.String"/> that represents this instance.
		/// </returns>
		public override string ToString()
		{
			return Name + " on " + TableName;
		}

		/// <inheritdoc cref="NamedSchemaObject{T}.Equals(T)"/>
		public override bool Equals(DatabaseIndex other)
		{
			if (ReferenceEquals(this, other))
			{
				return true;
			}

			if (ReferenceEquals(null, other))
			{
				return false;
			}

			if (!base.Equals(other))
			{
				return false;
			}

			if (IsUnique != other.IsUnique)
			{
				return false;
			}

			if (!string.Equals(IndexType, other.IndexType, StringComparison.CurrentCultureIgnoreCase))
			{
				return false;
			}

			if (!string.Equals(Filter, other.Filter, StringComparison.CurrentCultureIgnoreCase))
			{
				return false;
			}

			//the two sequences have the same names
			var columnNames1 = Columns.OrderBy(c => c.Ordinal).Select(c => c.ToIndexedColumn()).Select(x => x.GetNameWithOrder());
			var columnNames2 = other.Columns.OrderBy(c => c.Ordinal).Select(c => c.ToIndexedColumn()).Select(x => x.GetNameWithOrder());

			return columnNames1.AreEqual(columnNames2);
		}
	}
}

