using System;
using System.Collections;
using System.Data.Common;
using System.Management.Automation;
namespace PetSerAl.PowerShell.Data {
    [Cmdlet(VerbsCommon.New, nameof(DbCommand)), OutputType(typeof(DbCommand))]
    public sealed class NewDbCommandCmdlet : PSCmdlet {
        public NewDbCommandCmdlet() { }
        [Parameter(Mandatory = true, Position = 1)]
        public DbConnection Connection { private get; set; }
        [Parameter(Position = 2), Alias("Query"), ValidateNotNull]
        public string CommandText { private get; set; }
        [Parameter(Position = 3), Alias("Params"), ValidateNotNull]
        public IDictionary Parameters { private get; set; }
        protected override void BeginProcessing() {
            DbCommand command = Connection.CreateCommand();
            if(CommandText!=null) {
                command.CommandText=CommandText;
            }
            Utility.AddParameters(command, Parameters);
            WriteObject(command);
        }
    }
}
