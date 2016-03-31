using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Management.Automation;
using System.Reflection;
namespace PetSerAl.PowerShell.Data {
    internal abstract class Property {
        private static Collection<Attribute> parameterAttribute;
        public static IEnumerable<Property> GetSettableProperties(object obj, PropertiesSource propertiesSource) {
            switch(propertiesSource) {
                case PropertiesSource.PowerShell:
                    return PowerShellProperty.GetSettableProperties(obj);
                case PropertiesSource.Reflection:
                    return ReflectionProperty.GetSettableProperties(obj);
                case PropertiesSource.TypeDescriptor:
                    return TypeDescriptorProperty.GetSettableProperties(obj);
                default:
                    throw new ArgumentOutOfRangeException(nameof(propertiesSource));
            }
        }
        private RuntimeDefinedParameter dynamicParameter;
        protected Property() { }
        protected abstract string Name { get; }
        protected abstract Type Type { get; }
        public RuntimeDefinedParameter DynamicParameter {
            get {
                if(dynamicParameter==null) {
                    if(parameterAttribute==null) {
                        parameterAttribute=new Collection<Attribute>(Array.AsReadOnly(new Attribute[] { new ParameterAttribute() }));
                    }
                    dynamicParameter=new RuntimeDefinedParameter(Name, Type, parameterAttribute);
                }
                return dynamicParameter;
            }
        }
        public abstract void SetValue();
    }
    internal sealed class PowerShellProperty : Property {
        public static IEnumerable<Property> GetSettableProperties(object obj) => PSObject.AsPSObject(obj).Properties.Where(property => property.IsSettable).Select(property => (Property)new PowerShellProperty(property));
        private readonly PSPropertyInfo property;
        private PowerShellProperty(PSPropertyInfo property) {
            this.property=property;
        }
        protected override string Name => property.Name;
        protected override Type Type => new PSTypeName(property.TypeNameOfValue).Type;
        public override void SetValue() => property.Value=DynamicParameter.Value;
    }
    internal sealed class ReflectionProperty : Property {
        public static IEnumerable<Property> GetSettableProperties(object obj) => obj.GetType().GetProperties(BindingFlags.Instance|BindingFlags.Public).Where(property => property.CanWrite).Select(property => (Property)new ReflectionProperty(property, obj));
        private readonly PropertyInfo property;
        private readonly object obj;
        private ReflectionProperty(PropertyInfo property, object obj) {
            this.property=property;
            this.obj=obj;
        }
        protected override string Name => property.Name;
        protected override Type Type => property.PropertyType;
        public override void SetValue() => property.SetValue(obj, DynamicParameter.Value, null);
    }
    internal sealed class TypeDescriptorProperty : Property {
        public static IEnumerable<Property> GetSettableProperties(object obj) => TypeDescriptor.GetProperties(obj).Cast<PropertyDescriptor>().Where(property => !property.IsReadOnly).Select(property => (Property)new TypeDescriptorProperty(property, obj));
        private readonly PropertyDescriptor property;
        private readonly object obj;
        private TypeDescriptorProperty(PropertyDescriptor property, object obj) {
            this.property=property;
            this.obj=obj;
        }
        protected override string Name => property.Name;
        protected override Type Type => property.PropertyType;
        public override void SetValue() => property.SetValue(obj, DynamicParameter.Value);
    }
    public enum PropertiesSource {
        PowerShell,
        Reflection,
        TypeDescriptor
    }
}
