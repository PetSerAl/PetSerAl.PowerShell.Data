using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Management.Automation;
namespace PetSerAl.PowerShell.Data {
    internal static class Utility {
        public static void AddParameters(DbCommand command, IDictionary parameters) {
            if(parameters!=null) {
                IDictionaryEnumerator enumerator = parameters.GetEnumerator();
                try {
                    while(enumerator.MoveNext()) {
                        string name = string.Concat(enumerator.Key);
                        object value = UnwrapPSObject(enumerator.Value);
                        if(command.Parameters.Contains(name)) {
                            command.Parameters[name].Value=value;
                        } else {
                            DbParameter parameter = command.CreateParameter();
                            parameter.ParameterName=name;
                            parameter.Value=value;
                            command.Parameters.Add(parameter);
                        }
                    }
                } finally {
                    (enumerator as IDisposable)?.Dispose();
                }
            }
        }
        public static void MakeNamesDistinct(string[] names) {
            Dictionary<string, bool> usedNames = new Dictionary<string, bool>(names.Length, StringComparer.OrdinalIgnoreCase);
            foreach(string name in names) {
                usedNames[name]=false;
            }
            if(usedNames.Count<names.Length) {
                for(int i = 0; ; ++i) {
                    if(usedNames[names[i]]) {
                        int j = 1;
                        while(usedNames.ContainsKey(names[i]+j)) {
                            ++j;
                        }
                        usedNames[names[i]+=j]=true;
                        if(usedNames.Count==names.Length) {
                            break;
                        }
                    } else {
                        usedNames[names[i]]=true;
                    }
                }
            }
        }
        public static object UnwrapPSObject(object obj) {
            PSObject psObject = obj as PSObject;
            if(psObject!=null) {
                obj=psObject.BaseObject;
            }
            return obj;
        }
    }
}
