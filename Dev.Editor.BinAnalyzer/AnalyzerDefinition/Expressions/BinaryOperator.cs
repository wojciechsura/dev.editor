﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dev.Editor.BinAnalyzer.AnalyzerDefinition.Expressions;
using Dev.Editor.BinAnalyzer.AnalyzerDefinition.Statements;
using Dev.Editor.BinAnalyzer.Exceptions;
using Dev.Editor.Resources;

namespace Dev.Editor.BinAnalyzer.AnalyzerDefinition.Expressions
{
    class BinaryOperator : BaseExpressionNode
    {
        private readonly BaseExpressionNode left;
        private readonly BaseExpressionNode right;
        private readonly BinaryOperation operation;

        public BinaryOperator(int line, int column, BaseExpressionNode left, BaseExpressionNode right, BinaryOperation operation)
            : base(line, column)
        {
            this.left = left;
            this.right = right;
            this.operation = operation;
        }

        public override dynamic Eval(Scope scope)
        {
            try
            {
                dynamic leftValue = left.Eval(scope);
                dynamic rightValue = right.Eval(scope);

                switch (operation)
                {
                    case BinaryOperation.Add:
                        return leftValue + rightValue;
                    case BinaryOperation.Subtract:
                        return leftValue - rightValue;
                    case BinaryOperation.Multiply:
                        return leftValue * rightValue;
                    case BinaryOperation.Divide:
                        return leftValue / rightValue;
                    case BinaryOperation.Modulo:
                        return leftValue % rightValue;
                    case BinaryOperation.BitAnd:
                        return leftValue & rightValue;
                    case BinaryOperation.BitOr:
                        return leftValue | rightValue;
                    case BinaryOperation.BitXor:
                        return leftValue ^ rightValue;
                    case BinaryOperation.LessThan:
                        return leftValue < rightValue;
                    case BinaryOperation.LessThanOrEqual:
                        return leftValue <= rightValue;
                    case BinaryOperation.Equal:
                        return leftValue == rightValue;
                    case BinaryOperation.Inequal:
                        return leftValue != rightValue;
                    case BinaryOperation.GreaterThanOrEqual:
                        return leftValue >= rightValue;
                    case BinaryOperation.GreaterThan:
                        return leftValue > rightValue;
                    case BinaryOperation.LogicAnd:
                        return leftValue && rightValue;
                    case BinaryOperation.LogicOr:
                        return leftValue || rightValue;
                    case BinaryOperation.LogicXor:
                        return (leftValue && !rightValue) || (!leftValue && rightValue);
                    default:
                        throw new InvalidEnumArgumentException("Unsupported operation!");
                }
            }
            catch (BaseLocalizedException e)
            {
                throw new EvalException(Line, Column, "Failed to eval binary operator", String.Format(Strings.Message_EvalError_FailedToEvalBinaryOperator, e.LocalizedErrorMessage));
            }
            catch (Exception e)
            {
                throw new EvalException(Line, Column, "Failed to eval binary operator", String.Format(Strings.Message_EvalError_FailedToEvalBinaryOperator, e.Message));
            }
        }

        public BaseExpressionNode Left => left;
        public BaseExpressionNode Right => right;
        public BinaryOperation Operation => operation;
    }
}
