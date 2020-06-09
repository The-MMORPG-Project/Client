﻿using System;
using System.IO;

namespace Valk.Networking
{
    public class ENetProtocol : IDisposable
    {
        private BinaryWriter writer;
        private MemoryStream stream;
        private byte[] buffer;

        private void InitWriter(int size)
        {
            buffer = new byte[size];
            stream = new MemoryStream(buffer);
            writer = new BinaryWriter(stream);
        }

        public byte[] Serialize(byte code, params object[] values)
        {
            int bufferSize = 0;
            bufferSize += sizeof(byte);
            foreach (object value in values)
            {
                Type type = value.GetType();

                if (type == typeof(int) || type == typeof(uint))
                    bufferSize += sizeof(int);

                if (type == typeof(float))
                    bufferSize += sizeof(float);

                if (type == typeof(string))
                    bufferSize += (sizeof(char) * ((string)value).Length);
            }

            InitWriter(bufferSize);
            writer.Write(code);

            foreach (object value in values)
            {
                Type type = value.GetType();
                if (type == typeof(uint))
                    writer.Write((uint)value);

                if (type == typeof(float))
                    writer.Write((float)value);

                if (type == typeof(int))
                    writer.Write((int)value);

                if (type == typeof(string))
                    writer.Write((string)value);
            }
            return buffer;
        }

        // Flag: Has Dispose already been called?
        bool disposed = false;

        // Public implementation of Dispose pattern callable by consumers.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                writer.Dispose();
                stream.Dispose();
            }

            disposed = true;
        }
    }
}