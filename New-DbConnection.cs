using System;
using System.Data.Common;
using System.Linq;
using System.Management.Automation;
namespace PetSerAl.PowerShell.Data {
    [Cmdlet(VerbsCommon.New, nameof(DbConnection)), OutputType(typeof(DbConnection))]
    public sealed class NewDbConnectionCmdlet : PSCmdlet, IDynamicParameters {
#if !PSv4Plus
        private static string[] commonParameters;
#endif
        private static string[] excludeParameters;
#if !PSv4Plus
        private static string[] CommonParameters => commonParameters??(commonParameters=new[] {
            "Debug",
            "ErrorAction",
            "ErrorVariable",
            "OutBuffer",
            "OutVariable",
            "Verbose",
            "WarningAction",
            "WarningVariable"
        });
#endif
        private static string[] ExcludeParameters => excludeParameters??(excludeParameters=new[] {
            nameof(DbConnectionStringBuilder.BrowsableConnectionString),
            nameof(DbConnectionStringBuilder.ConnectionString),
            nameof(ConnectionString),
            nameof(ConnectionStringPropertiesSource),
            nameof(DbProviderFactory)
        });
        private DbConnectionStringBuilder connectionStringBuilder;
        private Property[] connectionStringProperties;
        public NewDbConnectionCmdlet() { }
        [Parameter(Mandatory = true, Position = 1), DbProviderFactoryTransformation]
#if PSv5Plus
        [ArgumentCompleter(typeof(DbProviderInvariantNameArgumentCompleter))]
#endif
        public DbProviderFactory DbProviderFactory { private get; set; }
        [Parameter(Position = 2), ValidateNotNull]
        public string ConnectionString { private get; set; }
#if PSv4Plus
        [Parameter(DontShow = true)]
#else
        [Parameter]
#endif
        [ValidateEnum(typeof(PropertiesSource))]
        public PropertiesSource ConnectionStringPropertiesSource { private get; set; } = PropertiesSource.TypeDescriptor;
        public object GetDynamicParameters() => DbProviderFactory==null ? null : GetConnectionStringDynamicParameters();
        private RuntimeDefinedParameterDictionary GetConnectionStringDynamicParameters() {
            connectionStringBuilder=DbProviderFactory.CreateConnectionStringBuilder();
            if(ConnectionString!=null) {
                try {
                    connectionStringBuilder.ConnectionString=ConnectionString;
                } catch (ArgumentException) {
                    connectionStringBuilder=null;
                    return null;
                }
            }
            connectionStringProperties=Property.GetSettableProperties(connectionStringBuilder, ConnectionStringPropertiesSource).ToArray();
            RuntimeDefinedParameterDictionary dynamicParameters = new RuntimeDefinedParameterDictionary();
            foreach(Property property in connectionStringProperties) {
                dynamicParameters.Add(property.DynamicParameter.Name, property.DynamicParameter);
            }
            foreach(string parameter in ExcludeParameters) {
                dynamicParameters.Remove(parameter);
            }
            foreach(string parameter in CommonParameters) {
                dynamicParameters.Remove(parameter);
            }
            return dynamicParameters;
        }
        protected override void BeginProcessing() {
            if(connectionStringProperties!=null) {
                bool connectionStringPropertySet = false;
                foreach(Property property in connectionStringProperties) {
                    if(property.DynamicParameter.IsSet) {
                        property.SetValue();
                        connectionStringPropertySet=true;
                    }
                }
                connectionStringProperties=null;
                if(connectionStringPropertySet) {
                    ConnectionString=connectionStringBuilder.ConnectionString;
                }
                connectionStringBuilder=null;
            }
            DbConnection connection = DbProviderFactory.CreateConnection();
            if(ConnectionString!=null) {
                connection.ConnectionString=ConnectionString;
            }
            WriteObject(connection);
        }
    }
}
