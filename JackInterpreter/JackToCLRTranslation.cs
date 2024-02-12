using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static JackParser;

namespace JackInterpreter
{
    internal static class JackToCLRTranslation
    {
        public static string GetFieldTypeString(TypeContext fieldType)
        {
            string fieldTypeString;
            if (fieldType is null) return "void";
            if (fieldType is TypeIntContext)
            {
                fieldTypeString = "int16";
            }
            else if (fieldType is TypeCharContext)
            {
                fieldTypeString = "char";
            }
            else if (fieldType is TypeBoolContext)
            {
                fieldTypeString = "bool";
            }
            else if (fieldType is TypeClassContext classContext)
            {
                var className = classContext.className().ID().ToString() ?? throw new NullReferenceException();
                if (className.Equals("Array"))
                {
                    return "int16";
                }
                else if (className.Equals("String"))
                {
                    return "class [NJackOS.Interface]NJackOS.Interface.JackStringClass";
                }
                fieldTypeString = $"class {JackDefinitions.JackAssemblyName}.{className}";
            }
            else
            {
                throw new NotImplementedException(fieldType.GetText());
            }
            return fieldTypeString;
        }
    }
}
