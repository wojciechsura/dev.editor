using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.IO;
using Dev.Editor.BinAnalyzer.Exceptions;
using Dev.Editor.Resources;
using Dev.Editor.BinAnalyzer.AnalyzerDefinition.Definitions;
using Dev.Editor.BinAnalyzer.Grammar;
using Irony.Parsing;


namespace Dev.Editor.BinAnalyzer
{
    public static partial class Compiler
	{

	    private static BaseEnumDefinition BuildEnumDefinition(ParseTreeNode defNode)
		{
		    string name = defNode.ChildNodes[0].Token.Text;
            string type = defNode.ChildNodes[1].ChildNodes[0].Token.Text;

			switch (type)
			{
                case "sbyte":
		            return BuildSbyteEnumDefinition(name, defNode);
                case "short":
		            return BuildShortEnumDefinition(name, defNode);
                case "int":
		            return BuildIntEnumDefinition(name, defNode);
                case "long":
		            return BuildLongEnumDefinition(name, defNode);
                case "byte":
		            return BuildByteEnumDefinition(name, defNode);
                case "ushort":
		            return BuildUshortEnumDefinition(name, defNode);
                case "uint":
		            return BuildUintEnumDefinition(name, defNode);
                case "ulong":
		            return BuildUlongEnumDefinition(name, defNode);
                default:
				    throw new InvalidOperationException("Unsupported underlying enum type!");
			}
		}		

        private static BaseEnumDefinition BuildByteEnumDefinition(string name, ParseTreeNode defNode)
        {
            var items = new List<ByteEnumItem>();
            for (int i = 0; i < defNode.ChildNodes[2].ChildNodes.Count; i++)
            {
                var current = defNode.ChildNodes[2].ChildNodes[i];

                string itemName = current.ChildNodes[0].Token.Text;

                if (current.ChildNodes[1].Term.Name != BinAnalyzerGrammar.UINT_NUMBER)
                    throw new SyntaxException(current.ChildNodes[1].Token.Location.Line,
                        current.ChildNodes[1].Token.Location.Column,
                        "Unsigned enum should contain only unsigned items!",
                        Strings.Message_SyntaxError_InvalidUnsignedEnumItem);

                byte value;
                try
                {
                    value = byte.Parse(current.ChildNodes[1].Token.Text.Substring(0, current.ChildNodes[1].Token.Text.Length - 1));
                }
				catch (OverflowException)
				{
					throw new SyntaxException(current.ChildNodes[1].Token.Location.Line,
                        current.ChildNodes[1].Token.Location.Column,
                        "Unsigned value outside enum type range!",
                        string.Format(Strings.Message_SyntaxError_IntegerValueOutsideRange, itemName, current.ChildNodes[1].Token.Text, "byte"));
				}
                catch (Exception)
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

                var item = new ByteEnumItem(itemName, value);
                items.Add(item);
            }

            return new ByteEnumDefinition(name, items);			
        }

        private static BaseEnumDefinition BuildUshortEnumDefinition(string name, ParseTreeNode defNode)
        {
            var items = new List<UshortEnumItem>();
            for (int i = 0; i < defNode.ChildNodes[2].ChildNodes.Count; i++)
            {
                var current = defNode.ChildNodes[2].ChildNodes[i];

                string itemName = current.ChildNodes[0].Token.Text;

                if (current.ChildNodes[1].Term.Name != BinAnalyzerGrammar.UINT_NUMBER)
                    throw new SyntaxException(current.ChildNodes[1].Token.Location.Line,
                        current.ChildNodes[1].Token.Location.Column,
                        "Unsigned enum should contain only unsigned items!",
                        Strings.Message_SyntaxError_InvalidUnsignedEnumItem);

                ushort value;
                try
                {
                    value = ushort.Parse(current.ChildNodes[1].Token.Text.Substring(0, current.ChildNodes[1].Token.Text.Length - 1));
                }
				catch (OverflowException)
				{
					throw new SyntaxException(current.ChildNodes[1].Token.Location.Line,
                        current.ChildNodes[1].Token.Location.Column,
                        "Unsigned value outside enum type range!",
                        string.Format(Strings.Message_SyntaxError_IntegerValueOutsideRange, itemName, current.ChildNodes[1].Token.Text, "ushort"));
				}
                catch (Exception)
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

                var item = new UshortEnumItem(itemName, value);
                items.Add(item);
            }

            return new UshortEnumDefinition(name, items);			
        }

        private static BaseEnumDefinition BuildUintEnumDefinition(string name, ParseTreeNode defNode)
        {
            var items = new List<UintEnumItem>();
            for (int i = 0; i < defNode.ChildNodes[2].ChildNodes.Count; i++)
            {
                var current = defNode.ChildNodes[2].ChildNodes[i];

                string itemName = current.ChildNodes[0].Token.Text;

                if (current.ChildNodes[1].Term.Name != BinAnalyzerGrammar.UINT_NUMBER)
                    throw new SyntaxException(current.ChildNodes[1].Token.Location.Line,
                        current.ChildNodes[1].Token.Location.Column,
                        "Unsigned enum should contain only unsigned items!",
                        Strings.Message_SyntaxError_InvalidUnsignedEnumItem);

                uint value;
                try
                {
                    value = uint.Parse(current.ChildNodes[1].Token.Text.Substring(0, current.ChildNodes[1].Token.Text.Length - 1));
                }
				catch (OverflowException)
				{
					throw new SyntaxException(current.ChildNodes[1].Token.Location.Line,
                        current.ChildNodes[1].Token.Location.Column,
                        "Unsigned value outside enum type range!",
                        string.Format(Strings.Message_SyntaxError_IntegerValueOutsideRange, itemName, current.ChildNodes[1].Token.Text, "uint"));
				}
                catch (Exception)
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

                var item = new UintEnumItem(itemName, value);
                items.Add(item);
            }

            return new UintEnumDefinition(name, items);			
        }

        private static BaseEnumDefinition BuildUlongEnumDefinition(string name, ParseTreeNode defNode)
        {
            var items = new List<UlongEnumItem>();
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
				catch (OverflowException)
				{
					throw new SyntaxException(current.ChildNodes[1].Token.Location.Line,
                        current.ChildNodes[1].Token.Location.Column,
                        "Unsigned value outside enum type range!",
                        string.Format(Strings.Message_SyntaxError_IntegerValueOutsideRange, itemName, current.ChildNodes[1].Token.Text, "ulong"));
				}
                catch (Exception)
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

                var item = new UlongEnumItem(itemName, value);
                items.Add(item);
            }

            return new UlongEnumDefinition(name, items);			
        }


        private static BaseEnumDefinition BuildSbyteEnumDefinition(string name, ParseTreeNode defNode)
        {
            var items = new List<SbyteEnumItem>();
            for (int i = 0; i < defNode.ChildNodes[2].ChildNodes.Count; i++)
            {
                var current = defNode.ChildNodes[2].ChildNodes[i];

                string itemName = current.ChildNodes[0].Token.Text;

                if (current.ChildNodes[1].Term.Name != BinAnalyzerGrammar.INT_NUMBER)
                    throw new SyntaxException(current.ChildNodes[1].Token.Location.Line,
                        current.ChildNodes[1].Token.Location.Column,
                        "Unsigned enum should contain only signed items!",
                        Strings.Message_SyntaxError_InvalidUnsignedEnumItem);

                sbyte value;
                try
                {
                    value = sbyte.Parse(current.ChildNodes[1].Token.Text);
                }
				catch (OverflowException)
				{
					throw new SyntaxException(current.ChildNodes[1].Token.Location.Line,
                        current.ChildNodes[1].Token.Location.Column,
                        "Unsigned value outside enum type range!",
                        string.Format(Strings.Message_SyntaxError_IntegerValueOutsideRange, itemName, current.ChildNodes[1].Token.Text, "sbyte"));
				}
                catch (Exception)
                {
                    throw new SyntaxException(current.ChildNodes[1].Token.Location.Line,
                        current.ChildNodes[1].Token.Location.Column,
                        "Invalid unsigned integer value",
                        string.Format(Strings.Message_SyntaxError_InvalidIntegerValue, current.ChildNodes[1].Token.Text));
                }

                if (items.Any(ei => ei.Value == value))
                    throw new SyntaxException(current.ChildNodes[1].Token.Location.Line,
                        current.ChildNodes[1].Token.Location.Column,
                        "Enum item with this value already exists!",
                        string.Format(Strings.Message_SyntaxError_EnumItemValueDuplicated, value));

                var item = new SbyteEnumItem(itemName, value);
                items.Add(item);
            }

            return new SbyteEnumDefinition(name, items);			
        }

        private static BaseEnumDefinition BuildShortEnumDefinition(string name, ParseTreeNode defNode)
        {
            var items = new List<ShortEnumItem>();
            for (int i = 0; i < defNode.ChildNodes[2].ChildNodes.Count; i++)
            {
                var current = defNode.ChildNodes[2].ChildNodes[i];

                string itemName = current.ChildNodes[0].Token.Text;

                if (current.ChildNodes[1].Term.Name != BinAnalyzerGrammar.INT_NUMBER)
                    throw new SyntaxException(current.ChildNodes[1].Token.Location.Line,
                        current.ChildNodes[1].Token.Location.Column,
                        "Unsigned enum should contain only signed items!",
                        Strings.Message_SyntaxError_InvalidUnsignedEnumItem);

                short value;
                try
                {
                    value = short.Parse(current.ChildNodes[1].Token.Text);
                }
				catch (OverflowException)
				{
					throw new SyntaxException(current.ChildNodes[1].Token.Location.Line,
                        current.ChildNodes[1].Token.Location.Column,
                        "Unsigned value outside enum type range!",
                        string.Format(Strings.Message_SyntaxError_IntegerValueOutsideRange, itemName, current.ChildNodes[1].Token.Text, "short"));
				}
                catch (Exception)
                {
                    throw new SyntaxException(current.ChildNodes[1].Token.Location.Line,
                        current.ChildNodes[1].Token.Location.Column,
                        "Invalid unsigned integer value",
                        string.Format(Strings.Message_SyntaxError_InvalidIntegerValue, current.ChildNodes[1].Token.Text));
                }

                if (items.Any(ei => ei.Value == value))
                    throw new SyntaxException(current.ChildNodes[1].Token.Location.Line,
                        current.ChildNodes[1].Token.Location.Column,
                        "Enum item with this value already exists!",
                        string.Format(Strings.Message_SyntaxError_EnumItemValueDuplicated, value));

                var item = new ShortEnumItem(itemName, value);
                items.Add(item);
            }

            return new ShortEnumDefinition(name, items);			
        }

        private static BaseEnumDefinition BuildIntEnumDefinition(string name, ParseTreeNode defNode)
        {
            var items = new List<IntEnumItem>();
            for (int i = 0; i < defNode.ChildNodes[2].ChildNodes.Count; i++)
            {
                var current = defNode.ChildNodes[2].ChildNodes[i];

                string itemName = current.ChildNodes[0].Token.Text;

                if (current.ChildNodes[1].Term.Name != BinAnalyzerGrammar.INT_NUMBER)
                    throw new SyntaxException(current.ChildNodes[1].Token.Location.Line,
                        current.ChildNodes[1].Token.Location.Column,
                        "Unsigned enum should contain only signed items!",
                        Strings.Message_SyntaxError_InvalidUnsignedEnumItem);

                int value;
                try
                {
                    value = int.Parse(current.ChildNodes[1].Token.Text);
                }
				catch (OverflowException)
				{
					throw new SyntaxException(current.ChildNodes[1].Token.Location.Line,
                        current.ChildNodes[1].Token.Location.Column,
                        "Unsigned value outside enum type range!",
                        string.Format(Strings.Message_SyntaxError_IntegerValueOutsideRange, itemName, current.ChildNodes[1].Token.Text, "int"));
				}
                catch (Exception)
                {
                    throw new SyntaxException(current.ChildNodes[1].Token.Location.Line,
                        current.ChildNodes[1].Token.Location.Column,
                        "Invalid unsigned integer value",
                        string.Format(Strings.Message_SyntaxError_InvalidIntegerValue, current.ChildNodes[1].Token.Text));
                }

                if (items.Any(ei => ei.Value == value))
                    throw new SyntaxException(current.ChildNodes[1].Token.Location.Line,
                        current.ChildNodes[1].Token.Location.Column,
                        "Enum item with this value already exists!",
                        string.Format(Strings.Message_SyntaxError_EnumItemValueDuplicated, value));

                var item = new IntEnumItem(itemName, value);
                items.Add(item);
            }

            return new IntEnumDefinition(name, items);			
        }

        private static BaseEnumDefinition BuildLongEnumDefinition(string name, ParseTreeNode defNode)
        {
            var items = new List<LongEnumItem>();
            for (int i = 0; i < defNode.ChildNodes[2].ChildNodes.Count; i++)
            {
                var current = defNode.ChildNodes[2].ChildNodes[i];

                string itemName = current.ChildNodes[0].Token.Text;

                if (current.ChildNodes[1].Term.Name != BinAnalyzerGrammar.INT_NUMBER)
                    throw new SyntaxException(current.ChildNodes[1].Token.Location.Line,
                        current.ChildNodes[1].Token.Location.Column,
                        "Unsigned enum should contain only signed items!",
                        Strings.Message_SyntaxError_InvalidUnsignedEnumItem);

                long value;
                try
                {
                    value = long.Parse(current.ChildNodes[1].Token.Text);
                }
				catch (OverflowException)
				{
					throw new SyntaxException(current.ChildNodes[1].Token.Location.Line,
                        current.ChildNodes[1].Token.Location.Column,
                        "Unsigned value outside enum type range!",
                        string.Format(Strings.Message_SyntaxError_IntegerValueOutsideRange, itemName, current.ChildNodes[1].Token.Text, "long"));
				}
                catch (Exception)
                {
                    throw new SyntaxException(current.ChildNodes[1].Token.Location.Line,
                        current.ChildNodes[1].Token.Location.Column,
                        "Invalid unsigned integer value",
                        string.Format(Strings.Message_SyntaxError_InvalidIntegerValue, current.ChildNodes[1].Token.Text));
                }

                if (items.Any(ei => ei.Value == value))
                    throw new SyntaxException(current.ChildNodes[1].Token.Location.Line,
                        current.ChildNodes[1].Token.Location.Column,
                        "Enum item with this value already exists!",
                        string.Format(Strings.Message_SyntaxError_EnumItemValueDuplicated, value));

                var item = new LongEnumItem(itemName, value);
                items.Add(item);
            }

            return new LongEnumDefinition(name, items);			
        }

    }
}