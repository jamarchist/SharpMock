using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Windows.Forms;
using SharpMock.Core.Interception.Registration;
using SharpMock.Core.PostCompiler.Replacement;
using SharpMock.Core.Utility;

namespace SharpMock.VisualStudio.Debugging
{
    public partial class RegistryForm : Form
    {
        private readonly ReplacementRegistry registry;

        public RegistryForm(ReplacementRegistry registry)
        {
            this.registry = registry;

            InitializeComponent();
            PopulateListBox();
        }

        private void PopulateListBox()
        {
            replaceableReferenceTypes.Items.Add(ReplaceableReferenceTypes.FieldAccessor);
            replaceableReferenceTypes.Items.Add(ReplaceableReferenceTypes.FieldAssignment);
            replaceableReferenceTypes.Items.Add(ReplaceableReferenceTypes.Method);

            replaceableReferenceTypes.SelectedIndexChanged += replaceableReferenceTypes_SelectedIndexChanged;
        }

        private void replaceableReferenceTypes_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            var reference = (string)replaceableReferenceTypes.SelectedItem;
            var references = registry.GetRegisteredReferences(reference);
            var table = GetTableRepresentation(references);

            registeredReferences.DataSource = table;
        }

        private void WriteDebug(string message, params string[] arguments)
        {
            debugDisplay.Items.Add(String.Format(message, arguments));
        }

        private DataTable GetTableRepresentation(List<IReplaceableReference> references)
        {
            if (references.Count == 0)
            {
                WriteDebug("No references for this reference type");
                return new DataTable();
            }

            var firstReferenceType = references[0].GetType();
            var table = new DataTable();
            var props = new Dictionary<string, PropertyInfo>();
            foreach (var property in firstReferenceType.GetProperties())
            {
                WriteDebug("Creating column for property: {0}", property.Name);
                table.Columns.Add(new DataColumn(property.Name, typeof(string)));
                props.Add(property.Name, property);
            }

            foreach (var reference in references)
            {
                var row = table.NewRow();
                foreach (DataColumn column in table.Columns)
                {
                    row[column.ColumnName] = TraceHelper.GetDebuggerDisplay(props[column.ColumnName].GetValue(reference, null));
                }

                table.Rows.Add(row);
                WriteDebug("Adding row");
            }

            return table;
        }
    }
}
