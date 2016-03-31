using System;
using System.Data;
using System.Data.Common;
using System.Management.Automation;
namespace PetSerAl.PowerShell.Data {
    [Cmdlet(VerbsCommon.Get, nameof(DbProviderFactory)), OutputType(typeof(DbProviderFactory))]
    public sealed class GetDbProviderFactoryCmdlet : PSCmdlet {
        public GetDbProviderFactoryCmdlet() { }
        [Parameter(Mandatory = true, ParameterSetName = nameof(InvariantName), Position = 1)]
#if PSv5Plus
        [ArgumentCompleter(typeof(DbProviderInvariantNameArgumentCompleter))]
#endif
        public string InvariantName { private get; set; }
        [Parameter(Mandatory = true, ParameterSetName = nameof(ProviderRow), Position = 1)]
        public DataRow ProviderRow { private get; set; }
        protected override void BeginProcessing() {
            DbProviderFactory result;
            switch(ParameterSetName) {
                case nameof(InvariantName):
                    result=DbProviderFactories.GetFactory(InvariantName);
                    break;
                case nameof(ProviderRow):
                    result=DbProviderFactories.GetFactory(ProviderRow);
                    break;
                default:
                    throw new Exception("Invalid ParameterSetName.");
            }
            WriteObject(result);
        }
    }
}
