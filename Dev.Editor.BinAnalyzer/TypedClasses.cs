using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.IO;
using Dev.Editor.BinAnalyzer.Data;
using Dev.Editor.BinAnalyzer.AnalyzerDefinition.Expressions;
using Dev.Editor.BinAnalyzer.Exceptions;
using Dev.Editor.Resources;
using Dev.Editor.BinAnalyzer.AnalyzerDefinition.Definitions;


namespace Dev.Editor.BinAnalyzer.AnalyzerDefinition.Definitions
{
    class SbyteEnumItem : BaseEnumItem 
	{
	    public SbyteEnumItem(string name, sbyte value)
		    : base(name)
        {
            Value = value;
        }

        public sbyte Value { get; }
	}

    class SbyteEnumDefinition : BaseEnumDefinition
    {
        private readonly List<SbyteEnumItem> items;

        public SbyteEnumDefinition(string name, List<SbyteEnumItem> items) 
            : base(name, "sbyte")
        {
            this.items = items;
        }

		public override BaseData GenerateEnumData(string field, string memberName)
		{
			var member = items.FirstOrDefault(i => i.Name.Equals(memberName));

			if (member == null)
				throw new ArgumentException("memberName");

			return new SbyteEnumData(field, Name, member.Value, member.Name);
		}

        public List<SbyteEnumItem> Items => items;
    }

    class ShortEnumItem : BaseEnumItem 
	{
	    public ShortEnumItem(string name, short value)
		    : base(name)
        {
            Value = value;
        }

        public short Value { get; }
	}

    class ShortEnumDefinition : BaseEnumDefinition
    {
        private readonly List<ShortEnumItem> items;

        public ShortEnumDefinition(string name, List<ShortEnumItem> items) 
            : base(name, "short")
        {
            this.items = items;
        }

		public override BaseData GenerateEnumData(string field, string memberName)
		{
			var member = items.FirstOrDefault(i => i.Name.Equals(memberName));

			if (member == null)
				throw new ArgumentException("memberName");

			return new ShortEnumData(field, Name, member.Value, member.Name);
		}

        public List<ShortEnumItem> Items => items;
    }

    class IntEnumItem : BaseEnumItem 
	{
	    public IntEnumItem(string name, int value)
		    : base(name)
        {
            Value = value;
        }

        public int Value { get; }
	}

    class IntEnumDefinition : BaseEnumDefinition
    {
        private readonly List<IntEnumItem> items;

        public IntEnumDefinition(string name, List<IntEnumItem> items) 
            : base(name, "int")
        {
            this.items = items;
        }

		public override BaseData GenerateEnumData(string field, string memberName)
		{
			var member = items.FirstOrDefault(i => i.Name.Equals(memberName));

			if (member == null)
				throw new ArgumentException("memberName");

			return new IntEnumData(field, Name, member.Value, member.Name);
		}

        public List<IntEnumItem> Items => items;
    }

    class LongEnumItem : BaseEnumItem 
	{
	    public LongEnumItem(string name, long value)
		    : base(name)
        {
            Value = value;
        }

        public long Value { get; }
	}

    class LongEnumDefinition : BaseEnumDefinition
    {
        private readonly List<LongEnumItem> items;

        public LongEnumDefinition(string name, List<LongEnumItem> items) 
            : base(name, "long")
        {
            this.items = items;
        }

		public override BaseData GenerateEnumData(string field, string memberName)
		{
			var member = items.FirstOrDefault(i => i.Name.Equals(memberName));

			if (member == null)
				throw new ArgumentException("memberName");

			return new LongEnumData(field, Name, member.Value, member.Name);
		}

        public List<LongEnumItem> Items => items;
    }

    class ByteEnumItem : BaseEnumItem 
	{
	    public ByteEnumItem(string name, byte value)
		    : base(name)
        {
            Value = value;
        }

        public byte Value { get; }
	}

    class ByteEnumDefinition : BaseEnumDefinition
    {
        private readonly List<ByteEnumItem> items;

        public ByteEnumDefinition(string name, List<ByteEnumItem> items) 
            : base(name, "byte")
        {
            this.items = items;
        }

		public override BaseData GenerateEnumData(string field, string memberName)
		{
			var member = items.FirstOrDefault(i => i.Name.Equals(memberName));

			if (member == null)
				throw new ArgumentException("memberName");

			return new ByteEnumData(field, Name, member.Value, member.Name);
		}

        public List<ByteEnumItem> Items => items;
    }

    class UshortEnumItem : BaseEnumItem 
	{
	    public UshortEnumItem(string name, ushort value)
		    : base(name)
        {
            Value = value;
        }

        public ushort Value { get; }
	}

    class UshortEnumDefinition : BaseEnumDefinition
    {
        private readonly List<UshortEnumItem> items;

        public UshortEnumDefinition(string name, List<UshortEnumItem> items) 
            : base(name, "ushort")
        {
            this.items = items;
        }

		public override BaseData GenerateEnumData(string field, string memberName)
		{
			var member = items.FirstOrDefault(i => i.Name.Equals(memberName));

			if (member == null)
				throw new ArgumentException("memberName");

			return new UshortEnumData(field, Name, member.Value, member.Name);
		}

        public List<UshortEnumItem> Items => items;
    }

    class UintEnumItem : BaseEnumItem 
	{
	    public UintEnumItem(string name, uint value)
		    : base(name)
        {
            Value = value;
        }

        public uint Value { get; }
	}

    class UintEnumDefinition : BaseEnumDefinition
    {
        private readonly List<UintEnumItem> items;

        public UintEnumDefinition(string name, List<UintEnumItem> items) 
            : base(name, "uint")
        {
            this.items = items;
        }

		public override BaseData GenerateEnumData(string field, string memberName)
		{
			var member = items.FirstOrDefault(i => i.Name.Equals(memberName));

			if (member == null)
				throw new ArgumentException("memberName");

			return new UintEnumData(field, Name, member.Value, member.Name);
		}

        public List<UintEnumItem> Items => items;
    }

    class UlongEnumItem : BaseEnumItem 
	{
	    public UlongEnumItem(string name, ulong value)
		    : base(name)
        {
            Value = value;
        }

        public ulong Value { get; }
	}

    class UlongEnumDefinition : BaseEnumDefinition
    {
        private readonly List<UlongEnumItem> items;

        public UlongEnumDefinition(string name, List<UlongEnumItem> items) 
            : base(name, "ulong")
        {
            this.items = items;
        }

		public override BaseData GenerateEnumData(string field, string memberName)
		{
			var member = items.FirstOrDefault(i => i.Name.Equals(memberName));

			if (member == null)
				throw new ArgumentException("memberName");

			return new UlongEnumData(field, Name, member.Value, member.Name);
		}

        public List<UlongEnumItem> Items => items;
    }

}

namespace Dev.Editor.BinAnalyzer.AnalyzerDefinition.Statements
{
    class SbyteFieldStatement : BaseFieldStatement
    {
        public SbyteFieldStatement(int line, int column, string name)
            : base(line, column, name)
        {
            
        }

        internal override void Read(BinaryReader reader, List<BaseData> result, Scope scope)
        {
            try
            {
                if (reader.BaseStream.Position + sizeof(sbyte) >= reader.BaseStream.Length)
                    throw new AnalysisException(Line, Column, "Unexpected end of stream", Strings.Message_AnalysisError_UnexpectedEndOfStream);

                sbyte value = reader.ReadSByte();

                var data = new SbyteData(name, value);
                result.Add(data);
                scope.AddContent(name, data);
            }
            catch (BaseLocalizedException e)
            {
                throw new AnalysisException(Line, Column, "Failed to load field!", string.Format(Strings.Message_AnalysisError_FailedToReadField, name, e.LocalizedErrorMessage));
            }
            catch (Exception e)
            {
                throw new AnalysisException(Line, Column, "Failed to load field!", string.Format(Strings.Message_AnalysisError_FailedToReadField, name, e.Message));
            }
        }
    }

    class ShortFieldStatement : BaseFieldStatement
    {
        public ShortFieldStatement(int line, int column, string name)
            : base(line, column, name)
        {
            
        }

        internal override void Read(BinaryReader reader, List<BaseData> result, Scope scope)
        {
            try
            {
                if (reader.BaseStream.Position + sizeof(short) >= reader.BaseStream.Length)
                    throw new AnalysisException(Line, Column, "Unexpected end of stream", Strings.Message_AnalysisError_UnexpectedEndOfStream);

                short value = reader.ReadInt16();

                var data = new ShortData(name, value);
                result.Add(data);
                scope.AddContent(name, data);
            }
            catch (BaseLocalizedException e)
            {
                throw new AnalysisException(Line, Column, "Failed to load field!", string.Format(Strings.Message_AnalysisError_FailedToReadField, name, e.LocalizedErrorMessage));
            }
            catch (Exception e)
            {
                throw new AnalysisException(Line, Column, "Failed to load field!", string.Format(Strings.Message_AnalysisError_FailedToReadField, name, e.Message));
            }
        }
    }

    class IntFieldStatement : BaseFieldStatement
    {
        public IntFieldStatement(int line, int column, string name)
            : base(line, column, name)
        {
            
        }

        internal override void Read(BinaryReader reader, List<BaseData> result, Scope scope)
        {
            try
            {
                if (reader.BaseStream.Position + sizeof(int) >= reader.BaseStream.Length)
                    throw new AnalysisException(Line, Column, "Unexpected end of stream", Strings.Message_AnalysisError_UnexpectedEndOfStream);

                int value = reader.ReadInt32();

                var data = new IntData(name, value);
                result.Add(data);
                scope.AddContent(name, data);
            }
            catch (BaseLocalizedException e)
            {
                throw new AnalysisException(Line, Column, "Failed to load field!", string.Format(Strings.Message_AnalysisError_FailedToReadField, name, e.LocalizedErrorMessage));
            }
            catch (Exception e)
            {
                throw new AnalysisException(Line, Column, "Failed to load field!", string.Format(Strings.Message_AnalysisError_FailedToReadField, name, e.Message));
            }
        }
    }

    class LongFieldStatement : BaseFieldStatement
    {
        public LongFieldStatement(int line, int column, string name)
            : base(line, column, name)
        {
            
        }

        internal override void Read(BinaryReader reader, List<BaseData> result, Scope scope)
        {
            try
            {
                if (reader.BaseStream.Position + sizeof(long) >= reader.BaseStream.Length)
                    throw new AnalysisException(Line, Column, "Unexpected end of stream", Strings.Message_AnalysisError_UnexpectedEndOfStream);

                long value = reader.ReadInt64();

                var data = new LongData(name, value);
                result.Add(data);
                scope.AddContent(name, data);
            }
            catch (BaseLocalizedException e)
            {
                throw new AnalysisException(Line, Column, "Failed to load field!", string.Format(Strings.Message_AnalysisError_FailedToReadField, name, e.LocalizedErrorMessage));
            }
            catch (Exception e)
            {
                throw new AnalysisException(Line, Column, "Failed to load field!", string.Format(Strings.Message_AnalysisError_FailedToReadField, name, e.Message));
            }
        }
    }

    class ByteFieldStatement : BaseFieldStatement
    {
        public ByteFieldStatement(int line, int column, string name)
            : base(line, column, name)
        {
            
        }

        internal override void Read(BinaryReader reader, List<BaseData> result, Scope scope)
        {
            try
            {
                if (reader.BaseStream.Position + sizeof(byte) >= reader.BaseStream.Length)
                    throw new AnalysisException(Line, Column, "Unexpected end of stream", Strings.Message_AnalysisError_UnexpectedEndOfStream);

                byte value = reader.ReadByte();

                var data = new ByteData(name, value);
                result.Add(data);
                scope.AddContent(name, data);
            }
            catch (BaseLocalizedException e)
            {
                throw new AnalysisException(Line, Column, "Failed to load field!", string.Format(Strings.Message_AnalysisError_FailedToReadField, name, e.LocalizedErrorMessage));
            }
            catch (Exception e)
            {
                throw new AnalysisException(Line, Column, "Failed to load field!", string.Format(Strings.Message_AnalysisError_FailedToReadField, name, e.Message));
            }
        }
    }

    class UshortFieldStatement : BaseFieldStatement
    {
        public UshortFieldStatement(int line, int column, string name)
            : base(line, column, name)
        {
            
        }

        internal override void Read(BinaryReader reader, List<BaseData> result, Scope scope)
        {
            try
            {
                if (reader.BaseStream.Position + sizeof(ushort) >= reader.BaseStream.Length)
                    throw new AnalysisException(Line, Column, "Unexpected end of stream", Strings.Message_AnalysisError_UnexpectedEndOfStream);

                ushort value = reader.ReadUInt16();

                var data = new UshortData(name, value);
                result.Add(data);
                scope.AddContent(name, data);
            }
            catch (BaseLocalizedException e)
            {
                throw new AnalysisException(Line, Column, "Failed to load field!", string.Format(Strings.Message_AnalysisError_FailedToReadField, name, e.LocalizedErrorMessage));
            }
            catch (Exception e)
            {
                throw new AnalysisException(Line, Column, "Failed to load field!", string.Format(Strings.Message_AnalysisError_FailedToReadField, name, e.Message));
            }
        }
    }

    class UintFieldStatement : BaseFieldStatement
    {
        public UintFieldStatement(int line, int column, string name)
            : base(line, column, name)
        {
            
        }

        internal override void Read(BinaryReader reader, List<BaseData> result, Scope scope)
        {
            try
            {
                if (reader.BaseStream.Position + sizeof(uint) >= reader.BaseStream.Length)
                    throw new AnalysisException(Line, Column, "Unexpected end of stream", Strings.Message_AnalysisError_UnexpectedEndOfStream);

                uint value = reader.ReadUInt32();

                var data = new UintData(name, value);
                result.Add(data);
                scope.AddContent(name, data);
            }
            catch (BaseLocalizedException e)
            {
                throw new AnalysisException(Line, Column, "Failed to load field!", string.Format(Strings.Message_AnalysisError_FailedToReadField, name, e.LocalizedErrorMessage));
            }
            catch (Exception e)
            {
                throw new AnalysisException(Line, Column, "Failed to load field!", string.Format(Strings.Message_AnalysisError_FailedToReadField, name, e.Message));
            }
        }
    }

    class UlongFieldStatement : BaseFieldStatement
    {
        public UlongFieldStatement(int line, int column, string name)
            : base(line, column, name)
        {
            
        }

        internal override void Read(BinaryReader reader, List<BaseData> result, Scope scope)
        {
            try
            {
                if (reader.BaseStream.Position + sizeof(ulong) >= reader.BaseStream.Length)
                    throw new AnalysisException(Line, Column, "Unexpected end of stream", Strings.Message_AnalysisError_UnexpectedEndOfStream);

                ulong value = reader.ReadUInt64();

                var data = new UlongData(name, value);
                result.Add(data);
                scope.AddContent(name, data);
            }
            catch (BaseLocalizedException e)
            {
                throw new AnalysisException(Line, Column, "Failed to load field!", string.Format(Strings.Message_AnalysisError_FailedToReadField, name, e.LocalizedErrorMessage));
            }
            catch (Exception e)
            {
                throw new AnalysisException(Line, Column, "Failed to load field!", string.Format(Strings.Message_AnalysisError_FailedToReadField, name, e.Message));
            }
        }
    }

    class FloatFieldStatement : BaseFieldStatement
    {
        public FloatFieldStatement(int line, int column, string name)
            : base(line, column, name)
        {
            
        }

        internal override void Read(BinaryReader reader, List<BaseData> result, Scope scope)
        {
            try
            {
                if (reader.BaseStream.Position + sizeof(float) >= reader.BaseStream.Length)
                    throw new AnalysisException(Line, Column, "Unexpected end of stream", Strings.Message_AnalysisError_UnexpectedEndOfStream);

                float value = reader.ReadSingle();

                var data = new FloatData(name, value);
                result.Add(data);
                scope.AddContent(name, data);
            }
            catch (BaseLocalizedException e)
            {
                throw new AnalysisException(Line, Column, "Failed to load field!", string.Format(Strings.Message_AnalysisError_FailedToReadField, name, e.LocalizedErrorMessage));
            }
            catch (Exception e)
            {
                throw new AnalysisException(Line, Column, "Failed to load field!", string.Format(Strings.Message_AnalysisError_FailedToReadField, name, e.Message));
            }
        }
    }

    class DoubleFieldStatement : BaseFieldStatement
    {
        public DoubleFieldStatement(int line, int column, string name)
            : base(line, column, name)
        {
            
        }

        internal override void Read(BinaryReader reader, List<BaseData> result, Scope scope)
        {
            try
            {
                if (reader.BaseStream.Position + sizeof(double) >= reader.BaseStream.Length)
                    throw new AnalysisException(Line, Column, "Unexpected end of stream", Strings.Message_AnalysisError_UnexpectedEndOfStream);

                double value = reader.ReadDouble();

                var data = new DoubleData(name, value);
                result.Add(data);
                scope.AddContent(name, data);
            }
            catch (BaseLocalizedException e)
            {
                throw new AnalysisException(Line, Column, "Failed to load field!", string.Format(Strings.Message_AnalysisError_FailedToReadField, name, e.LocalizedErrorMessage));
            }
            catch (Exception e)
            {
                throw new AnalysisException(Line, Column, "Failed to load field!", string.Format(Strings.Message_AnalysisError_FailedToReadField, name, e.Message));
            }
        }
    }

    class SbyteArrayFieldStatement : BaseFieldStatement
    {
        private readonly Expression count;

        public SbyteArrayFieldStatement(int line, int column, string name, Expression count)
            : base(line, column, name)
        {
            this.count = count;
        }

        internal override void Read(BinaryReader reader, List<BaseData> result, Scope scope)
        {
            try
            {
                dynamic countValue = count.Eval(scope);
                int countInt = (int)countValue;
                
                if (reader.BaseStream.Position + sizeof(sbyte) * countInt >= reader.BaseStream.Length)
                    throw new AnalysisException(Line, Column, "Unexpected end of stream", Strings.Message_AnalysisError_UnexpectedEndOfStream);

                var data = new SbyteData[countInt];
                for (int i = 0; i < countInt; i++)
                {
                    sbyte value = reader.ReadSByte();
                    var element = new SbyteData(i.ToString(), value);
                    data[i] = element;
                }

                var item = new ArrayData<SbyteData>(name, "Sbyte", data);

                result.Add(item);
                scope.AddContent(name, item);
            }
            catch (BaseLocalizedException e)
            {
                throw new AnalysisException(Line, Column, "Failed to load field!", string.Format(Strings.Message_AnalysisError_FailedToReadField, name, e.LocalizedErrorMessage));
            }
            catch (Exception e)
            {
                throw new AnalysisException(Line, Column, "Failed to load field!", string.Format(Strings.Message_AnalysisError_FailedToReadField, name, e.Message));
            }
        }
    }

    class ShortArrayFieldStatement : BaseFieldStatement
    {
        private readonly Expression count;

        public ShortArrayFieldStatement(int line, int column, string name, Expression count)
            : base(line, column, name)
        {
            this.count = count;
        }

        internal override void Read(BinaryReader reader, List<BaseData> result, Scope scope)
        {
            try
            {
                dynamic countValue = count.Eval(scope);
                int countInt = (int)countValue;
                
                if (reader.BaseStream.Position + sizeof(short) * countInt >= reader.BaseStream.Length)
                    throw new AnalysisException(Line, Column, "Unexpected end of stream", Strings.Message_AnalysisError_UnexpectedEndOfStream);

                var data = new ShortData[countInt];
                for (int i = 0; i < countInt; i++)
                {
                    short value = reader.ReadInt16();
                    var element = new ShortData(i.ToString(), value);
                    data[i] = element;
                }

                var item = new ArrayData<ShortData>(name, "Short", data);

                result.Add(item);
                scope.AddContent(name, item);
            }
            catch (BaseLocalizedException e)
            {
                throw new AnalysisException(Line, Column, "Failed to load field!", string.Format(Strings.Message_AnalysisError_FailedToReadField, name, e.LocalizedErrorMessage));
            }
            catch (Exception e)
            {
                throw new AnalysisException(Line, Column, "Failed to load field!", string.Format(Strings.Message_AnalysisError_FailedToReadField, name, e.Message));
            }
        }
    }

    class IntArrayFieldStatement : BaseFieldStatement
    {
        private readonly Expression count;

        public IntArrayFieldStatement(int line, int column, string name, Expression count)
            : base(line, column, name)
        {
            this.count = count;
        }

        internal override void Read(BinaryReader reader, List<BaseData> result, Scope scope)
        {
            try
            {
                dynamic countValue = count.Eval(scope);
                int countInt = (int)countValue;
                
                if (reader.BaseStream.Position + sizeof(int) * countInt >= reader.BaseStream.Length)
                    throw new AnalysisException(Line, Column, "Unexpected end of stream", Strings.Message_AnalysisError_UnexpectedEndOfStream);

                var data = new IntData[countInt];
                for (int i = 0; i < countInt; i++)
                {
                    int value = reader.ReadInt32();
                    var element = new IntData(i.ToString(), value);
                    data[i] = element;
                }

                var item = new ArrayData<IntData>(name, "Int", data);

                result.Add(item);
                scope.AddContent(name, item);
            }
            catch (BaseLocalizedException e)
            {
                throw new AnalysisException(Line, Column, "Failed to load field!", string.Format(Strings.Message_AnalysisError_FailedToReadField, name, e.LocalizedErrorMessage));
            }
            catch (Exception e)
            {
                throw new AnalysisException(Line, Column, "Failed to load field!", string.Format(Strings.Message_AnalysisError_FailedToReadField, name, e.Message));
            }
        }
    }

    class LongArrayFieldStatement : BaseFieldStatement
    {
        private readonly Expression count;

        public LongArrayFieldStatement(int line, int column, string name, Expression count)
            : base(line, column, name)
        {
            this.count = count;
        }

        internal override void Read(BinaryReader reader, List<BaseData> result, Scope scope)
        {
            try
            {
                dynamic countValue = count.Eval(scope);
                int countInt = (int)countValue;
                
                if (reader.BaseStream.Position + sizeof(long) * countInt >= reader.BaseStream.Length)
                    throw new AnalysisException(Line, Column, "Unexpected end of stream", Strings.Message_AnalysisError_UnexpectedEndOfStream);

                var data = new LongData[countInt];
                for (int i = 0; i < countInt; i++)
                {
                    long value = reader.ReadInt64();
                    var element = new LongData(i.ToString(), value);
                    data[i] = element;
                }

                var item = new ArrayData<LongData>(name, "Long", data);

                result.Add(item);
                scope.AddContent(name, item);
            }
            catch (BaseLocalizedException e)
            {
                throw new AnalysisException(Line, Column, "Failed to load field!", string.Format(Strings.Message_AnalysisError_FailedToReadField, name, e.LocalizedErrorMessage));
            }
            catch (Exception e)
            {
                throw new AnalysisException(Line, Column, "Failed to load field!", string.Format(Strings.Message_AnalysisError_FailedToReadField, name, e.Message));
            }
        }
    }

    class ByteArrayFieldStatement : BaseFieldStatement
    {
        private readonly Expression count;

        public ByteArrayFieldStatement(int line, int column, string name, Expression count)
            : base(line, column, name)
        {
            this.count = count;
        }

        internal override void Read(BinaryReader reader, List<BaseData> result, Scope scope)
        {
            try
            {
                dynamic countValue = count.Eval(scope);
                int countInt = (int)countValue;
                
                if (reader.BaseStream.Position + sizeof(byte) * countInt >= reader.BaseStream.Length)
                    throw new AnalysisException(Line, Column, "Unexpected end of stream", Strings.Message_AnalysisError_UnexpectedEndOfStream);

                var data = new ByteData[countInt];
                for (int i = 0; i < countInt; i++)
                {
                    byte value = reader.ReadByte();
                    var element = new ByteData(i.ToString(), value);
                    data[i] = element;
                }

                var item = new ArrayData<ByteData>(name, "Byte", data);

                result.Add(item);
                scope.AddContent(name, item);
            }
            catch (BaseLocalizedException e)
            {
                throw new AnalysisException(Line, Column, "Failed to load field!", string.Format(Strings.Message_AnalysisError_FailedToReadField, name, e.LocalizedErrorMessage));
            }
            catch (Exception e)
            {
                throw new AnalysisException(Line, Column, "Failed to load field!", string.Format(Strings.Message_AnalysisError_FailedToReadField, name, e.Message));
            }
        }
    }

    class UshortArrayFieldStatement : BaseFieldStatement
    {
        private readonly Expression count;

        public UshortArrayFieldStatement(int line, int column, string name, Expression count)
            : base(line, column, name)
        {
            this.count = count;
        }

        internal override void Read(BinaryReader reader, List<BaseData> result, Scope scope)
        {
            try
            {
                dynamic countValue = count.Eval(scope);
                int countInt = (int)countValue;
                
                if (reader.BaseStream.Position + sizeof(ushort) * countInt >= reader.BaseStream.Length)
                    throw new AnalysisException(Line, Column, "Unexpected end of stream", Strings.Message_AnalysisError_UnexpectedEndOfStream);

                var data = new UshortData[countInt];
                for (int i = 0; i < countInt; i++)
                {
                    ushort value = reader.ReadUInt16();
                    var element = new UshortData(i.ToString(), value);
                    data[i] = element;
                }

                var item = new ArrayData<UshortData>(name, "Ushort", data);

                result.Add(item);
                scope.AddContent(name, item);
            }
            catch (BaseLocalizedException e)
            {
                throw new AnalysisException(Line, Column, "Failed to load field!", string.Format(Strings.Message_AnalysisError_FailedToReadField, name, e.LocalizedErrorMessage));
            }
            catch (Exception e)
            {
                throw new AnalysisException(Line, Column, "Failed to load field!", string.Format(Strings.Message_AnalysisError_FailedToReadField, name, e.Message));
            }
        }
    }

    class UintArrayFieldStatement : BaseFieldStatement
    {
        private readonly Expression count;

        public UintArrayFieldStatement(int line, int column, string name, Expression count)
            : base(line, column, name)
        {
            this.count = count;
        }

        internal override void Read(BinaryReader reader, List<BaseData> result, Scope scope)
        {
            try
            {
                dynamic countValue = count.Eval(scope);
                int countInt = (int)countValue;
                
                if (reader.BaseStream.Position + sizeof(uint) * countInt >= reader.BaseStream.Length)
                    throw new AnalysisException(Line, Column, "Unexpected end of stream", Strings.Message_AnalysisError_UnexpectedEndOfStream);

                var data = new UintData[countInt];
                for (int i = 0; i < countInt; i++)
                {
                    uint value = reader.ReadUInt32();
                    var element = new UintData(i.ToString(), value);
                    data[i] = element;
                }

                var item = new ArrayData<UintData>(name, "Uint", data);

                result.Add(item);
                scope.AddContent(name, item);
            }
            catch (BaseLocalizedException e)
            {
                throw new AnalysisException(Line, Column, "Failed to load field!", string.Format(Strings.Message_AnalysisError_FailedToReadField, name, e.LocalizedErrorMessage));
            }
            catch (Exception e)
            {
                throw new AnalysisException(Line, Column, "Failed to load field!", string.Format(Strings.Message_AnalysisError_FailedToReadField, name, e.Message));
            }
        }
    }

    class UlongArrayFieldStatement : BaseFieldStatement
    {
        private readonly Expression count;

        public UlongArrayFieldStatement(int line, int column, string name, Expression count)
            : base(line, column, name)
        {
            this.count = count;
        }

        internal override void Read(BinaryReader reader, List<BaseData> result, Scope scope)
        {
            try
            {
                dynamic countValue = count.Eval(scope);
                int countInt = (int)countValue;
                
                if (reader.BaseStream.Position + sizeof(ulong) * countInt >= reader.BaseStream.Length)
                    throw new AnalysisException(Line, Column, "Unexpected end of stream", Strings.Message_AnalysisError_UnexpectedEndOfStream);

                var data = new UlongData[countInt];
                for (int i = 0; i < countInt; i++)
                {
                    ulong value = reader.ReadUInt64();
                    var element = new UlongData(i.ToString(), value);
                    data[i] = element;
                }

                var item = new ArrayData<UlongData>(name, "Ulong", data);

                result.Add(item);
                scope.AddContent(name, item);
            }
            catch (BaseLocalizedException e)
            {
                throw new AnalysisException(Line, Column, "Failed to load field!", string.Format(Strings.Message_AnalysisError_FailedToReadField, name, e.LocalizedErrorMessage));
            }
            catch (Exception e)
            {
                throw new AnalysisException(Line, Column, "Failed to load field!", string.Format(Strings.Message_AnalysisError_FailedToReadField, name, e.Message));
            }
        }
    }

    class FloatArrayFieldStatement : BaseFieldStatement
    {
        private readonly Expression count;

        public FloatArrayFieldStatement(int line, int column, string name, Expression count)
            : base(line, column, name)
        {
            this.count = count;
        }

        internal override void Read(BinaryReader reader, List<BaseData> result, Scope scope)
        {
            try
            {
                dynamic countValue = count.Eval(scope);
                int countInt = (int)countValue;
                
                if (reader.BaseStream.Position + sizeof(float) * countInt >= reader.BaseStream.Length)
                    throw new AnalysisException(Line, Column, "Unexpected end of stream", Strings.Message_AnalysisError_UnexpectedEndOfStream);

                var data = new FloatData[countInt];
                for (int i = 0; i < countInt; i++)
                {
                    float value = reader.ReadSingle();
                    var element = new FloatData(i.ToString(), value);
                    data[i] = element;
                }

                var item = new ArrayData<FloatData>(name, "Float", data);

                result.Add(item);
                scope.AddContent(name, item);
            }
            catch (BaseLocalizedException e)
            {
                throw new AnalysisException(Line, Column, "Failed to load field!", string.Format(Strings.Message_AnalysisError_FailedToReadField, name, e.LocalizedErrorMessage));
            }
            catch (Exception e)
            {
                throw new AnalysisException(Line, Column, "Failed to load field!", string.Format(Strings.Message_AnalysisError_FailedToReadField, name, e.Message));
            }
        }
    }

    class DoubleArrayFieldStatement : BaseFieldStatement
    {
        private readonly Expression count;

        public DoubleArrayFieldStatement(int line, int column, string name, Expression count)
            : base(line, column, name)
        {
            this.count = count;
        }

        internal override void Read(BinaryReader reader, List<BaseData> result, Scope scope)
        {
            try
            {
                dynamic countValue = count.Eval(scope);
                int countInt = (int)countValue;
                
                if (reader.BaseStream.Position + sizeof(double) * countInt >= reader.BaseStream.Length)
                    throw new AnalysisException(Line, Column, "Unexpected end of stream", Strings.Message_AnalysisError_UnexpectedEndOfStream);

                var data = new DoubleData[countInt];
                for (int i = 0; i < countInt; i++)
                {
                    double value = reader.ReadDouble();
                    var element = new DoubleData(i.ToString(), value);
                    data[i] = element;
                }

                var item = new ArrayData<DoubleData>(name, "Double", data);

                result.Add(item);
                scope.AddContent(name, item);
            }
            catch (BaseLocalizedException e)
            {
                throw new AnalysisException(Line, Column, "Failed to load field!", string.Format(Strings.Message_AnalysisError_FailedToReadField, name, e.LocalizedErrorMessage));
            }
            catch (Exception e)
            {
                throw new AnalysisException(Line, Column, "Failed to load field!", string.Format(Strings.Message_AnalysisError_FailedToReadField, name, e.Message));
            }
        }
    }


    class SbyteEnumFieldStatement : BaseFieldStatement
    {
        private readonly SbyteEnumDefinition enumDef;

        public SbyteEnumFieldStatement(int line, int column, string name, SbyteEnumDefinition enumDef)
            : base(line, column, name)
        {
            this.enumDef = enumDef;
        }

        internal override void Read(BinaryReader reader, List<BaseData> result, Scope scope)
        {
            try
            {
                if (reader.BaseStream.Position + sizeof(sbyte) >= reader.BaseStream.Length)
                    throw new AnalysisException(Line, Column, "Unexpected end of stream", Strings.Message_AnalysisError_UnexpectedEndOfStream);

                sbyte value = reader.ReadSByte();

                var enumItem = enumDef.Items.FirstOrDefault(i => i.Value == value);
                
                var data = new SbyteEnumData(name, enumDef.Name, value, $"{enumItem?.Name ?? "Invalid enum value"} ({value})");
                result.Add(data);
                scope.AddContent(name, data);
            }
            catch (BaseLocalizedException e)
            {
                throw new AnalysisException(Line, Column, "Failed to load field!", string.Format(Strings.Message_AnalysisError_FailedToReadField, name, e.LocalizedErrorMessage));
            }
            catch (Exception e)
            {
                throw new AnalysisException(Line, Column, "Failed to load field!", string.Format(Strings.Message_AnalysisError_FailedToReadField, name, e.Message));
            }
        }
    }


    class ShortEnumFieldStatement : BaseFieldStatement
    {
        private readonly ShortEnumDefinition enumDef;

        public ShortEnumFieldStatement(int line, int column, string name, ShortEnumDefinition enumDef)
            : base(line, column, name)
        {
            this.enumDef = enumDef;
        }

        internal override void Read(BinaryReader reader, List<BaseData> result, Scope scope)
        {
            try
            {
                if (reader.BaseStream.Position + sizeof(short) >= reader.BaseStream.Length)
                    throw new AnalysisException(Line, Column, "Unexpected end of stream", Strings.Message_AnalysisError_UnexpectedEndOfStream);

                short value = reader.ReadInt16();

                var enumItem = enumDef.Items.FirstOrDefault(i => i.Value == value);
                
                var data = new ShortEnumData(name, enumDef.Name, value, $"{enumItem?.Name ?? "Invalid enum value"} ({value})");
                result.Add(data);
                scope.AddContent(name, data);
            }
            catch (BaseLocalizedException e)
            {
                throw new AnalysisException(Line, Column, "Failed to load field!", string.Format(Strings.Message_AnalysisError_FailedToReadField, name, e.LocalizedErrorMessage));
            }
            catch (Exception e)
            {
                throw new AnalysisException(Line, Column, "Failed to load field!", string.Format(Strings.Message_AnalysisError_FailedToReadField, name, e.Message));
            }
        }
    }


    class IntEnumFieldStatement : BaseFieldStatement
    {
        private readonly IntEnumDefinition enumDef;

        public IntEnumFieldStatement(int line, int column, string name, IntEnumDefinition enumDef)
            : base(line, column, name)
        {
            this.enumDef = enumDef;
        }

        internal override void Read(BinaryReader reader, List<BaseData> result, Scope scope)
        {
            try
            {
                if (reader.BaseStream.Position + sizeof(int) >= reader.BaseStream.Length)
                    throw new AnalysisException(Line, Column, "Unexpected end of stream", Strings.Message_AnalysisError_UnexpectedEndOfStream);

                int value = reader.ReadInt32();

                var enumItem = enumDef.Items.FirstOrDefault(i => i.Value == value);
                
                var data = new IntEnumData(name, enumDef.Name, value, $"{enumItem?.Name ?? "Invalid enum value"} ({value})");
                result.Add(data);
                scope.AddContent(name, data);
            }
            catch (BaseLocalizedException e)
            {
                throw new AnalysisException(Line, Column, "Failed to load field!", string.Format(Strings.Message_AnalysisError_FailedToReadField, name, e.LocalizedErrorMessage));
            }
            catch (Exception e)
            {
                throw new AnalysisException(Line, Column, "Failed to load field!", string.Format(Strings.Message_AnalysisError_FailedToReadField, name, e.Message));
            }
        }
    }


    class LongEnumFieldStatement : BaseFieldStatement
    {
        private readonly LongEnumDefinition enumDef;

        public LongEnumFieldStatement(int line, int column, string name, LongEnumDefinition enumDef)
            : base(line, column, name)
        {
            this.enumDef = enumDef;
        }

        internal override void Read(BinaryReader reader, List<BaseData> result, Scope scope)
        {
            try
            {
                if (reader.BaseStream.Position + sizeof(long) >= reader.BaseStream.Length)
                    throw new AnalysisException(Line, Column, "Unexpected end of stream", Strings.Message_AnalysisError_UnexpectedEndOfStream);

                long value = reader.ReadInt64();

                var enumItem = enumDef.Items.FirstOrDefault(i => i.Value == value);
                
                var data = new LongEnumData(name, enumDef.Name, value, $"{enumItem?.Name ?? "Invalid enum value"} ({value})");
                result.Add(data);
                scope.AddContent(name, data);
            }
            catch (BaseLocalizedException e)
            {
                throw new AnalysisException(Line, Column, "Failed to load field!", string.Format(Strings.Message_AnalysisError_FailedToReadField, name, e.LocalizedErrorMessage));
            }
            catch (Exception e)
            {
                throw new AnalysisException(Line, Column, "Failed to load field!", string.Format(Strings.Message_AnalysisError_FailedToReadField, name, e.Message));
            }
        }
    }


    class ByteEnumFieldStatement : BaseFieldStatement
    {
        private readonly ByteEnumDefinition enumDef;

        public ByteEnumFieldStatement(int line, int column, string name, ByteEnumDefinition enumDef)
            : base(line, column, name)
        {
            this.enumDef = enumDef;
        }

        internal override void Read(BinaryReader reader, List<BaseData> result, Scope scope)
        {
            try
            {
                if (reader.BaseStream.Position + sizeof(byte) >= reader.BaseStream.Length)
                    throw new AnalysisException(Line, Column, "Unexpected end of stream", Strings.Message_AnalysisError_UnexpectedEndOfStream);

                byte value = reader.ReadByte();

                var enumItem = enumDef.Items.FirstOrDefault(i => i.Value == value);
                
                var data = new ByteEnumData(name, enumDef.Name, value, $"{enumItem?.Name ?? "Invalid enum value"} ({value})");
                result.Add(data);
                scope.AddContent(name, data);
            }
            catch (BaseLocalizedException e)
            {
                throw new AnalysisException(Line, Column, "Failed to load field!", string.Format(Strings.Message_AnalysisError_FailedToReadField, name, e.LocalizedErrorMessage));
            }
            catch (Exception e)
            {
                throw new AnalysisException(Line, Column, "Failed to load field!", string.Format(Strings.Message_AnalysisError_FailedToReadField, name, e.Message));
            }
        }
    }


    class UshortEnumFieldStatement : BaseFieldStatement
    {
        private readonly UshortEnumDefinition enumDef;

        public UshortEnumFieldStatement(int line, int column, string name, UshortEnumDefinition enumDef)
            : base(line, column, name)
        {
            this.enumDef = enumDef;
        }

        internal override void Read(BinaryReader reader, List<BaseData> result, Scope scope)
        {
            try
            {
                if (reader.BaseStream.Position + sizeof(ushort) >= reader.BaseStream.Length)
                    throw new AnalysisException(Line, Column, "Unexpected end of stream", Strings.Message_AnalysisError_UnexpectedEndOfStream);

                ushort value = reader.ReadUInt16();

                var enumItem = enumDef.Items.FirstOrDefault(i => i.Value == value);
                
                var data = new UshortEnumData(name, enumDef.Name, value, $"{enumItem?.Name ?? "Invalid enum value"} ({value})");
                result.Add(data);
                scope.AddContent(name, data);
            }
            catch (BaseLocalizedException e)
            {
                throw new AnalysisException(Line, Column, "Failed to load field!", string.Format(Strings.Message_AnalysisError_FailedToReadField, name, e.LocalizedErrorMessage));
            }
            catch (Exception e)
            {
                throw new AnalysisException(Line, Column, "Failed to load field!", string.Format(Strings.Message_AnalysisError_FailedToReadField, name, e.Message));
            }
        }
    }


    class UintEnumFieldStatement : BaseFieldStatement
    {
        private readonly UintEnumDefinition enumDef;

        public UintEnumFieldStatement(int line, int column, string name, UintEnumDefinition enumDef)
            : base(line, column, name)
        {
            this.enumDef = enumDef;
        }

        internal override void Read(BinaryReader reader, List<BaseData> result, Scope scope)
        {
            try
            {
                if (reader.BaseStream.Position + sizeof(uint) >= reader.BaseStream.Length)
                    throw new AnalysisException(Line, Column, "Unexpected end of stream", Strings.Message_AnalysisError_UnexpectedEndOfStream);

                uint value = reader.ReadUInt32();

                var enumItem = enumDef.Items.FirstOrDefault(i => i.Value == value);
                
                var data = new UintEnumData(name, enumDef.Name, value, $"{enumItem?.Name ?? "Invalid enum value"} ({value})");
                result.Add(data);
                scope.AddContent(name, data);
            }
            catch (BaseLocalizedException e)
            {
                throw new AnalysisException(Line, Column, "Failed to load field!", string.Format(Strings.Message_AnalysisError_FailedToReadField, name, e.LocalizedErrorMessage));
            }
            catch (Exception e)
            {
                throw new AnalysisException(Line, Column, "Failed to load field!", string.Format(Strings.Message_AnalysisError_FailedToReadField, name, e.Message));
            }
        }
    }


    class UlongEnumFieldStatement : BaseFieldStatement
    {
        private readonly UlongEnumDefinition enumDef;

        public UlongEnumFieldStatement(int line, int column, string name, UlongEnumDefinition enumDef)
            : base(line, column, name)
        {
            this.enumDef = enumDef;
        }

        internal override void Read(BinaryReader reader, List<BaseData> result, Scope scope)
        {
            try
            {
                if (reader.BaseStream.Position + sizeof(ulong) >= reader.BaseStream.Length)
                    throw new AnalysisException(Line, Column, "Unexpected end of stream", Strings.Message_AnalysisError_UnexpectedEndOfStream);

                ulong value = reader.ReadUInt64();

                var enumItem = enumDef.Items.FirstOrDefault(i => i.Value == value);
                
                var data = new UlongEnumData(name, enumDef.Name, value, $"{enumItem?.Name ?? "Invalid enum value"} ({value})");
                result.Add(data);
                scope.AddContent(name, data);
            }
            catch (BaseLocalizedException e)
            {
                throw new AnalysisException(Line, Column, "Failed to load field!", string.Format(Strings.Message_AnalysisError_FailedToReadField, name, e.LocalizedErrorMessage));
            }
            catch (Exception e)
            {
                throw new AnalysisException(Line, Column, "Failed to load field!", string.Format(Strings.Message_AnalysisError_FailedToReadField, name, e.Message));
            }
        }
    }


    class FieldFactory
    {
        public static BaseFieldStatement FromTypeName(int line, int column, string typeName, string name)
        {
            switch (typeName)
            {
                case "sbyte":
                    return new SbyteFieldStatement(line, column, name);
                case "short":
                    return new ShortFieldStatement(line, column, name);
                case "int":
                    return new IntFieldStatement(line, column, name);
                case "long":
                    return new LongFieldStatement(line, column, name);
                case "byte":
                    return new ByteFieldStatement(line, column, name);
                case "ushort":
                    return new UshortFieldStatement(line, column, name);
                case "uint":
                    return new UintFieldStatement(line, column, name);
                case "ulong":
                    return new UlongFieldStatement(line, column, name);
                case "float":
                    return new FloatFieldStatement(line, column, name);
                case "double":
                    return new DoubleFieldStatement(line, column, name);
                case "skip":
                    return new SkipFieldStatement(line, column, name);
                case "char":
                    return new CharFieldStatement(line, column, name);
                default:
                    throw new InvalidEnumArgumentException("Unsupported type name!");
            }
        }        

        public static BaseFieldStatement FromEnum(int line, int column, string name, BaseEnumDefinition enumDef)
        {
            switch (enumDef)
            {
                case SbyteEnumDefinition sbyteEnumDefinition:
                    return new SbyteEnumFieldStatement(line, column, name, sbyteEnumDefinition);
                case ShortEnumDefinition shortEnumDefinition:
                    return new ShortEnumFieldStatement(line, column, name, shortEnumDefinition);
                case IntEnumDefinition intEnumDefinition:
                    return new IntEnumFieldStatement(line, column, name, intEnumDefinition);
                case LongEnumDefinition longEnumDefinition:
                    return new LongEnumFieldStatement(line, column, name, longEnumDefinition);
                case ByteEnumDefinition byteEnumDefinition:
                    return new ByteEnumFieldStatement(line, column, name, byteEnumDefinition);
                case UshortEnumDefinition ushortEnumDefinition:
                    return new UshortEnumFieldStatement(line, column, name, ushortEnumDefinition);
                case UintEnumDefinition uintEnumDefinition:
                    return new UintEnumFieldStatement(line, column, name, uintEnumDefinition);
                case UlongEnumDefinition ulongEnumDefinition:
                    return new UlongEnumFieldStatement(line, column, name, ulongEnumDefinition);
                default:
                    throw new InvalidEnumArgumentException("Unsupported enum type!");
            }
        }
    }

    class ArrayFieldFactory
    {
        public static BaseFieldStatement FromTypeName(int line, int column, string typeName, string name, Expression count)
        {
            switch (typeName)
            {
                case "sbyte":
                    return new SbyteArrayFieldStatement(line, column, name, count);
                case "short":
                    return new ShortArrayFieldStatement(line, column, name, count);
                case "int":
                    return new IntArrayFieldStatement(line, column, name, count);
                case "long":
                    return new LongArrayFieldStatement(line, column, name, count);
                case "byte":
                    return new ByteArrayFieldStatement(line, column, name, count);
                case "ushort":
                    return new UshortArrayFieldStatement(line, column, name, count);
                case "uint":
                    return new UintArrayFieldStatement(line, column, name, count);
                case "ulong":
                    return new UlongArrayFieldStatement(line, column, name, count);
                case "float":
                    return new FloatArrayFieldStatement(line, column, name, count);
                case "double":
                    return new DoubleArrayFieldStatement(line, column, name, count);
                case "skip":
                    return new SkipArrayFieldStatement(line, column, name, count);
                case "char":
                    return new CharArrayFieldStatement(line, column, name, count);
                default:
                    throw new InvalidEnumArgumentException("Unsupported type name!");
            }
        }
    }
}

namespace Dev.Editor.BinAnalyzer.Data
{
    public class SbyteData : BaseValueData
    {       
        private readonly sbyte value;

        public SbyteData(string name, sbyte value)
            : base(name, "sbyte")
        {
            this.value = value;
        }

        public override dynamic GetValue()
        {
            return value;
        }
    }

    public class ShortData : BaseValueData
    {       
        private readonly short value;

        public ShortData(string name, short value)
            : base(name, "short")
        {
            this.value = value;
        }

        public override dynamic GetValue()
        {
            return value;
        }
    }

    public class IntData : BaseValueData
    {       
        private readonly int value;

        public IntData(string name, int value)
            : base(name, "int")
        {
            this.value = value;
        }

        public override dynamic GetValue()
        {
            return value;
        }
    }

    public class LongData : BaseValueData
    {       
        private readonly long value;

        public LongData(string name, long value)
            : base(name, "long")
        {
            this.value = value;
        }

        public override dynamic GetValue()
        {
            return value;
        }
    }

    public class ByteData : BaseValueData
    {       
        private readonly byte value;

        public ByteData(string name, byte value)
            : base(name, "byte")
        {
            this.value = value;
        }

        public override dynamic GetValue()
        {
            return value;
        }
    }

    public class UshortData : BaseValueData
    {       
        private readonly ushort value;

        public UshortData(string name, ushort value)
            : base(name, "ushort")
        {
            this.value = value;
        }

        public override dynamic GetValue()
        {
            return value;
        }
    }

    public class UintData : BaseValueData
    {       
        private readonly uint value;

        public UintData(string name, uint value)
            : base(name, "uint")
        {
            this.value = value;
        }

        public override dynamic GetValue()
        {
            return value;
        }
    }

    public class UlongData : BaseValueData
    {       
        private readonly ulong value;

        public UlongData(string name, ulong value)
            : base(name, "ulong")
        {
            this.value = value;
        }

        public override dynamic GetValue()
        {
            return value;
        }
    }

    public class FloatData : BaseValueData
    {       
        private readonly float value;

        public FloatData(string name, float value)
            : base(name, "float")
        {
            this.value = value;
        }

        public override dynamic GetValue()
        {
            return value;
        }
    }

    public class DoubleData : BaseValueData
    {       
        private readonly double value;

        public DoubleData(string name, double value)
            : base(name, "double")
        {
            this.value = value;
        }

        public override dynamic GetValue()
        {
            return value;
        }
    }


    public class SbyteEnumData : BaseValueData
    {
        private readonly sbyte value;
        private readonly string enumValue;

        public SbyteEnumData(string name, string enumName, sbyte value, string enumValue)
            : base(name, $"{enumName} : byte")
        {
            this.value = value;
            this.enumValue = enumValue;
        }

        public override dynamic GetValue()
        {
            return value;
        }

        public override string Value => enumValue;
    }


    public class ShortEnumData : BaseValueData
    {
        private readonly short value;
        private readonly string enumValue;

        public ShortEnumData(string name, string enumName, short value, string enumValue)
            : base(name, $"{enumName} : byte")
        {
            this.value = value;
            this.enumValue = enumValue;
        }

        public override dynamic GetValue()
        {
            return value;
        }

        public override string Value => enumValue;
    }


    public class IntEnumData : BaseValueData
    {
        private readonly int value;
        private readonly string enumValue;

        public IntEnumData(string name, string enumName, int value, string enumValue)
            : base(name, $"{enumName} : byte")
        {
            this.value = value;
            this.enumValue = enumValue;
        }

        public override dynamic GetValue()
        {
            return value;
        }

        public override string Value => enumValue;
    }


    public class LongEnumData : BaseValueData
    {
        private readonly long value;
        private readonly string enumValue;

        public LongEnumData(string name, string enumName, long value, string enumValue)
            : base(name, $"{enumName} : byte")
        {
            this.value = value;
            this.enumValue = enumValue;
        }

        public override dynamic GetValue()
        {
            return value;
        }

        public override string Value => enumValue;
    }


    public class ByteEnumData : BaseValueData
    {
        private readonly byte value;
        private readonly string enumValue;

        public ByteEnumData(string name, string enumName, byte value, string enumValue)
            : base(name, $"{enumName} : byte")
        {
            this.value = value;
            this.enumValue = enumValue;
        }

        public override dynamic GetValue()
        {
            return value;
        }

        public override string Value => enumValue;
    }


    public class UshortEnumData : BaseValueData
    {
        private readonly ushort value;
        private readonly string enumValue;

        public UshortEnumData(string name, string enumName, ushort value, string enumValue)
            : base(name, $"{enumName} : byte")
        {
            this.value = value;
            this.enumValue = enumValue;
        }

        public override dynamic GetValue()
        {
            return value;
        }

        public override string Value => enumValue;
    }


    public class UintEnumData : BaseValueData
    {
        private readonly uint value;
        private readonly string enumValue;

        public UintEnumData(string name, string enumName, uint value, string enumValue)
            : base(name, $"{enumName} : byte")
        {
            this.value = value;
            this.enumValue = enumValue;
        }

        public override dynamic GetValue()
        {
            return value;
        }

        public override string Value => enumValue;
    }


    public class UlongEnumData : BaseValueData
    {
        private readonly ulong value;
        private readonly string enumValue;

        public UlongEnumData(string name, string enumName, ulong value, string enumValue)
            : base(name, $"{enumName} : byte")
        {
            this.value = value;
            this.enumValue = enumValue;
        }

        public override dynamic GetValue()
        {
            return value;
        }

        public override string Value => enumValue;
    }

    public class DataFactory
    {
        public static BaseData DataFromDynamic(string name, dynamic d)
        {
            if (d is sbyte sbyteDynamic)
                return new SbyteData(name, sbyteDynamic);
            else if (d is short shortDynamic)
                return new ShortData(name, shortDynamic);
            else if (d is int intDynamic)
                return new IntData(name, intDynamic);
            else if (d is long longDynamic)
                return new LongData(name, longDynamic);
            else if (d is byte byteDynamic)
                return new ByteData(name, byteDynamic);
            else if (d is ushort ushortDynamic)
                return new UshortData(name, ushortDynamic);
            else if (d is uint uintDynamic)
                return new UintData(name, uintDynamic);
            else if (d is ulong ulongDynamic)
                return new UlongData(name, ulongDynamic);
            else if (d is float floatDynamic)
                return new FloatData(name, floatDynamic);
            else if (d is double doubleDynamic)
                return new DoubleData(name, doubleDynamic);
            else  if (d is string str)
                return new CharArrayData(name, str);
            else if (d is bool b)
                return new BoolData(name, b);
            else

                throw new InvalidOperationException("Unsupported type!");
        }
    }
}