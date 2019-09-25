using Dev.Editor.BinAnalyzer.AnalyzerDefinition;
using Dev.Editor.BinAnalyzer.AnalyzerDefinition.Expressions;
using Dev.Editor.BinAnalyzer.Exceptions;
using Dev.Editor.BinAnalyzer.Grammar;
using Dev.Editor.BinAnalyzer.AnalyzerDefinition.Definitions;
using Dev.Editor.BinAnalyzer.AnalyzerDefinition.Statements;
using Irony.Parsing;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dev.Editor.Resources;

namespace Dev.Editor.BinAnalyzer
{
    public static class Compiler
    {
        private static ParseTree BuildParseTree(string source)
        {
            ParseTree parseTree;

            LanguageData language = new LanguageData(new BinAnalyzerGrammar());
            Parser parser = new Parser(language);
            parseTree = parser.Parse(source);

            return parseTree;
        }

        private static Definitions ProcessDefinitions(ParseTreeNode parseTreeNode)
        {
            var structDefinitions = new List<StructDefinition>();
            var unsignedEnumDefinitions = new List<UnsignedEnumDefinition>();
            var signedEnumDefinitions = new List<SignedEnumDefinition>();

            var definitions = new Definitions(structDefinitions, unsignedEnumDefinitions, signedEnumDefinitions);

            foreach (var node in parseTreeNode.ChildNodes)
            {
                var defNode = node.ChildNodes[0];

                if (defNode.Term.Name.Equals(BinAnalyzerGrammar.STRUCT_DEF))
                {
                    string name = defNode.ChildNodes[0].Token.Text;

                    List<BaseStatement> statements = ProcessStatements(defNode.ChildNodes[1], definitions);

                    var structDef = new StructDefinition(name, statements);

                    if (structDefinitions.Any(i => i.Name.Equals(structDef.Name)))
                        throw new SyntaxException(defNode.Span.Location.Column, 
                            defNode.Span.Location.Column, 
                            "Structure with the same name already exists!",
                            string.Format(Strings.Message_SyntaxError_StructureAlreadyExists, structDef.Name));

                    structDefinitions.Add(structDef);
                }
                else if (defNode.Term.Name.Equals(BinAnalyzerGrammar.ENUM_DEF))
                {
                    string name = defNode.ChildNodes[0].Token.Text;
                    string type = defNode.ChildNodes[1].ChildNodes[0].Token.Text;

                    if (new[] { "byte", "ushort", "uint", "ulong" }.Contains(type))
                    {
                        // Unsigned enum

                        var items = new List<UnsignedEnumItem>();
                        for (int i = 0; i < defNode.ChildNodes[2].ChildNodes.Count; i++)
                        {
                            var current = defNode.ChildNodes[2].ChildNodes[i];

                            string itemName = current.ChildNodes[0].Token.Text;

                            if (current.ChildNodes[1].Term.Name != BinAnalyzerGrammar.UINT_NUMBER)
                                throw new SyntaxException(current.ChildNodes[1].Token.Location.Line,
                                    current.ChildNodes[1].Token.Location.Column,
                                    "Unsigned enum should contain only unsigned items!",
                                    Strings.Message_SyntaxError_InvalidUnsignedEnumItem);

                            ulong value;
                            try
                            {
                                value = ulong.Parse(current.ChildNodes[1].Token.Text.Substring(0, current.ChildNodes[1].Token.Text.Length - 1));
                            }
                            catch (Exception e)
                            {
                                throw new SyntaxException(current.ChildNodes[1].Token.Location.Line,
                                    current.ChildNodes[1].Token.Location.Column,
                                    "Invalid unsigned integer value",
                                    string.Format(Strings.Message_SyntaxError_InvalidUnsignedIntegerValue, current.ChildNodes[1].Token.Text));
                            }

                            if (items.Any(ei => ei.Value == value))
                                throw new SyntaxException(current.ChildNodes[1].Token.Location.Line,
                                    current.ChildNodes[1].Token.Location.Column,
                                    "Enum item with this value already exists!",
                                    string.Format(Strings.Message_SyntaxError_EnumItemValueDuplicated, value));

                            var item = new UnsignedEnumItem(itemName, value);
                            items.Add(item);
                        }

                        var def = new UnsignedEnumDefinition(name, type, items);
                        unsignedEnumDefinitions.Add(def);
                    }
                    else if (new[] { "sbyte", "short", "int", "long"}.Contains(type))
                    {
                        // Signed enum

                        var items = new List<SignedEnumItem>();
                        for (int i = 0; i < defNode.ChildNodes[2].ChildNodes.Count; i++)
                        {
                            var current = defNode.ChildNodes[2].ChildNodes[i];

                            string itemName = current.ChildNodes[0].Token.Text;

                            if (current.ChildNodes[1].Term.Name != BinAnalyzerGrammar.INT_NUMBER)
                                throw new SyntaxException(current.ChildNodes[1].Token.Location.Line,
                                    current.ChildNodes[1].Token.Location.Column,
                                    "Signed enum should contain only signed items!",
                                    Strings.Message_SyntaxError_InvalidSignedEnumItem);

                            long value;
                            try
                            {
                                value = long.Parse(current.ChildNodes[1].Token.Text);
                            }
                            catch (Exception e)
                            {
                                throw new SyntaxException(current.ChildNodes[1].Token.Location.Line,
                                    current.ChildNodes[1].Token.Location.Column,
                                    "Invalid integer value",
                                    string.Format(Strings.Message_SyntaxError_InvalidIntegerValue, current.ChildNodes[1].Token.Text));
                            }

                            if (items.Any(ei => ei.Value == value))
                                throw new SyntaxException(current.ChildNodes[1].Token.Location.Line,
                                    current.ChildNodes[1].Token.Location.Column,
                                    "Enum item with this value already exists!",
                                    string.Format(Strings.Message_SyntaxError_EnumItemValueDuplicated, value));

                            var item = new SignedEnumItem(itemName, value);
                            items.Add(item);
                        }

                        var def = new SignedEnumDefinition(name, type, items);
                        signedEnumDefinitions.Add(def);
                    }
                }
                else
                    throw new InvalidOperationException("Unsupported definition!");
            }

            return definitions;
        }

        private static BaseExpressionNode InternalProcessIntNumber(ParseTreeNode parseTreeNode)
        {
            try
            {
                var value = long.Parse(parseTreeNode.Token.Text);
                return new NumericNode(parseTreeNode.Span.Location.Line, parseTreeNode.Span.Location.Column, value);
            }
            catch
            {
                throw new SyntaxException(parseTreeNode.Span.Location.Line,
                    parseTreeNode.Span.Location.Column,
                    "Invalid integer value",
                    string.Format(Strings.Message_SyntaxError_InvalidIntegerValue, parseTreeNode.Token.Text));
            }
        }

        private static BaseExpressionNode InternalProcessUintNumber(ParseTreeNode parseTreeNode)
        {
            try
            {
                var value = ulong.Parse(parseTreeNode.Token.Text.Substring(0, parseTreeNode.Token.Text.Length - 1));
                return new NumericNode(parseTreeNode.Span.Location.Line, parseTreeNode.Span.Location.Column, value);
            }
            catch
            {
                throw new SyntaxException(parseTreeNode.Span.Location.Line,
                    parseTreeNode.Span.Location.Column,
                    "Invalid unsigned integer value",
                    string.Format(Strings.Message_SyntaxError_InvalidIntegerValue, parseTreeNode.Token.Text));
            }
        }

        private static BaseExpressionNode InternalProcessFloatNumber(ParseTreeNode parseTreeNode)
        {
            try
            {
                var value = float.Parse(parseTreeNode.Token.Text, CultureInfo.InvariantCulture);
                return new NumericNode(parseTreeNode.Span.Location.Line, parseTreeNode.Span.Location.Column, value);
            }
            catch
            {
                throw new SyntaxException(parseTreeNode.Span.Location.Line,
                    parseTreeNode.Span.Location.Column,
                    "Invalid floating point value",
                    string.Format(Strings.Message_SyntaxError_InvalidFloatNumber, parseTreeNode.Token.Text));
            }
        }

        private static BaseExpressionNode InternalProcessBoolValue(ParseTreeNode parseTreeNode)
        {
            return new NumericNode(parseTreeNode.Span.Location.Line, parseTreeNode.Span.Location.Column, parseTreeNode.ChildNodes[0].Token.Text.Equals("true"));
        }

        private static BaseExpressionNode InternalProcessQualifiedIdentifier(ParseTreeNode parseTreeNode)
        {
            var identifier = new List<string>();

            var current = parseTreeNode;

            do
            {
                identifier.Add(current.ChildNodes[0].Token.Text);

                if (current.ChildNodes.Count == 3 && current.ChildNodes[2].Term.Name.Equals(BinAnalyzerGrammar.QUALIFIED_IDENTIFIER))
                    current = current.ChildNodes[2];
                else
                    current = null;
            }
            while (current != null);

            return new QualifiedIdentifierNode(parseTreeNode.Span.Location.Line, parseTreeNode.Span.Location.Column, identifier);
        }

        private static BaseExpressionNode InternalProcessComponent(ParseTreeNode parseTreeNode)
        {
            if (parseTreeNode.ChildNodes[0].Term.Name.Equals(BinAnalyzerGrammar.INT_NUMBER))
                return InternalProcessIntNumber(parseTreeNode.ChildNodes[0]);
            if (parseTreeNode.ChildNodes[0].Term.Name.Equals(BinAnalyzerGrammar.UINT_NUMBER))
                return InternalProcessUintNumber(parseTreeNode.ChildNodes[0]);
            if (parseTreeNode.ChildNodes[0].Term.Name.Equals(BinAnalyzerGrammar.FLOAT_NUMBER))
                return InternalProcessFloatNumber(parseTreeNode.ChildNodes[0]);
            if (parseTreeNode.ChildNodes[0].Term.Name.Equals(BinAnalyzerGrammar.BOOL_VALUE))
                return InternalProcessBoolValue(parseTreeNode.ChildNodes[0]);
            if (parseTreeNode.ChildNodes[0].Term.Name.Equals(BinAnalyzerGrammar.QUALIFIED_IDENTIFIER))
                return InternalProcessQualifiedIdentifier(parseTreeNode.ChildNodes[0]);
            if (parseTreeNode.ChildNodes[0].Term.Name.Equals(BinAnalyzerGrammar.EXPRESSION))
                return InternalProcessExpression(parseTreeNode.ChildNodes[0]);

            throw new InvalidOperationException("Invalid Component definition!");
        }

        private static BaseExpressionNode InternalProcessBitTerm(ParseTreeNode parseTreeNode)
        {
            if (parseTreeNode.ChildNodes[0].Term.Name.Equals(BinAnalyzerGrammar.COMPONENT))
                return InternalProcessComponent(parseTreeNode.ChildNodes[0]);

            if (parseTreeNode.ChildNodes[1].Token.Text.Equals("|"))
                return new BinaryOperator(parseTreeNode.Span.Location.Line, parseTreeNode.Span.Location.Column, InternalProcessBitTerm(parseTreeNode.ChildNodes[0]),
                    InternalProcessComponent(parseTreeNode.ChildNodes[2]),
                    BinaryOperation.BitOr);

            if (parseTreeNode.ChildNodes[1].Token.Text.Equals("&"))
                return new BinaryOperator(parseTreeNode.Span.Location.Line, parseTreeNode.Span.Location.Column, InternalProcessBitTerm(parseTreeNode.ChildNodes[0]),
                    InternalProcessComponent(parseTreeNode.ChildNodes[2]),
                    BinaryOperation.BitAnd);

            if (parseTreeNode.ChildNodes[1].Token.Text.Equals("^"))
                return new BinaryOperator(parseTreeNode.Span.Location.Line, parseTreeNode.Span.Location.Column, InternalProcessBitTerm(parseTreeNode.ChildNodes[0]),
                    InternalProcessComponent(parseTreeNode.ChildNodes[2]),
                    BinaryOperation.BitXor);

            throw new InvalidOperationException("Invalid BitTerm definition!");
        }

        private static BaseExpressionNode InternalProcessTerm(ParseTreeNode parseTreeNode)
        {
            if (parseTreeNode.ChildNodes[0].Term.Name.Equals(BinAnalyzerGrammar.BIT_TERM))
                return InternalProcessBitTerm(parseTreeNode.ChildNodes[0]);

            if (parseTreeNode.ChildNodes[1].Token.Text.Equals("*"))
                return new BinaryOperator(parseTreeNode.Span.Location.Line, parseTreeNode.Span.Location.Column, InternalProcessTerm(parseTreeNode.ChildNodes[0]),
                    InternalProcessBitTerm(parseTreeNode.ChildNodes[2]),
                    BinaryOperation.Multiply);

            if (parseTreeNode.ChildNodes[1].Token.Text.Equals("/"))
                return new BinaryOperator(parseTreeNode.Span.Location.Line, parseTreeNode.Span.Location.Column, InternalProcessTerm(parseTreeNode.ChildNodes[0]),
                    InternalProcessBitTerm(parseTreeNode.ChildNodes[2]),
                    BinaryOperation.Divide);

            if (parseTreeNode.ChildNodes[1].Token.Text.Equals("%"))
                return new BinaryOperator(parseTreeNode.Span.Location.Line, parseTreeNode.Span.Location.Column, InternalProcessTerm(parseTreeNode.ChildNodes[0]),
                    InternalProcessBitTerm(parseTreeNode.ChildNodes[2]),
                    BinaryOperation.Modulo);

            throw new InvalidOperationException("Invalid Term definition!");
        }

        private static BaseExpressionNode InternalProcessSum(ParseTreeNode parseTreeNode)
        {
            if (parseTreeNode.ChildNodes[0].Term.Name.Equals(BinAnalyzerGrammar.TERM))
                return InternalProcessTerm(parseTreeNode.ChildNodes[0]);

            if (parseTreeNode.ChildNodes[1].Token.Text.Equals("+"))
                return new BinaryOperator(parseTreeNode.Span.Location.Line, parseTreeNode.Span.Location.Column, InternalProcessSum(parseTreeNode.ChildNodes[0]),
                    InternalProcessTerm(parseTreeNode.ChildNodes[2]),
                    BinaryOperation.Add);

            if (parseTreeNode.ChildNodes[1].Token.Text.Equals("-"))
                return new BinaryOperator(parseTreeNode.Span.Location.Line, parseTreeNode.Span.Location.Column, InternalProcessSum(parseTreeNode.ChildNodes[0]),
                    InternalProcessTerm(parseTreeNode.ChildNodes[2]),
                    BinaryOperation.Subtract);

            throw new InvalidOperationException("Invalid Expression definition!");
        }

        private static BaseExpressionNode InternalProcessComparison(ParseTreeNode parseTreeNode)
        {
            if (parseTreeNode.ChildNodes[0].Term.Name.Equals(BinAnalyzerGrammar.SUM))
                return InternalProcessSum(parseTreeNode.ChildNodes[0]);

            if (parseTreeNode.ChildNodes[1].Token.Text.Equals("<"))
                return new BinaryOperator(parseTreeNode.Span.Location.Line, parseTreeNode.Span.Location.Column, InternalProcessComparison(parseTreeNode.ChildNodes[0]),
                    InternalProcessSum(parseTreeNode.ChildNodes[2]),
                    BinaryOperation.LessThan);

            if (parseTreeNode.ChildNodes[1].Token.Text.Equals("<="))
                return new BinaryOperator(parseTreeNode.Span.Location.Line, parseTreeNode.Span.Location.Column, InternalProcessComparison(parseTreeNode.ChildNodes[0]),
                    InternalProcessSum(parseTreeNode.ChildNodes[2]),
                    BinaryOperation.LessThanOrEqual);

            if (parseTreeNode.ChildNodes[1].Token.Text.Equals("=="))
                return new BinaryOperator(parseTreeNode.Span.Location.Line, parseTreeNode.Span.Location.Column, InternalProcessComparison(parseTreeNode.ChildNodes[0]),
                    InternalProcessSum(parseTreeNode.ChildNodes[2]),
                    BinaryOperation.Equal);

            if (parseTreeNode.ChildNodes[1].Token.Text.Equals("!="))
                return new BinaryOperator(parseTreeNode.Span.Location.Line, parseTreeNode.Span.Location.Column, InternalProcessComparison(parseTreeNode.ChildNodes[0]),
                    InternalProcessSum(parseTreeNode.ChildNodes[2]),
                    BinaryOperation.Inequal);

            if (parseTreeNode.ChildNodes[1].Token.Text.Equals(">="))
                return new BinaryOperator(parseTreeNode.Span.Location.Line, parseTreeNode.Span.Location.Column, InternalProcessComparison(parseTreeNode.ChildNodes[0]),
                    InternalProcessSum(parseTreeNode.ChildNodes[2]),
                    BinaryOperation.GreaterThanOrEqual);

            if (parseTreeNode.ChildNodes[1].Token.Text.Equals(">"))
                return new BinaryOperator(parseTreeNode.Span.Location.Line, parseTreeNode.Span.Location.Column, InternalProcessComparison(parseTreeNode.ChildNodes[0]),
                    InternalProcessSum(parseTreeNode.ChildNodes[2]),
                    BinaryOperation.GreaterThan);

            throw new InvalidOperationException("Invalid Expression definition!");
        }

        private static BaseExpressionNode InternalProcessExpression(ParseTreeNode parseTreeNode)
        {
            if (parseTreeNode.ChildNodes[0].Term.Name.Equals(BinAnalyzerGrammar.COMPARISON))
                return InternalProcessComparison(parseTreeNode.ChildNodes[0]);

            if (parseTreeNode.ChildNodes[1].Token.Text.Equals("&&"))
                return new BinaryOperator(parseTreeNode.Span.Location.Line, parseTreeNode.Span.Location.Column, InternalProcessExpression(parseTreeNode.ChildNodes[0]),
                    InternalProcessComparison(parseTreeNode.ChildNodes[2]),
                    BinaryOperation.LogicAnd);

            if (parseTreeNode.ChildNodes[1].Token.Text.Equals("||"))
                return new BinaryOperator(parseTreeNode.Span.Location.Line, parseTreeNode.Span.Location.Column, InternalProcessExpression(parseTreeNode.ChildNodes[0]),
                    InternalProcessComparison(parseTreeNode.ChildNodes[2]),
                    BinaryOperation.LogicOr);

            if (parseTreeNode.ChildNodes[1].Token.Text.Equals("^^"))
                return new BinaryOperator(parseTreeNode.Span.Location.Line, parseTreeNode.Span.Location.Column, InternalProcessExpression(parseTreeNode.ChildNodes[0]),
                    InternalProcessComparison(parseTreeNode.ChildNodes[2]),
                    BinaryOperation.LogicXor);

            throw new InvalidOperationException("Invalid Expression definition!");
        }

        private static Expression ProcessExpression(ParseTreeNode parseTreeNode)
        {
            return new Expression(parseTreeNode.Span.Location.Line, 
                parseTreeNode.Span.Location.Position, 
                InternalProcessExpression(parseTreeNode));
        }

        private static List<BaseStatement> ProcessStatements(ParseTreeNode parseTreeNode, Definitions definitions)
        {
            List<BaseStatement> result = new List<BaseStatement>();

            for (int i = 0; i < parseTreeNode.ChildNodes.Count; i++)
            {
                var current = parseTreeNode.ChildNodes[i].ChildNodes[0];

                if (current.Term.Name.Equals(BinAnalyzerGrammar.BUILTIN_FIELD))
                {
                    string type = current.ChildNodes[0].ChildNodes[0].ChildNodes[0].Token.Text;
                    string name = current.ChildNodes[1].Token.Text;

                    if (result.Any(r => r is BaseFieldStatement fieldStatement && fieldStatement.Name.Equals(name)))
                        throw new SyntaxException(current.Span.Location.Line, 
                            current.Span.Location.Column, 
                            $"Field with name {name} already exists!",
                            string.Format(Strings.Message_SyntaxError_FieldAlreadyExists, name));

                    BaseFieldStatement statement = FieldFactory.FromTypeName(current.Span.Location.Line, current.Span.Location.Column, type, name);
                    result.Add(statement);
                }
                else if (current.Term.Name.Equals(BinAnalyzerGrammar.CUSTOM_FIELD))
                {
                    string type = current.ChildNodes[0].Token.Text;
                    string name = current.ChildNodes[1].Token.Text;

                    if (result.Any(r => r is BaseFieldStatement fieldStatement && fieldStatement.Name.Equals(name)))
                        throw new SyntaxException(current.Span.Location.Line,
                            current.Span.Location.Column,
                            $"Field with name {name} already exists!",
                            string.Format(Strings.Message_SyntaxError_FieldAlreadyExists, name));

                    // Struct?

                    var structDef = definitions.StructDefinitions.FirstOrDefault(d => d.Name.Equals(type));
                    if (structDef != null)
                    {
                        StructFieldStatement statement = new StructFieldStatement(current.Span.Location.Line, current.Span.Location.Column, name, structDef);
                        result.Add(statement);
                        continue;
                    }

                    // Signed enum?

                    var signedEnumDef = definitions.SignedEnumDefinitions.FirstOrDefault(d => d.Name.Equals(type));
                    if (signedEnumDef != null)
                    {
                        var statement = FieldFactory.FromSignedEnumTypeName(current.Span.Location.Line, 
                            current.Span.Location.Column, 
                            name, 
                            signedEnumDef);
                        result.Add(statement);
                        continue;
                    }

                    var unsignedEnumDef = definitions.UnsignedEnumDefinitions.FirstOrDefault(d => d.Name.Equals(type));
                    if (unsignedEnumDef != null)
                    {
                        var statement = FieldFactory.FromUnsignedEnumTypeName(current.Span.Location.Line,
                            current.Span.Location.Column,
                            name,
                            unsignedEnumDef);
                        result.Add(statement);
                        continue;
                    }

                    throw new SyntaxException(current.Span.Location.Line, 
                        current.Span.Location.Column, 
                        $"Cannot find type {type} !",
                        string.Format(Strings.Message_SyntaxError_CannotFindTypeName, type));
                }
                else if (current.Term.Name.Equals(BinAnalyzerGrammar.BUILTIN_ARRAY_FIELD))
                {
                    string type = current.ChildNodes[0].ChildNodes[0].ChildNodes[0].Token.Text;
                    Expression count = ProcessExpression(current.ChildNodes[1]);
                    string name = current.ChildNodes[2].Token.Text;

                    if (result.Any(r => r is BaseFieldStatement fieldStatement && fieldStatement.Name.Equals(name)))
                        throw new SyntaxException(current.Span.Location.Line,
                            current.Span.Location.Column,
                            $"Field with name {name} already exists!",
                            string.Format(Strings.Message_SyntaxError_FieldAlreadyExists, name));
                    BaseFieldStatement statement = ArrayFieldFactory.FromTypeName(current.Span.Location.Line, current.Span.Location.Column, type, name, count);
                    result.Add(statement);
                }
                else if (current.Term.Name.Equals(BinAnalyzerGrammar.CUSTOM_ARRAY_FIELD))
                {
                    string type = current.ChildNodes[0].Token.Text;
                    Expression count = ProcessExpression(current.ChildNodes[1]);
                    string name = current.ChildNodes[2].Token.Text;

                    if (result.Any(r => r is BaseFieldStatement fieldStatement && fieldStatement.Name.Equals(name)))
                        throw new SyntaxException(current.Span.Location.Line,
                            current.Span.Location.Column,
                            $"Field with name {name} already exists!",
                            string.Format(Strings.Message_SyntaxError_FieldAlreadyExists, name));

                    // Struct?

                    var structDef = definitions.StructDefinitions.FirstOrDefault(d => d.Name.Equals(type));
                    if (structDef != null)
                    {
                        StructArrayFieldStatement statement = new StructArrayFieldStatement(current.Span.Location.Line, current.Span.Location.Column, name, structDef, count);
                        result.Add(statement);
                        continue;
                    }
                    else
                        throw new SyntaxException(current.Span.Location.Line,
                            current.Span.Location.Column,
                            $"Cannot find type {type} !",
                            string.Format(Strings.Message_SyntaxError_CannotFindTypeName, type));

                }
                else if (current.Term.Name.Equals(BinAnalyzerGrammar.ASSIGNMENT))
                {
                    string name = current.ChildNodes[0].Token.Text;
                    Expression expression = ProcessExpression(current.ChildNodes[1]);

                    if (result.Any(r => r is BaseFieldStatement fieldStatement && fieldStatement.Name.Equals(name)))
                        if (result.Any(r => r is BaseFieldStatement fieldStatement && fieldStatement.Name.Equals(name)))
                            throw new SyntaxException(current.Span.Location.Line,
                                current.Span.Location.Column,
                                $"Field with name {name} already exists!",
                                string.Format(Strings.Message_SyntaxError_FieldAlreadyExists, name));

                    var statement = new AssignmentStatement(current.Span.Location.Line, current.Span.Location.Column, name, expression);
                    result.Add(statement);
                }
                else if (current.Term.Name.Equals(BinAnalyzerGrammar.SHOW_STATEMENT))
                {
                    Expression expression = ProcessExpression(current.ChildNodes[0]);
                    string alias = current.ChildNodes[1].Token.Text;

                    if (result.Any(r => r is BaseFieldStatement fieldStatement && fieldStatement.Name.Equals(alias)))
                        throw new SyntaxException(current.Span.Location.Line,
                            current.Span.Location.Column,
                            $"Field with name {alias} already exists!",
                            string.Format(Strings.Message_SyntaxError_FieldAlreadyExists, alias));

                    var statement = new ShowStatement(current.Span.Location.Line, current.Span.Location.Column, expression, alias);
                    result.Add(statement);
                }
                else if (current.Term.Name.Equals(BinAnalyzerGrammar.IF_STATEMENT))
                {
                    Expression expression = ProcessExpression(current.ChildNodes[0]);
                    var statements = ProcessStatements(current.ChildNodes[1], definitions);

                    var statement = new IfStatement(current.Span.Location.Line, current.Span.Location.Column, expression, statements);
                    result.Add(statement);
                }
                else
                    throw new InvalidOperationException("Invalid statement!");
            }

            return result;
        }

        private static Analyzer ProcessParseTree(ParseTree parseTree)
        {
            Definitions definitions;
            definitions = ProcessDefinitions(parseTree.Root.ChildNodes[0]);

            List<BaseStatement> statements;
            statements = ProcessStatements(parseTree.Root.ChildNodes[1], definitions);

            return new Analyzer(statements);            
        }

        public static Analyzer Compile(string source)
        {
            ParseTree parseTree = BuildParseTree(source);

            if (parseTree.Status == ParseTreeStatus.Error)
            {
                if (parseTree.HasErrors())
                {
                    var message = parseTree.ParserMessages.First(m => m.Level == Irony.ErrorLevel.Error);

                    throw new SyntaxException(message.Location.Line, 
                        message.Location.Column, 
                        $"Syntax error: {message.Message}", 
                        string.Format(Strings.Message_SyntaxError_SyntaxError, message.Message));
                }
                else
                {
                    throw new SyntaxException(1, 1, "Failed to process definition!", Strings.Message_SyntaxError_FailedToProcessDefinition);
                }
            }

            Analyzer program = ProcessParseTree(parseTree);
            return program;
        }
    }
}
