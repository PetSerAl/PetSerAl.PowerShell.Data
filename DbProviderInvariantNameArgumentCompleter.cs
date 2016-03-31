#if PSv5Plus
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Language;
namespace PetSerAl.PowerShell.Data {
    internal class DbProviderInvariantNameArgumentCompleter : IArgumentCompleter {
        public DbProviderInvariantNameArgumentCompleter() { }
        IEnumerable<CompletionResult> IArgumentCompleter.CompleteArgument(string commandName, string parameterName, string wordToComplete, CommandAst commandAst, IDictionary fakeBoundParameters) {
            WildcardPattern wp = new WildcardPattern(wordToComplete+"*", WildcardOptions.CultureInvariant|WildcardOptions.IgnoreCase);
            return DbProviderFactories.GetFactoryClasses().Rows.Cast<DataRow>().Select(providerRow => {
                string completionText = (string)providerRow["InvariantName"];
                string listItemText = providerRow["Name"] as string;
                if(string.IsNullOrEmpty(listItemText)) {
                    listItemText=completionText;
                }
                string toolTip = providerRow["Description"] as string;
                if(string.IsNullOrEmpty(toolTip)) {
                    toolTip=completionText;
                }
                return new CompletionResult(completionText, listItemText, CompletionResultType.ParameterValue, toolTip);
            }).Where(cr => wp.IsMatch(cr.CompletionText)||wp.IsMatch(cr.ListItemText)).OrderBy(cr => cr.CompletionText);
        }
    }
}
#endif
