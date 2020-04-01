﻿using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;

namespace protobuf_net.Grpc.Test
{
    /// <summary>
    /// Legacy ServiceContract, not directly in the gRPC Request/Reply model
    /// </summary>
    [ServiceContract]
    public interface ILegacyService
    {
        // blocking, multiple arguments
        HelloReply Shared_Legacy_BlockingUnary(string arg1, long arg2);
        HelloReply Shared_Legacy_BlockingUnary_ManyArgs(string arg1, long arg2, HelloRequest arg3,
            string arg4, long arg5, HelloRequest arg6,string arg7, long arg8, HelloRequest arg9);
        void Shared_Legacy_BlockingUnary_ValVoid(string arg1, long arg2);

        // blocking, ValueType
        void Shared_Legacy_BlockingUnary_ValueTypeVoid(long arg);
        long Shared_Legacy_BlockingUnary_ValueTypeValueType(DateTime arg);
        IList<long> Shared_Legacy_BlockingUnary_IListValueTypeIListValueType(IList<DateTime> arg);
        long Shared_Legacy_BlockingUnary_VoidValueType();

        // async, multiple arguments
        Task<HelloReply> Shared_Legacy_TaskUnary(string arg1, long arg2);
        Task Shared_Legacy_TaskUnary_ValVoid(string arg1, long arg2);
        ValueTask<HelloReply> Shared_Legacy_ValueTaskUnary(string arg1, long arg2);
        ValueTask Shared_Legacy_ValueTaskUnary_ValVoid(string arg1, long arg2);

        // async, ValueType
        Task Shared_Legacy_TaskUnary_ValueTypeVoid(long arg);
        Task<long> Shared_Legacy_TaskUnary_ValueTypeValueType(DateTime arg);
        Task<IList<long>> Shared_Legacy_TaskUnary_IListValueTypeIListValueType(IList<DateTime> arg);
        Task<long> Shared_Legacy_TaskUnary_VoidValueType();
        ValueTask Shared_Legacy_ValueTaskUnary_ValueTypeVoid(long arg);
        ValueTask<long> Shared_Legacy_ValueTaskUnary_ValueTypeValueType(DateTime arg);
        ValueTask<IList<long>> Shared_Legacy_ValueTaskUnary_IListValueTypeIListValueType(IList<DateTime> arg);
        ValueTask<long> Shared_Legacy_ValueTaskUnary_VoidValueType();
    }
}
