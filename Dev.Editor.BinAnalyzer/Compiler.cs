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

            var definitions = new Definitions(structDefinitions);

            foreach (var node in parseTreeNode.ChildNodes)
            {
                var defNode = node.ChildNodes[0];

                if (defNode.Term.Name.Equals(BinAnalyzerGrammar.STRUCT_DEF))
                {
                    string name = defNode.ChildNodes[0].Token.Text;

                    List<BaseStatement> statements = ProcessStatements(defNode.ChildNodes[1], definitions);

                    var structDef = new StructDefinition(name, statements);

                    if (structDefinitions.Any(i => i.Name.Equals(structDef.Name)))
                        throw new SyntaxException(defNode.Token.Location.Column, 
                            defNode.Token.Location.Column, 
                            "Structure with the same name already exists!",
                            string.Format(Strings.Message_SyntaxError_StructureAlreadyExists, structDef.Name));

                    structDefinitions.Add(structDef);
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
                return new NumericNode(parseTreeNode.Token.Location.Line, parseTreeNode.Token.Location.Column, value);
            }
            catch
            {
                throw new SyntaxException(parseTreeNode.Token.Location.Line,
                    parseTreeNode.Token.Location.Column,
                    "Invalid integer value",
                    string.Format(Strings.Message_SyntaxError_InvalidIntegerValue, parseTreeNode.Token.Text));
            }
        }

        private static BaseExpressionNode InternalProcessFloatNumber(ParseTreeNode parseTreeNode)
        {
            try
            {
                var value = float.Parse(parseTreeNode.Token.Text, CultureInfo.InvariantCulture);
                return new NumericNode(parseTreeNode.Token.Location.Line, parseTreeNode.Token.Location.Column, value);
            }
            catch
            {
                throw new SyntaxException(parseTreeNode.Token.Location.Line,
                    parseTreeNode.Token.Location.Column,
                    "Invalid floating point value",
                    string.Format(Strings.Message_SyntaxError_InvalidFloatNumber, parseTreeNode.Token.Text));
            }
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

            return new QualifiedIdentifierNode(current.Token.Location.Line, current.Token.Location.Column, identifier);
        }


        private static BaseExpressionNode InternalProcessComponent(ParseTreeNode parseTreeNode)
        {
            if (parseTreeNode.ChildNodes[0].Term.Name.Equals(BinAnalyzerGrammar.INT_NUMBER))
                return InternalProcessIntNumber(parseTreeNode.ChildNodes[0]);
            if (parseTreeNode.ChildNodes[0].Term.Name.Equals(BinAnalyzerGrammar.FLOAT_NUMBER))
                return InternalProcessFloatNumber(parseTreeNode.ChildNodes[0]);
            if (parseTreeNode.ChildNodes[0].Term.Name.Equals(BinAnalyzerGrammar.QUALIFIED_IDENTIFIER))
                return InternalProcessQualifiedIdentifier(parseTreeNode.ChildNodes[0]);

            if (parseTreeNode.ChildNodes[0].Token.Text.Equals("(") &&
                parseTreeNode.ChildNodes[2].Token.Text.Equals(")"))
                return InternalProcessExpression(parseTreeNode.ChildNodes[1]);

            throw new InvalidOperationException("Invalid Component definition!");
        }

        private static BaseExpressionNode InternalProcessBitTerm(ParseTreeNode parseTreeNode)
        {
            if (parseTreeNode.ChildNodes[0].Term.Name.Equals(BinAnalyzerGrammar.COMPONENT))
                return InternalProcessComponent(parseTreeNode.ChildNodes[0]);

            if (parseTreeNode.ChildNodes[1].Token.Text.Equals("|"))
                return new BinaryOperator(parseTreeNode.Token.Location.Line, parseTreeNode.Token.Location.Column, InternalProcessBitTerm(parseTreeNode.ChildNodes[0]),
                    InternalProcessComponent(parseTreeNode.ChildNodes[2]),
                    BinaryOperation.Or);

            if (parseTreeNode.ChildNodes[1].Token.Text.Equals("&"))
                return new BinaryOperator(parseTreeNode.Token.Location.Line, parseTreeNode.Token.Location.Column, InternalProcessBitTerm(parseTreeNode.ChildNodes[0]),
                    InternalProcessComponent(parseTreeNode.ChildNodes[2]),
                    BinaryOperation.And);

            if (parseTreeNode.ChildNodes[1].Token.Text.Equals("^"))
                return new BinaryOperator(parseTreeNode.Token.Location.Line, parseTreeNode.Token.Location.Column, InternalProcessBitTerm(parseTreeNode.ChildNodes[0]),
                    InternalProcessComponent(parseTreeNode.ChildNodes[2]),
                    BinaryOperation.Xor);

            throw new InvalidOperationException("Invalid BitTerm definition!");
        }

        private static BaseExpressionNode InternalProcessTerm(ParseTreeNode parseTreeNode)
        {
            if (parseTreeNode.ChildNodes[0].Term.Name.Equals(BinAnalyzerGrammar.BIT_TERM))
                return InternalProcessBitTerm(parseTreeNode.ChildNodes[0]);

            if (parseTreeNode.ChildNodes[1].Token.Text.Equals("*"))
                return new BinaryOperator(parseTreeNode.Token.Location.Line, parseTreeNode.Token.Location.Column, InternalProcessTerm(parseTreeNode.ChildNodes[0]),
                    InternalProcessBitTerm(parseTreeNode.ChildNodes[2]),
                    BinaryOperation.Multiply);

            if (parseTreeNode.ChildNodes[1].Token.Text.Equals("/"))
                return new BinaryOperator(parseTreeNode.Token.Location.Line, parseTreeNode.Token.Location.Column, InternalProcessTerm(parseTreeNode.ChildNodes[0]),
                    InternalProcessBitTerm(parseTreeNode.ChildNodes[2]),
                    BinaryOperation.Divide);

            if (parseTreeNode.ChildNodes[1].Token.Text.Equals("%"))
                return new BinaryOperator(parseTreeNode.Token.Location.Line, parseTreeNode.Token.Location.Column, InternalProcessTerm(parseTreeNode.ChildNodes[0]),
                    InternalProcessBitTerm(parseTreeNode.ChildNodes[2]),
                    BinaryOperation.Modulo);

            throw new InvalidOperationException("Invalid Term definition!");
        }

        private static BaseExpressionNode InternalProcessExpression(ParseTreeNode parseTreeNode)
        {
            if (parseTreeNode.ChildNodes[0].Term.Name.Equals(BinAnalyzerGrammar.TERM))
                return InternalProcessTerm(parseTreeNode.ChildNodes[0]);

            if (parseTreeNode.ChildNodes[1].Token.Text.Equals("+"))
                return new BinaryOperator(parseTreeNode.Token.Location.Line, parseTreeNode.Token.Location.Column, InternalProcessExpression(parseTreeNode.ChildNodes[0]),
                    InternalProcessTerm(parseTreeNode.ChildNodes[2]),
                    BinaryOperation.Add);

            if (parseTreeNode.ChildNodes[1].Token.Text.Equals("-"))
                return new BinaryOperator(parseTreeNode.Token.Location.Line, parseTreeNode.Token.Location.Column, InternalProcessExpression(parseTreeNode.ChildNodes[0]),
                    InternalProcessTerm(parseTreeNode.ChildNodes[2]),
                    BinaryOperation.Subtract);

            throw new InvalidOperationException("Invalid Expression definition!");
        }

        private static Expression ProcessExpression(ParseTreeNode parseTreeNode)
        {
            return new Expression(parseTreeNode.Token.Location.Line, 
                parseTreeNode.Token.Location.Position, 
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
                    string type = current.ChildNodes[0].ChildNodes[0].Token.Text;
                    string name = current.ChildNodes[1].Token.Text;

                    if (result.Any(r => r is BaseFieldStatement fieldStatement && fieldStatement.Name.Equals(name)))
                        throw new SyntaxException(current.Token.Location.Line, 
                            current.Token.Location.Column, 
                            $"Field with name {name} already exists!",
                            string.Format(Strings.Message_SyntaxError_FieldAlreadyExists, name));

                    BaseFieldStatement statement = FieldFactory.FromTypeName(current.Token.Location.Line, current.Token.Location.Column, type, name);
                    result.Add(statement);
                }
                else if (current.Term.Name.Equals(BinAnalyzerGrammar.CUSTOM_FIELD))
                {
                    string type = current.ChildNodes[0].Token.Text;
                    string name = current.ChildNodes[1].Token.Text;

                    if (result.Any(r => r is BaseFieldStatement fieldStatement && fieldStatement.Name.Equals(name)))
                        throw new SyntaxException(current.Token.Location.Line,
                            current.Token.Location.Column,
                            $"Field with name {name} already exists!",
                            string.Format(Strings.Message_SyntaxError_FieldAlreadyExists, name));

                    // Struct?

                    var structDef = definitions.StructDefinitions.FirstOrDefault(d => d.Name.Equals(type));
                    if (structDef != null)
                    {
                        StructFieldStatement statement = new StructFieldStatement(current.Token.Location.Line, current.Token.Location.Column, name, structDef);
                        result.Add(statement);
                        continue;
                    }
                    else
                        throw new SyntaxException(current.Token.Location.Line, 
                            current.Token.Location.Column, 
                            $"Cannot find type {type} !",
                            string.Format(Strings.Message_SyntaxError_CannotFindTypeName, type));
                }
                else if (current.Term.Name.Equals(BinAnalyzerGrammar.BUILTIN_ARRAY_FIELD))
                {
                    string type = current.ChildNodes[0].ChildNodes[0].Token.Text;
                    Expression count = ProcessExpression(current.ChildNodes[1]);
                    string name = current.ChildNodes[2].Token.Text;

                    if (result.Any(r => r is BaseFieldStatement fieldStatement && fieldStatement.Name.Equals(name)))
                        throw new SyntaxException(current.Token.Location.Line,
                            current.Token.Location.Column,
                            $"Field with name {name} already exists!",
                            string.Format(Strings.Message_SyntaxError_FieldAlreadyExists, name));
                    BaseFieldStatement statement = ArrayFieldFactory.FromTypeName(current.Token.Location.Line, current.Token.Location.Column, type, name, count);
                    result.Add(statement);
                }
                else if (current.Term.Name.Equals(BinAnalyzerGrammar.CUSTOM_ARRAY_FIELD))
                {
                    string type = current.ChildNodes[0].Token.Text;
                    Expression count = ProcessExpression(current.ChildNodes[1]);
                    string name = current.ChildNodes[2].Token.Text;

                    if (result.Any(r => r is BaseFieldStatement fieldStatement && fieldStatement.Name.Equals(name)))
                        throw new SyntaxException(current.Token.Location.Line,
                            current.Token.Location.Column,
                            $"Field with name {name} already exists!",
                            string.Format(Strings.Message_SyntaxError_FieldAlreadyExists, name));

                    // Struct?

                    var structDef = definitions.StructDefinitions.FirstOrDefault(d => d.Name.Equals(type));
                    if (structDef != null)
                    {
                        StructArrayFieldStatement statement = new StructArrayFieldStatement(current.Token.Location.Line, current.Token.Location.Column, name, structDef, count);
                        result.Add(statement);
                        continue;
                    }
                    else
                        throw new SyntaxException(current.Token.Location.Line,
                            current.Token.Location.Column,
                            $"Cannot find type {type} !",
                            string.Format(Strings.Message_SyntaxError_CannotFindTypeName, type));

                }
                else if (current.Term.Name.Equals(BinAnalyzerGrammar.ASSIGNMENT))
                {
                    string name = current.ChildNodes[0].Token.Text;
                    Expression expression = ProcessExpression(current.ChildNodes[1]);

                    if (result.Any(r => r is BaseFieldStatement fieldStatement && fieldStatement.Name.Equals(name)))
                        if (result.Any(r => r is BaseFieldStatement fieldStatement && fieldStatement.Name.Equals(name)))
                            throw new SyntaxException(current.Token.Location.Line,
                                current.Token.Location.Column,
                                $"Field with name {name} already exists!",
                                string.Format(Strings.Message_SyntaxError_FieldAlreadyExists, name));

                    var statement = new AssignmentStatement(current.Token.Location.Line, current.Token.Location.Column, name, expression);
                    result.Add(statement);
                }
                else if (current.Term.Name.Equals(BinAnalyzerGrammar.SHOW_STATEMENT))
                {
                    Expression expression = ProcessExpression(current.ChildNodes[0]);
                    string alias = current.ChildNodes[1].Token.Text;

                    if (result.Any(r => r is BaseFieldStatement fieldStatement && fieldStatement.Name.Equals(alias)))
                        throw new SyntaxException(current.Token.Location.Line,
                            current.Token.Location.Column,
                            $"Field with name {alias} already exists!",
                            string.Format(Strings.Message_SyntaxError_FieldAlreadyExists, alias));

                    var statement = new ShowStatement(current.Token.Location.Line, current.Token.Location.Column, expression, alias);
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
