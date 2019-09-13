using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irony.Parsing;

namespace Dev.Editor.BinAnalyzer.Grammar
{
    class BinAnalyzerGrammar : Irony.Parsing.Grammar
    {
        // Public constants ---------------------------------------------------

        public const string TYPE_BYTE = "byte";
        public const string TYPE_SBYTE = "sbyte";
        public const string TYPE_SHORT = "short";
        public const string TYPE_USHORT = "ushort";
        public const string TYPE_INT = "int";
        public const string TYPE_UINT = "uint";
        public const string TYPE_LONG = "long";
        public const string TYPE_ULONG = "ulong";
        public const string TYPE_SKIP = "skip";
        public const string TYPE_FLOAT = "float";
        public const string TYPE_DOUBLE = "double";
        
        public const string IDENTIFIER = "identifier";
        public const string QUALIFIED_IDENTIFIER = "qualifiedIdentifier";
        public const string POSITIVE_INT_NUMBER = "positiveIntNumber";
        public const string INT_NUMBER = "intNumber";
        public const string FLOAT_NUMBER = "floatNumber";
        public const string EXPRESSION = "expression";
        public const string COMPONENT = "component";
        public const string TERM = "term";
        public const string BIT_TERM = "bitTerm";
        public const string TYPE = "type";
        public const string BUILTIN_FIELD = "builtinField";
        public const string CUSTOM_FIELD = "customField";
        public const string BUILTIN_ARRAY_FIELD = "builtinArrayField";
        public const string CUSTOM_ARRAY_FIELD = "customArrayField";
        public const string ASSIGNMENT = "assignment";
        public const string SHOW_STATEMENT = "showStatement";
        public const string STATEMENT = "statement";
        public const string STATEMENTS = "statements";
        public const string STRUCT_DEF = "structDef";
        public const string DEFINITION = "definition";
        public const string DEFINITIONS = "definitions";
        public const string ANALYZER = "analyzer";

        // Public methods -----------------------------------------------------

        public BinAnalyzerGrammar()
        {
            var intTypeName = ToTerm(TYPE_BYTE) | TYPE_SBYTE | TYPE_SHORT | TYPE_USHORT | TYPE_INT | TYPE_UINT | TYPE_LONG | TYPE_ULONG | TYPE_SKIP;
            var floatTypeName = ToTerm(TYPE_FLOAT) | TYPE_DOUBLE;

            var identifier = new IdentifierTerminal(IDENTIFIER);
            var qualifiedIdentifier = new NonTerminal(QUALIFIED_IDENTIFIER);
            var positiveIntNumber = new NumberLiteral(POSITIVE_INT_NUMBER, NumberOptions.IntOnly);
            var intNumber = new NumberLiteral(INT_NUMBER, NumberOptions.IntOnly | NumberOptions.AllowSign);
            var floatNumber = new NumberLiteral(FLOAT_NUMBER, NumberOptions.AllowSign | NumberOptions.AllowStartEndDot);

            var expression = new NonTerminal(EXPRESSION);
            var component = new NonTerminal(COMPONENT);
            var term = new NonTerminal(TERM);
            var bitTerm = new NonTerminal(BIT_TERM);

            var type = new NonTerminal(TYPE);
            var builtinField = new NonTerminal(BUILTIN_FIELD);
            var customField = new NonTerminal(CUSTOM_FIELD);
            var builtinArrayField = new NonTerminal(BUILTIN_ARRAY_FIELD);
            var customArrayField = new NonTerminal(CUSTOM_ARRAY_FIELD);
            var assignment = new NonTerminal(ASSIGNMENT);
            var showStatement = new NonTerminal(SHOW_STATEMENT);
            var statement = new NonTerminal(STATEMENT);
            var statements = new NonTerminal(STATEMENTS);

            var structDef = new NonTerminal(STRUCT_DEF);
            var definition = new NonTerminal(DEFINITION);
            var definitions = new NonTerminal(DEFINITIONS);

            var program = new NonTerminal(ANALYZER);

            // Non-terminals

            qualifiedIdentifier.Rule = identifier | identifier + "." + qualifiedIdentifier;

            // Math expressions

            component.Rule = intNumber | floatNumber | qualifiedIdentifier | ToTerm("(") + expression + ")";
            bitTerm.Rule = component | bitTerm + "|" + component | bitTerm + "&" + component | bitTerm + "^" + component;
            term.Rule = bitTerm | term + "*" + bitTerm | term + "/" + bitTerm | term + "%" + bitTerm;            
            expression.Rule = term | expression + "+" + term | expression + "-" + term;

            // Fields

            type.Rule = intTypeName | floatTypeName;
            builtinField.Rule = type + identifier + ToTerm(";");
            customField.Rule = identifier + identifier + ToTerm(";");
            builtinArrayField.Rule = type + ToTerm("[") + expression + "]" + identifier + ";";
            customArrayField.Rule = identifier + "[" + expression + "]" + identifier + ";";
            assignment.Rule = ToTerm("let") + identifier + "=" + expression + ";";
            showStatement.Rule = ToTerm("show") + expression + ToTerm("as") + identifier + ";";
            statement.Rule = builtinField | customField | builtinArrayField | customArrayField | assignment | showStatement;
            MakePlusRule(statements, statement);

            // Definitions

            structDef.Rule = ToTerm("struct") + identifier + "{" + statements + "}" + ";";

            definition.Rule = structDef;
            MakeStarRule(definitions, definition);

            program.Rule = definitions + statements;

            MarkPunctuation(",", ";", "{", "}", "[", "]", "=", "struct", "enum", "let", "show", "as");

            Root = program;
        }
    }
}
