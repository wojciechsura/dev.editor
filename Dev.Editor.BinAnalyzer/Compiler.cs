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
using Dev.Editor.BinAnalyzer.AnalyzerDefinition.Show;

namespace Dev.Editor.BinAnalyzer
{
    public static partial class Compiler
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
            var enumDefinitions = new List<BaseEnumDefinition>();

            var definitions = new Definitions(structDefinitions, enumDefinitions);

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
                    var definition = BuildEnumDefinition(defNode);
                    enumDefinitions.Add(definition);
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

                if (current.ChildNodes.Count == 2 && current.ChildNodes[1].Term.Name.Equals(BinAnalyzerGrammar.QUALIFIED_IDENTIFIER))
                    current = current.ChildNodes[1];
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
                parseTreeNode.Span.Location.Column, 
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

                    // Enum?

                    var enumDef = definitions.EnumDefinitions.FirstOrDefault(d => d.Name.Equals(type));
                    if (enumDef != null)
                    {
                        var statement = FieldFactory.FromEnum(current.Span.Location.Line, 
                            current.Span.Location.Column, 
                            name, 
                            enumDef);
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
                    string alias = current.ChildNodes[1].Token.Text;
                    if (result.Any(r => r is BaseFieldStatement fieldStatement && fieldStatement.Name.Equals(alias)))
                        throw new SyntaxException(current.Span.Location.Line,
                            current.Span.Location.Column,
                            $"Field with name {alias} already exists!",
                            string.Format(Strings.Message_SyntaxError_FieldAlreadyExists, alias));

                    var showValue = current.ChildNodes[0].ChildNodes[0];

                    BaseStatement statement;

                    if (showValue.Term.Name == BinAnalyzerGrammar.EXPRESSION)
                    {
                        Expression expression = ProcessExpression(showValue);
                        var expressionShowValue = new ExpressionShowValue(current.Span.Location.Line, current.Span.Location.Column, expression);
                        statement = new ShowStatement(current.Span.Location.Line, current.Span.Location.Column, expressionShowValue, alias);
                    }
                    else if (showValue.Term.Name == BinAnalyzerGrammar.ENUM_CONST)
                    {
                        var enumName = showValue.ChildNodes[0].Token.Text;
                        var enumMember = showValue.ChildNodes[1].Token.Text;

                        var enumDef = definitions.EnumDefinitions.FirstOrDefault(d => d.Name.Equals(enumName));
                        if (enumDef != null)
                        {
                            var enumShowValue = new EnumShowValue(enumDef, enumMember);
                            statement = new ShowStatement(current.Span.Location.Line, current.Span.Location.Column, enumShowValue, alias);
                        }                        
                        else
                        {
                            throw new SyntaxException(current.Span.Location.Line,
                                current.Span.Location.Column,
                                $"Cannot find type {enumName} !",
                                string.Format(Strings.Message_SyntaxError_CannotFindTypeName, enumName));
                        }                        
                    }
                    else
                    {
                        throw new InvalidOperationException("Unsupported show statement syntax!");
                    }
                    
                    result.Add(statement);
                }
                else if (current.Term.Name.Equals(BinAnalyzerGrammar.IF_STATEMENT))
                {
                    var conditions = new List<(Expression condition, List<BaseStatement> statements)>();

                    // Regular if condition is required
                    Expression expression = ProcessExpression(current.ChildNodes[0].ChildNodes[0]);
                    var statements = ProcessStatements(current.ChildNodes[0].ChildNodes[1], definitions);

                    conditions.Add((expression, statements));

                    for (int j = 0; j < current.ChildNodes[1].ChildNodes.Count; j++)
                    {
                        var elseifExpression = ProcessExpression(current.ChildNodes[1].ChildNodes[j].ChildNodes[0]);
                        var elseifStatements = ProcessStatements(current.ChildNodes[1].ChildNodes[j].ChildNodes[1], definitions);

                        conditions.Add((elseifExpression, elseifStatements));
                    }

                    List<BaseStatement> elseStatements = null;
                    if (current.ChildNodes[2].ChildNodes.Count > 0)
                    {
                        elseStatements = ProcessStatements(current.ChildNodes[2].ChildNodes[0], definitions);
                    }

                    var statement = new IfStatement(current.Span.Location.Line, current.Span.Location.Column, conditions, elseStatements);
                    result.Add(statement);
                }
                else if (current.Term.Name.Equals(BinAnalyzerGrammar.REPEAT_STATEMENT))
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
                        RepeatStatement statement = new RepeatStatement(current.Span.Location.Line, current.Span.Location.Column, name, structDef);
                        result.Add(statement);
                        continue;
                    }
                    else
                        throw new SyntaxException(current.Span.Location.Line,
                            current.Span.Location.Column,
                            $"Cannot find type {type} !",
                            string.Format(Strings.Message_SyntaxError_CannotFindTypeName, type));
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
