// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Buffers;
using System.IO;
using MessagePack;
using Microsoft.AspNetCore.SignalR.Protocol;

namespace Microsoft.AspNetCore.Components.Server.BlazorPack
{
    internal sealed class BlazorPackHubProtocolWorker : MessagePackHubProtocolWorker
    {
        protected override object DeserializeObject(ref MessagePackReader reader, Type type, string field)
        {
            try
            {
                if (type == typeof(string))
                {
                    return ReadString(ref reader, "argument");
                }
                else if (type == typeof(bool))
                {
                    return reader.ReadBoolean();
                }
                else if (type == typeof(int))
                {
                    return reader.ReadInt32();
                }
                else if (type == typeof(long))
                {
                    return reader.ReadInt64();
                }
                else if (type == typeof(float))
                {
                    return reader.ReadSingle();
                }
                else if (type == typeof(ReadOnlySequence<byte>))
                {
                    // This is how I think it should work, but sometimes the memory seems to get corrupted.
                    // The "await foreach (var chunk in subject)" code in RemoteJSDataStream sometimes gets
                    // chunks with negative lengths. So, this is not actually used in this proof-of-concept
                    return reader.ReadBytes() ?? null;
                }
                else if (type == typeof(byte[]))
                {
                    var bytes = reader.ReadBytes();
                    if (!bytes.HasValue)
                    {
                        return null;
                    }
                    else if (bytes.Value.Length == 0)
                    {
                        return Array.Empty<byte>();
                    }

                    return bytes.Value.ToArray();
                }
            }
            catch (Exception ex)
            {
                throw new InvalidDataException($"Deserializing object of the `{type.Name}` type for '{field}' failed.", ex);
            }

            throw new FormatException($"Type {type} is not supported");
        }

        protected override void Serialize(ref MessagePackWriter writer, Type type, object value)
        {
            switch (value)
            {
                case null:
                    writer.WriteNil();
                    break;

                case bool boolValue:
                    writer.Write(boolValue);
                    break;

                case string stringValue:
                    writer.Write(stringValue);
                    break;

                case int intValue:
                    writer.Write(intValue);
                    break;

                case long longValue:
                    writer.Write(longValue);
                    break;

                case float floatValue:
                    writer.Write(floatValue);
                    break;

                case ArraySegment<byte> bytes:
                    writer.Write(bytes);
                    break;

                case byte[] byteArray:
                    writer.Write(byteArray);
                    break;

                default:
                    throw new FormatException($"Unsupported argument type {type}");
            }
        }
    }
}
