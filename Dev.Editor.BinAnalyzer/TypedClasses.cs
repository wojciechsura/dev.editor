﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using Dev.Editor.BinAnalyzer.Data;
using Dev.Editor.BinAnalyzer.ProgramItems.Expressions;

namespace Dev.Editor.BinAnalyzer.ProgramItems.Statements
{
     class ByteFieldStatement : BaseFieldStatement
     {
         public ByteFieldStatement(string name)
             : base(name)
         {
            
         }

         internal override void Read(BinaryReader reader, List<BaseData> result, Scope scope)
         {
             try
             {
                 byte value = reader.ReadByte();

                 var data = new ByteData(name, value);
                 result.Add(data);
                 scope.Contents.Add(name, data);
             }
             catch
             {
                 throw new InvalidOperationException("Cannot load data!");
             }
         }
     }

     class SbyteFieldStatement : BaseFieldStatement
     {
         public SbyteFieldStatement(string name)
             : base(name)
         {
            
         }

         internal override void Read(BinaryReader reader, List<BaseData> result, Scope scope)
         {
             try
             {
                 sbyte value = reader.ReadSByte();

                 var data = new SbyteData(name, value);
                 result.Add(data);
                 scope.Contents.Add(name, data);
             }
             catch
             {
                 throw new InvalidOperationException("Cannot load data!");
             }
         }
     }

     class ShortFieldStatement : BaseFieldStatement
     {
         public ShortFieldStatement(string name)
             : base(name)
         {
            
         }

         internal override void Read(BinaryReader reader, List<BaseData> result, Scope scope)
         {
             try
             {
                 short value = reader.ReadInt16();

                 var data = new ShortData(name, value);
                 result.Add(data);
                 scope.Contents.Add(name, data);
             }
             catch
             {
                 throw new InvalidOperationException("Cannot load data!");
             }
         }
     }

     class UshortFieldStatement : BaseFieldStatement
     {
         public UshortFieldStatement(string name)
             : base(name)
         {
            
         }

         internal override void Read(BinaryReader reader, List<BaseData> result, Scope scope)
         {
             try
             {
                 ushort value = reader.ReadUInt16();

                 var data = new UshortData(name, value);
                 result.Add(data);
                 scope.Contents.Add(name, data);
             }
             catch
             {
                 throw new InvalidOperationException("Cannot load data!");
             }
         }
     }

     class IntFieldStatement : BaseFieldStatement
     {
         public IntFieldStatement(string name)
             : base(name)
         {
            
         }

         internal override void Read(BinaryReader reader, List<BaseData> result, Scope scope)
         {
             try
             {
                 int value = reader.ReadInt32();

                 var data = new IntData(name, value);
                 result.Add(data);
                 scope.Contents.Add(name, data);
             }
             catch
             {
                 throw new InvalidOperationException("Cannot load data!");
             }
         }
     }

     class UintFieldStatement : BaseFieldStatement
     {
         public UintFieldStatement(string name)
             : base(name)
         {
            
         }

         internal override void Read(BinaryReader reader, List<BaseData> result, Scope scope)
         {
             try
             {
                 uint value = reader.ReadUInt32();

                 var data = new UintData(name, value);
                 result.Add(data);
                 scope.Contents.Add(name, data);
             }
             catch
             {
                 throw new InvalidOperationException("Cannot load data!");
             }
         }
     }

     class LongFieldStatement : BaseFieldStatement
     {
         public LongFieldStatement(string name)
             : base(name)
         {
            
         }

         internal override void Read(BinaryReader reader, List<BaseData> result, Scope scope)
         {
             try
             {
                 long value = reader.ReadInt64();

                 var data = new LongData(name, value);
                 result.Add(data);
                 scope.Contents.Add(name, data);
             }
             catch
             {
                 throw new InvalidOperationException("Cannot load data!");
             }
         }
     }

     class UlongFieldStatement : BaseFieldStatement
     {
         public UlongFieldStatement(string name)
             : base(name)
         {
            
         }

         internal override void Read(BinaryReader reader, List<BaseData> result, Scope scope)
         {
             try
             {
                 ulong value = reader.ReadUInt64();

                 var data = new UlongData(name, value);
                 result.Add(data);
                 scope.Contents.Add(name, data);
             }
             catch
             {
                 throw new InvalidOperationException("Cannot load data!");
             }
         }
     }

     class FloatFieldStatement : BaseFieldStatement
     {
         public FloatFieldStatement(string name)
             : base(name)
         {
            
         }

         internal override void Read(BinaryReader reader, List<BaseData> result, Scope scope)
         {
             try
             {
                 float value = reader.ReadSingle();

                 var data = new FloatData(name, value);
                 result.Add(data);
                 scope.Contents.Add(name, data);
             }
             catch
             {
                 throw new InvalidOperationException("Cannot load data!");
             }
         }
     }

     class DoubleFieldStatement : BaseFieldStatement
     {
         public DoubleFieldStatement(string name)
             : base(name)
         {
            
         }

         internal override void Read(BinaryReader reader, List<BaseData> result, Scope scope)
         {
             try
             {
                 double value = reader.ReadDouble();

                 var data = new DoubleData(name, value);
                 result.Add(data);
                 scope.Contents.Add(name, data);
             }
             catch
             {
                 throw new InvalidOperationException("Cannot load data!");
             }
         }
     }

     class ByteArrayFieldStatement : BaseFieldStatement
     {
         private readonly Expression count;

         public ByteArrayFieldStatement(string name, Expression count)
             : base(name)
         {
             this.count = count;
         }
     }

     class SbyteArrayFieldStatement : BaseFieldStatement
     {
         private readonly Expression count;

         public SbyteArrayFieldStatement(string name, Expression count)
             : base(name)
         {
             this.count = count;
         }
     }

     class ShortArrayFieldStatement : BaseFieldStatement
     {
         private readonly Expression count;

         public ShortArrayFieldStatement(string name, Expression count)
             : base(name)
         {
             this.count = count;
         }
     }

     class UshortArrayFieldStatement : BaseFieldStatement
     {
         private readonly Expression count;

         public UshortArrayFieldStatement(string name, Expression count)
             : base(name)
         {
             this.count = count;
         }
     }

     class IntArrayFieldStatement : BaseFieldStatement
     {
         private readonly Expression count;

         public IntArrayFieldStatement(string name, Expression count)
             : base(name)
         {
             this.count = count;
         }
     }

     class UintArrayFieldStatement : BaseFieldStatement
     {
         private readonly Expression count;

         public UintArrayFieldStatement(string name, Expression count)
             : base(name)
         {
             this.count = count;
         }
     }

     class LongArrayFieldStatement : BaseFieldStatement
     {
         private readonly Expression count;

         public LongArrayFieldStatement(string name, Expression count)
             : base(name)
         {
             this.count = count;
         }
     }

     class UlongArrayFieldStatement : BaseFieldStatement
     {
         private readonly Expression count;

         public UlongArrayFieldStatement(string name, Expression count)
             : base(name)
         {
             this.count = count;
         }
     }

     class FloatArrayFieldStatement : BaseFieldStatement
     {
         private readonly Expression count;

         public FloatArrayFieldStatement(string name, Expression count)
             : base(name)
         {
             this.count = count;
         }
     }

     class DoubleArrayFieldStatement : BaseFieldStatement
     {
         private readonly Expression count;

         public DoubleArrayFieldStatement(string name, Expression count)
             : base(name)
         {
             this.count = count;
         }
     }

    class FieldFactory
    {
        public static BaseFieldStatement FromTypeName(string typeName, string name)
        {
            switch (typeName)
            {
                case "byte":
                    return new ByteFieldStatement(name);
                case "sbyte":
                    return new SbyteFieldStatement(name);
                case "short":
                    return new ShortFieldStatement(name);
                case "ushort":
                    return new UshortFieldStatement(name);
                case "int":
                    return new IntFieldStatement(name);
                case "uint":
                    return new UintFieldStatement(name);
                case "long":
                    return new LongFieldStatement(name);
                case "ulong":
                    return new UlongFieldStatement(name);
                case "float":
                    return new FloatFieldStatement(name);
                case "double":
                    return new DoubleFieldStatement(name);
                case "skip":
                    return new SkipFieldStatement(name);
                default:
                    throw new InvalidEnumArgumentException("Unsupported type name!");
            }
        }
    }

    class ArrayFieldFactory
    {
        public static BaseFieldStatement FromTypeName(string typeName, string name, Expression count)
        {
            switch (typeName)
            {
                case "byte":
                    return new ByteArrayFieldStatement(name, count);
                case "sbyte":
                    return new SbyteArrayFieldStatement(name, count);
                case "short":
                    return new ShortArrayFieldStatement(name, count);
                case "ushort":
                    return new UshortArrayFieldStatement(name, count);
                case "int":
                    return new IntArrayFieldStatement(name, count);
                case "uint":
                    return new UintArrayFieldStatement(name, count);
                case "long":
                    return new LongArrayFieldStatement(name, count);
                case "ulong":
                    return new UlongArrayFieldStatement(name, count);
                case "float":
                    return new FloatArrayFieldStatement(name, count);
                case "double":
                    return new DoubleArrayFieldStatement(name, count);
                case "skip":
                    return new SkipArrayFieldStatement(name, count);
                default:
                    throw new InvalidEnumArgumentException("Unsupported type name!");
            }
        }
    }
}

namespace Dev.Editor.BinAnalyzer.Data
{
    public class ByteData : BaseValueData
    {       
        public ByteData(string name, byte value)
            : base(name)
        {
            Value = value;
        }

        public override dynamic GetValue()
        {
            return Value;
        }

        public byte Value { get; }
    }

    public class SbyteData : BaseValueData
    {       
        public SbyteData(string name, sbyte value)
            : base(name)
        {
            Value = value;
        }

        public override dynamic GetValue()
        {
            return Value;
        }

        public sbyte Value { get; }
    }

    public class ShortData : BaseValueData
    {       
        public ShortData(string name, short value)
            : base(name)
        {
            Value = value;
        }

        public override dynamic GetValue()
        {
            return Value;
        }

        public short Value { get; }
    }

    public class UshortData : BaseValueData
    {       
        public UshortData(string name, ushort value)
            : base(name)
        {
            Value = value;
        }

        public override dynamic GetValue()
        {
            return Value;
        }

        public ushort Value { get; }
    }

    public class IntData : BaseValueData
    {       
        public IntData(string name, int value)
            : base(name)
        {
            Value = value;
        }

        public override dynamic GetValue()
        {
            return Value;
        }

        public int Value { get; }
    }

    public class UintData : BaseValueData
    {       
        public UintData(string name, uint value)
            : base(name)
        {
            Value = value;
        }

        public override dynamic GetValue()
        {
            return Value;
        }

        public uint Value { get; }
    }

    public class LongData : BaseValueData
    {       
        public LongData(string name, long value)
            : base(name)
        {
            Value = value;
        }

        public override dynamic GetValue()
        {
            return Value;
        }

        public long Value { get; }
    }

    public class UlongData : BaseValueData
    {       
        public UlongData(string name, ulong value)
            : base(name)
        {
            Value = value;
        }

        public override dynamic GetValue()
        {
            return Value;
        }

        public ulong Value { get; }
    }

    public class FloatData : BaseValueData
    {       
        public FloatData(string name, float value)
            : base(name)
        {
            Value = value;
        }

        public override dynamic GetValue()
        {
            return Value;
        }

        public float Value { get; }
    }

    public class DoubleData : BaseValueData
    {       
        public DoubleData(string name, double value)
            : base(name)
        {
            Value = value;
        }

        public override dynamic GetValue()
        {
            return Value;
        }

        public double Value { get; }
    }


    public class DataFactory
    {
        public static BaseData DataFromDynamic(string name, dynamic d)
        {
            if (d is byte byteDynamic)
                return new ByteData(name, byteDynamic);
            else if (d is sbyte sbyteDynamic)
                return new SbyteData(name, sbyteDynamic);
            else if (d is short shortDynamic)
                return new ShortData(name, shortDynamic);
            else if (d is ushort ushortDynamic)
                return new UshortData(name, ushortDynamic);
            else if (d is int intDynamic)
                return new IntData(name, intDynamic);
            else if (d is uint uintDynamic)
                return new UintData(name, uintDynamic);
            else if (d is long longDynamic)
                return new LongData(name, longDynamic);
            else if (d is ulong ulongDynamic)
                return new UlongData(name, ulongDynamic);
            else if (d is float floatDynamic)
                return new FloatData(name, floatDynamic);
            else if (d is double doubleDynamic)
                return new DoubleData(name, doubleDynamic);
            else 
                throw new InvalidOperationException("Unsupported type!");
        }
    }
}