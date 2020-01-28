﻿using System;

namespace Wjire.RPC.DotNetty.Serializer
{
    public interface ISerializer
    {
        object ToObject(object obj, Type type);

        T ToObject<T>(string objString);

        byte[] ToBytes(object obj);

    }
}
