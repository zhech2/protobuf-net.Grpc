using System;
using System.Linq;
using System.Reflection;
using Grpc.Core;
using ProtoBuf.Grpc.Configuration;

namespace ProtoBuf.Grpc.Internal
{
    public class ValueTypeWrapper<T> where T : struct
    {
        public T Value;

        public ValueTypeWrapper(T value)
        {
            Value = value;
        }
    }

    internal class ValueTypeWrapperMarshallerFactory : MarshallerFactory
    {
        private MarshallerCache _cache;
        static readonly MethodInfo s_getMarshaller = typeof(MarshallerCache).GetMethod(
            nameof(MarshallerCache.GetMarshaller), BindingFlags.Instance | BindingFlags.NonPublic)!;
        static readonly MethodInfo s_invokeDeserializer = typeof(ValueTypeWrapperMarshallerFactory).GetMethod(
            nameof(InvokeDeserializer), BindingFlags.Instance | BindingFlags.NonPublic)!;
        static readonly MethodInfo s_invokeSerializer = typeof(ValueTypeWrapperMarshallerFactory).GetMethod(
            nameof(InvokeSerializer), BindingFlags.Instance | BindingFlags.NonPublic)!;

        internal ValueTypeWrapperMarshallerFactory(MarshallerCache cache)
        {
            _cache = cache;
        }

        protected internal override bool CanSerialize(Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ValueTypeWrapper<>))
                type = type.GetGenericArguments()[0];

            return _cache.CanSerializeType(type);
        }

        protected override T Deserialize<T>(byte[] payload)
        {
            var type = typeof(T);
            if (!type.IsGenericType || type.GetGenericTypeDefinition() != typeof(ValueTypeWrapper<>))
            {
                return _cache.GetMarshaller<T>().Deserializer(payload);
            }

            type = type.GetGenericArguments()[0];
            var marshaller = s_getMarshaller.MakeGenericMethod(type).Invoke(_cache, null);
            if (marshaller == null)
                throw new InvalidOperationException("No marshaller available for " + typeof(T).FullName);
            
            return (T)s_invokeDeserializer.MakeGenericMethod(type).Invoke(this, new object[] { marshaller, payload });
        }

        protected override byte[] Serialize<T>(T value)
        {
            var type = typeof(T);
            if (!type.IsGenericType || type.GetGenericTypeDefinition() != typeof(ValueTypeWrapper<>))
            {
                return _cache.GetMarshaller<T>().Serializer(value);
            }

            type = type.GetGenericArguments()[0];
            var marshaller = s_getMarshaller.MakeGenericMethod(type).Invoke(_cache, null);
            if (marshaller == null)
                throw new InvalidOperationException("No marshaller available for " + typeof(T).FullName);
            
            return (byte[])s_invokeSerializer.MakeGenericMethod(type).Invoke(this, new object?[] { marshaller, value });
        }

        private ValueTypeWrapper<T> InvokeDeserializer<T>(object marshaller, byte[] payload) where T : struct
        {
            return new ValueTypeWrapper<T>(((Marshaller<T>)marshaller).Deserializer(payload));
        }

        private byte[] InvokeSerializer<T>(object marshaller, ValueTypeWrapper<T> value) where T : struct
        {
            return ((Marshaller<T>)marshaller).Serializer(value.Value);
        }
    }
}
