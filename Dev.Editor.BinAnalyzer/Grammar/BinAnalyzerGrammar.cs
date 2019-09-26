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
        public const string TYPE_CHAR = "char";

        public const string ONE_LINE_COMMENT = "oneLineComment";
        public const string MULTIPLE_LINE_COMMENT = "multipleLineComment";
        public const string IDENTIFIER = "identifier";
        public const string QUALIFIED_IDENTIFIER = "qualifiedIdentifier";
        public const string POSITIVE_INT_NUMBER = "positiveIntNumber";
        public const string INT_NUMBER = "intNumber";
        public const string UINT_NUMBER = "uintNumber";
        public const string FLOAT_NUMBER = "floatNumber";
        public const string BOOL_VALUE = "boolValue";
        public const string SUM = "sum";
        public const string COMPARISON = "comparison";
        public const string EXPRESSION = "expression";
        public const string COMPONENT = "component";
        public const string TERM = "term";
        public const string BIT_TERM = "bitTerm";
        public const string TYPE = "type";
        public const string INT_TYPE_NAME = "intTypeName";
        public const string FLOAT_TYPE_NAME = "floatTypeName";
        public const string SPECIAL_TYPE_NAME = "specialTypeName";
        public const string BUILTIN_FIELD = "builtinField";
        public const string CUSTOM_FIELD = "customField";
        public const string BUILTIN_ARRAY_FIELD = "builtinArrayField";
        public const string CUSTOM_ARRAY_FIELD = "customArrayField";
        public const string IF_STATEMENT = "if";
        public const string IF_CONDITION = "ifCondition";
        public const string ELSEIF_CONDITION = "elseIfCondition";
        public const string ELSEIF_CONDITIONS = "elseIfConditions";
        public const string ELSE_CONDITION = "elseCondition";
        public const string ASSIGNMENT = "assignment";
        public const string SHOW_STATEMENT = "showStatement";
        public const string STATEMENT = "statement";
        public const string STATEMENTS = "statements";
        public const string STRUCT_DEF = "structDef";
        public const string ENUM_DEF = "enumDef";
        public const string ENUM_ITEM_DEF = "enumItemDef";
        public const string ENUM_ITEMS_DEF = "enumItemsDef";
        public const string DEFINITION = "definition";
        public const string DEFINITIONS = "definitions";
        public const string ANALYZER = "analyzer";

        // Public methods -----------------------------------------------------

        public BinAnalyzerGrammar()
        {
            var oneLineComment = new CommentTerminal(ONE_LINE_COMMENT, "//", "\r", "\n", "\u2085", "\u2028", "\u2029");
            var multipleLineComment = new CommentTerminal(MULTIPLE_LINE_COMMENT, "/*", "*/");

            var intTypeName = new NonTerminal(INT_TYPE_NAME);
            var floatTypeName = new NonTerminal(FLOAT_TYPE_NAME);
            var specialTypeName = new NonTerminal(SPECIAL_TYPE_NAME);

            var identifier = new IdentifierTerminal(IDENTIFIER);
            var qualifiedIdentifier = new NonTerminal(QUALIFIED_IDENTIFIER);            
            var intNumber = new RegexBasedTerminal(INT_NUMBER, "\\-?[0-9]+");
            var uintNumber = new RegexBasedTerminal(UINT_NUMBER, "[0-9]+[uU]");
            var floatNumber = new RegexBasedTerminal(FLOAT_NUMBER, "[0-9]+\\.[0-9]+f?");
            var boolValue = new NonTerminal(BOOL_VALUE);

            var expression = new NonTerminal(EXPRESSION);
            var comparison = new NonTerminal(COMPARISON);
            var sum = new NonTerminal(SUM);
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
            var ifStatement = new NonTerminal(IF_STATEMENT);
            var ifCondition = new NonTerminal(IF_CONDITION);
            var elseifCondition = new NonTerminal(ELSEIF_CONDITION);
            var elseifConditions = new NonTerminal(ELSEIF_CONDITIONS);
            var elseCondition = new NonTerminal(ELSE_CONDITION);
            var statement = new NonTerminal(STATEMENT);
            var statements = new NonTerminal(STATEMENTS);

            var structDef = new NonTerminal(STRUCT_DEF);
            var enumDef = new NonTerminal(ENUM_DEF);
            var enumItemDef = new NonTerminal(ENUM_ITEM_DEF);
            var enumItemsDef = new NonTerminal(ENUM_ITEMS_DEF);
            var definition = new NonTerminal(DEFINITION);
            var definitions = new NonTerminal(DEFINITIONS);

            var program = new NonTerminal(ANALYZER);

            // Comments
            NonGrammarTerminals.Add(oneLineComment);
            NonGrammarTerminals.Add(multipleLineComment);

            // Non-terminals

            qualifiedIdentifier.Rule = identifier | identifier + "." + qualifiedIdentifier;
            boolValue.Rule = ToTerm("true") | "false";

            // Math expressions

            component.Rule = intNumber | uintNumber | floatNumber | boolValue | qualifiedIdentifier | ToTerm("(") + expression + ")";
            bitTerm.Rule = component | bitTerm + "|" + component | bitTerm + "&" + component | bitTerm + "^" + component;
            term.Rule = bitTerm | term + "*" + bitTerm | term + "/" + bitTerm | term + "%" + bitTerm;            
            sum.Rule = term | sum + "+" + term | sum + "-" + term;
            comparison.Rule = sum | comparison + "<" + sum | comparison + "<=" + sum | comparison + "==" + sum | comparison + "!=" + sum | comparison + ">=" + sum | comparison + ">" + sum;
            expression.Rule = comparison | expression + "&&" + comparison | expression + "||" + comparison | expression + "^^" + comparison;

            // Fields

            intTypeName.Rule = ToTerm(TYPE_BYTE) | TYPE_SBYTE | TYPE_SHORT | TYPE_USHORT | TYPE_INT | TYPE_UINT | TYPE_LONG | TYPE_ULONG;
            floatTypeName.Rule = ToTerm(TYPE_FLOAT) | TYPE_DOUBLE;
            specialTypeName.Rule = ToTerm(TYPE_CHAR) | TYPE_SKIP;
            type.Rule = intTypeName | floatTypeName | specialTypeName;
            builtinField.Rule = type + identifier + ToTerm(";");
            customField.Rule = identifier + identifier + ToTerm(";");
            builtinArrayField.Rule = type + ToTerm("[") + expression + "]" + identifier + ";";
            customArrayField.Rule = identifier + "[" + expression + "]" + identifier + ";";
            ifStatement.Rule = ifCondition + elseifConditions + elseCondition;
            ifCondition.Rule = ToTerm("if") + "(" + expression + ")" + "{" + statements + "}";
            elseifCondition.Rule = ToTerm("elseif") + "(" + expression + ")" + "{" + statements + "}";
            MakeStarRule(elseifConditions, elseifCondition);
            elseCondition.Rule = ToTerm("else") + "{" + statements + "}" | Empty;
            assignment.Rule = ToTerm("let") + identifier + "=" + expression + ";";
            showStatement.Rule = ToTerm("show") + expression + ToTerm("as") + identifier + ";";
            statement.Rule = builtinField | customField | builtinArrayField | customArrayField | assignment | showStatement | ifStatement;
            MakePlusRule(statements, statement);

            // Definitions

            structDef.Rule = ToTerm("struct") + identifier + "{" + statements + "}" + ";";

            enumDef.Rule = ToTerm("enum") + identifier + ":" + intTypeName + "{" + enumItemsDef + "}" + ";";
            enumItemDef.Rule = identifier + "=" + intNumber | identifier + "=" + uintNumber;
            enumItemsDef.Rule = MakePlusRule(enumItemsDef, ToTerm(","), enumItemDef);

            definition.Rule = structDef | enumDef;
            MakeStarRule(definitions, definition);

            program.Rule = definitions + statements;

            MarkPunctuation(",", ";", ":", "{", "}", "[", "]", "(", ")", "=", "struct", "enum", "let", "show", "as", "if", "elseif", "else");

            Root = program;
        }
    }
}
