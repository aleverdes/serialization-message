using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace AleVerDes.BinarySerialization.Converters.Extensions
{
    public class ListBinaryConverter<T> : IBinaryConverter where T : class, IBinaryConverter, new()
    {
        public Type SerializationType { get; }

        private readonly T _singleValueConverter;

        public ListBinaryConverter()
        {
            _singleValueConverter = new T();
            var listType = typeof(List<>);
            SerializationType = listType.MakeGenericType(_singleValueConverter.SerializationType);
        }
        
        public void Serialize(object value, BinaryWriter bw)
        {
            var typedValue = (IList)value;
            var length = typedValue.Count;
            bw.Write(BitConverter.GetBytes(length));
            for (var i = 0; i < length; i++)
            {
                _singleValueConverter.Serialize(typedValue[i], bw);
            }
        }

        public object Deserialize(BinaryReader br)
        {
            var arrayLength = br.ReadInt32();
            var typedValue = (IList) Activator.CreateInstance(SerializationType);
            for (var i = 0; i < arrayLength; i++)
            {
                typedValue.Add(_singleValueConverter.Deserialize(br));
            }
            return typedValue;
        }
    }
}