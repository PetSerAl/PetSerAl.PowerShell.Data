using System;
using System.Management.Automation;
namespace PetSerAl.PowerShell.Data {
    internal sealed class ValidateEnumAttribute : ValidateArgumentsAttribute {
        private Type type;
        public ValidateEnumAttribute(Type type) {
            this.type=type;
        }
        protected override void Validate(object arguments, EngineIntrinsics engineIntrinsics) {
            if(!Enum.IsDefined(type, arguments)) {
                throw new ArgumentOutOfRangeException(nameof(arguments));
            }
        }
    }
}
