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

namespace Dev.Editor.BinAnalyzer
{
    public static class Compiler
    {
        private static ParseTree BuildParseTree(string source)
        {
            ParseTree parseTree = null;

            try
            {
                LanguageData language = new LanguageData(new BinAnalyzerGrammar());
                Parser parser = new Parser(language);
                parseTree = parser.Parse(source);
            }
            catch (Exception e)
            {
                throw new SyntaxErrorException("Syntax error while processing binary definition", e);
            }

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
                        throw new ParsingException("Structure with the same name already exists!");

                    structDefinitions.Add(structDef);
                }
                else
                    throw new ParsingException("Unsupported definition!");
            }

            return definitions;
        }

        private static BaseExpressionNode InternalProcessIntNumber(ParseTreeNode parseTreeNode)
        {
            var value = Int32.Parse(parseTreeNode.Token.Text);
            return new NumericNode(value);
        }

        private static BaseExpressionNode InternalProcessFloatNumber(ParseTreeNode parseTreeNode)
        {
            var value = float.Parse(parseTreeNode.Token.Text, CultureInfo.InvariantCulture);
            return new NumericNode(value);
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

            return new QualifiedIdentifierNode(identifier);
        }


        private static BaseExpressionNode InternalProcessComponent(ParseTreeNode parseTreeNode)
        {
            if (parseTreeNode.ChildNodes[0].Term.Equals(BinAnalyzerGrammar.INT_NUMBER))
                return InternalProcessIntNumber(parseTreeNode.ChildNodes[0]);
            if (parseTreeNode.ChildNodes[0].Term.Equals(BinAnalyzerGrammar.FLOAT_NUMBER))
                return InternalProcessFloatNumber(parseTreeNode.ChildNodes[0]);
            if (parseTreeNode.ChildNodes[0].Term.Equals(BinAnalyzerGrammar.QUALIFIED_IDENTIFIER))
                return InternalProcessQualifiedIdentifier(parseTreeNode.ChildNodes[0]);

            if (parseTreeNode.ChildNodes[0].Token.Text.Equals("(") &&
                parseTreeNode.ChildNodes[2].Token.Text.Equals(")"))
                return InternalProcessExpression(parseTreeNode.ChildNodes[1]);

            throw new ParsingException("Invalid operation");
        }

        private static BaseExpressionNode InternalProcessBitTerm(ParseTreeNode parseTreeNode)
        {
            if (parseTreeNode.ChildNodes[0].Term.Name.Equals(BinAnalyzerGrammar.COMPONENT))
                return InternalProcessComponent(parseTreeNode.ChildNodes[0]);

            if (parseTreeNode.ChildNodes[1].Token.Text.Equals("|"))
                return new BinaryOperator(InternalProcessBitTerm(parseTreeNode.ChildNodes[0]),
                    InternalProcessComponent(parseTreeNode.ChildNodes[2]),
                    BinaryOperation.Or);

            if (parseTreeNode.ChildNodes[1].Token.Text.Equals("&"))
                return new BinaryOperator(InternalProcessBitTerm(parseTreeNode.ChildNodes[0]),
                    InternalProcessComponent(parseTreeNode.ChildNodes[2]),
                    BinaryOperation.And);

            if (parseTreeNode.ChildNodes[1].Token.Text.Equals("^"))
                return new BinaryOperator(InternalProcessBitTerm(parseTreeNode.ChildNodes[0]),
                    InternalProcessComponent(parseTreeNode.ChildNodes[2]),
                    BinaryOperation.Xor);

            throw new ParsingException("Unsupported operation!");
        }

        private static BaseExpressionNode InternalProcessTerm(ParseTreeNode parseTreeNode)
        {
            if (parseTreeNode.ChildNodes[0].Term.Name.Equals(BinAnalyzerGrammar.BIT_TERM))
                return InternalProcessBitTerm(parseTreeNode.ChildNodes[0]);

            if (parseTreeNode.ChildNodes[1].Token.Text.Equals("*"))
                return new BinaryOperator(InternalProcessTerm(parseTreeNode.ChildNodes[0]),
                    InternalProcessBitTerm(parseTreeNode.ChildNodes[2]),
                    BinaryOperation.Multiply);

            if (parseTreeNode.ChildNodes[1].Token.Text.Equals("/"))
                return new BinaryOperator(InternalProcessTerm(parseTreeNode.ChildNodes[0]),
                    InternalProcessBitTerm(parseTreeNode.ChildNodes[2]),
                    BinaryOperation.Divide);

            if (parseTreeNode.ChildNodes[1].Token.Text.Equals("%"))
                return new BinaryOperator(InternalProcessTerm(parseTreeNode.ChildNodes[0]),
                    InternalProcessBitTerm(parseTreeNode.ChildNodes[2]),
                    BinaryOperation.Modulo);

            throw new ParsingException("Unsupported operation!");
        }

        private static BaseExpressionNode InternalProcessExpression(ParseTreeNode parseTreeNode)
        {
            if (parseTreeNode.ChildNodes[0].Term.Name.Equals(BinAnalyzerGrammar.TERM))
                return InternalProcessTerm(parseTreeNode.ChildNodes[0]);

            if (parseTreeNode.ChildNodes[1].Token.Text.Equals("+"))
                return new BinaryOperator(InternalProcessExpression(parseTreeNode.ChildNodes[0]),
                    InternalProcessTerm(parseTreeNode.ChildNodes[2]),
                    BinaryOperation.Add);

            if (parseTreeNode.ChildNodes[1].Token.Text.Equals("-"))
                return new BinaryOperator(InternalProcessExpression(parseTreeNode.ChildNodes[0]),
                    InternalProcessTerm(parseTreeNode.ChildNodes[2]),
                    BinaryOperation.Subtract);

            throw new ParsingException("Unsupported operation!");
        }

        private static Expression ProcessExpression(ParseTreeNode parseTreeNode)
        {
            return new Expression(InternalProcessExpression(parseTreeNode));
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
                        throw new ParsingException($"Field with name {name} already exists!");

                    BaseFieldStatement statement = FieldFactory.FromTypeName(type, name);
                    result.Add(statement);
                }
                else if (current.Term.Name.Equals(BinAnalyzerGrammar.CUSTOM_FIELD))
                {
                    string type = current.ChildNodes[0].Token.Text;
                    string name = current.ChildNodes[1].Token.Text;

                    if (result.Any(r => r is BaseFieldStatement fieldStatement && fieldStatement.Name.Equals(name)))
                        throw new ParsingException($"Field with name {name} already exists!");

                    // Struct?

                    var structDef = definitions.StructDefinitions.FirstOrDefault(d => d.Name.Equals(type));
                    if (structDef != null)
                    {
                        StructFieldStatement statement = new StructFieldStatement(name, structDef);
                        result.Add(statement);
                        continue;
                    }
                    else
                        throw new ParsingException("Not recognized type!");
                }
                else if (current.Term.Name.Equals(BinAnalyzerGrammar.BUILTIN_ARRAY_FIELD))
                {
                    string type = current.ChildNodes[0].ChildNodes[0].Token.Text;
                    Expression count = ProcessExpression(current.ChildNodes[1]);
                    string name = current.ChildNodes[2].Token.Text;

                    if (result.Any(r => r is BaseFieldStatement fieldStatement && fieldStatement.Name.Equals(name)))
                        throw new ParsingException($"Field with name {name} already exists!");

                    BaseFieldStatement statement = ArrayFieldFactory.FromTypeName(type, name, count);
                    result.Add(statement);
                }
                else if (current.Term.Name.Equals(BinAnalyzerGrammar.CUSTOM_ARRAY_FIELD))
                {
                    string type = current.ChildNodes[0].Token.Text;
                    Expression count = ProcessExpression(current.ChildNodes[1]);
                    string name = current.ChildNodes[2].Token.Text;

                    if (result.Any(r => r is BaseFieldStatement fieldStatement && fieldStatement.Name.Equals(name)))
                        throw new ParsingException($"Field with name {name} already exists!");

                    // Struct?

                    var structDef = definitions.StructDefinitions.FirstOrDefault(d => d.Name.Equals(type));
                    if (structDef != null)
                    {
                        StructArrayFieldStatement statement = new StructArrayFieldStatement(name, structDef, count);
                        result.Add(statement);
                        continue;
                    }
                    else
                        throw new ParsingException("Not recognized type!");

                }
                else if (current.Term.Name.Equals(BinAnalyzerGrammar.ASSIGNMENT))
                {
                    string name = current.ChildNodes[0].Token.Text;
                    Expression expression = ProcessExpression(current.ChildNodes[1]);

                    if (result.Any(r => r is BaseFieldStatement fieldStatement && fieldStatement.Name.Equals(name)))
                        throw new ParsingException($"Field with name {name} already exists!");

                    var statement = new AssignmentStatement(name, expression);
                    result.Add(statement);
                }
                else if (current.Term.Name.Equals(BinAnalyzerGrammar.SHOW_STATEMENT))
                {
                    Expression expression = ProcessExpression(current.ChildNodes[0]);
                    string alias = current.ChildNodes[1].Token.Text;

                    if (result.Any(r => r is BaseFieldStatement fieldStatement && fieldStatement.Name.Equals(alias)))
                        throw new ParsingException($"Field with name {alias} already exists!");

                    var statement = new ShowStatement(expression, alias);
                    result.Add(statement);
                }
                else
                    throw new ParsingException("Invalid statement!");
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
            Analyzer program = ProcessParseTree(parseTree);
            return program;
        }
    }
}
