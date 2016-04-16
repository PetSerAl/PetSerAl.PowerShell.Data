using System;
using System.Collections;
using System.Data;
using System.Data.Common;
using System.Management.Automation;
namespace PetSerAl.PowerShell.Data {
    [Cmdlet(VerbsLifecycle.Invoke, nameof(DbCommand)), OutputType(typeof(int), typeof(object), typeof(PSCustomObject), typeof(DataRow), typeof(DataTable))]
    public sealed class InvokeDbCommandCmdlet : PSCmdlet {
        public InvokeDbCommandCmdlet() { }
        [Parameter(Mandatory = true, ParameterSetName = nameof(Command), Position = 1)]
        public DbCommand Command { private get; set; }
        [Parameter(Mandatory = true, ParameterSetName = nameof(Connection), Position = 1)]
        public DbConnection Connection { private get; set; }
        [Parameter(Mandatory = true, ParameterSetName = nameof(Connection), Position = 2), Alias("Query")]
        public string CommandText { private get; set; }
        [Parameter(Position = 3), Alias("Params"), ValidateNotNull]
        public IDictionary Parameters { private get; set; }
        [Parameter, Alias("As"), ValidateEnum(typeof(CommandReturnType))]
        public CommandReturnType ReturnAs { private get; set; } = CommandReturnType.CustomObject;
        protected override void BeginProcessing() {
            if(Command==null) {
                using(Command=Connection.CreateCommand()) {
                    Command.CommandText=CommandText;
                    Utility.AddParameters(Command, Parameters);
                    ExecuteCommand();
                }
            } else {
                Utility.AddParameters(Command, Parameters);
                Connection=Command.Connection;
                ExecuteCommand();
            }
        }
        private void ExecuteCommand() {
            if(Connection!=null&&Connection.State==ConnectionState.Closed) {
                Connection.Open();
            }
            switch(ReturnAs) {
                case CommandReturnType.None:
                    Command.ExecuteNonQuery();
                    break;
                case CommandReturnType.AffectedRows:
                    WriteObject(Command.ExecuteNonQuery());
                    break;
                case CommandReturnType.Scalar:
                    WriteObject(Command.ExecuteScalar());
                    break;
                case CommandReturnType.Values:
                    WriteValues();
                    break;
                case CommandReturnType.CustomObject:
                    WriteCustomObject();
                    break;
                case CommandReturnType.DataRow:
                    WriteDataTable(true);
                    break;
                case CommandReturnType.DataTable:
                    WriteDataTable(false);
                    break;
                default:
                    throw new Exception("Invalid ReturnAs.");
            }
        }
        private void WriteValues() {
            using(DbDataReader dataReader = Command.ExecuteReader()) {
                do {
                    while(dataReader.Read()) {
                        for(int i = 0; i<dataReader.FieldCount; ++i) {
                            WriteObject(dataReader.GetValue(i));
                        }
                    }
                } while(dataReader.NextResult());
            }
        }
        private void WriteCustomObject() {
            using(DbDataReader dataReader = Command.ExecuteReader()) {
                do {
                    string[] names = new string[dataReader.FieldCount];
                    for(int i = 0; i<names.Length; ++i) {
                        names[i]=dataReader.GetName(i);
                    }
                    Utility.MakeNamesDistinct(names);
                    while(dataReader.Read()) {
                        PSObject psObject = new PSObject();
                        for(int i = 0; i<dataReader.FieldCount; ++i) {
                            psObject.Properties.Add(new PSNoteProperty(names[i], dataReader.GetValue(i)));
                        }
                        WriteObject(psObject);
                    }
                } while(dataReader.NextResult());
            }
        }
        private void WriteDataTable(bool enumerate) {
            using(DbDataReader dataReader = Command.ExecuteReader()) {
                do {
                    DataTable dataTable = new DataTable();
                    dataTable.Load(dataReader);
                    WriteObject(dataTable, enumerate);
                } while(!dataReader.IsClosed);
            }
        }
    }
    public enum CommandReturnType {
        None,
        AffectedRows,
        Scalar,
        Values,
        CustomObject,
        DataRow,
        DataTable
    }
}
