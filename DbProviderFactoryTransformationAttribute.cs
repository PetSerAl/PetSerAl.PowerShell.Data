using System;
using System.Data;
using System.Data.Common;
using System.Management.Automation;
namespace PetSerAl.PowerShell.Data {
    internal sealed class DbProviderFactoryTransformationAttribute : ArgumentTransformationAttribute {
        public override object Transform(EngineIntrinsics engineIntrinsics, object inputData) {
            object obj = Utility.UnwrapPSObject(inputData);
            string invariantName = obj as string;
            if(invariantName!=null) {
                inputData=DbProviderFactories.GetFactory(invariantName);
            } else {
                DataRow providerRow = obj as DataRow;
                if(providerRow!=null) {
                    inputData=DbProviderFactories.GetFactory(providerRow);
                }
            }
            return inputData;
        }
    }
}
