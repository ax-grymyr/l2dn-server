using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace L2Dn.IO;

public sealed class EncryptedStream: WrapperStream
{
    private readonly EncryptionVersion _version;
    internal const int HeaderSize = 28;
    private const string _headerPrefix = "Lineage2Ver";

    private EncryptedStream(Stream stream, EncryptionVersion version): base(stream)
    {
        _version = version;
    }

    public EncryptionVersion Version => _version;

    public static EncryptedStream OpenRead(string filePath) => OpenRead(
        new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read),
        Path.GetFileName(filePath));

    public static EncryptedStream OpenRead(Stream stream, string fileName)
    {
        EncryptionVersion version = ReadHeader(stream);
        Stream decryptionStream = CreateInputStream(stream, fileName, version);
        return new EncryptedStream(decryptionStream, version);
    }

    public static EncryptedStream OpenWrite(Stream stream, string fileName, EncryptionVersion version)
    {
        Stream encryptionStream = CreateOutputStream(stream, fileName, version);
        WriteHeader(stream, version);
        return new EncryptedStream(encryptionStream, version);
    }

    private static Stream CreateInputStream(Stream stream, string fileName, EncryptionVersion version) =>
        version switch
        {
            // Xor
            EncryptionVersion.Ver111 => new XorStream(stream, EncryptionKeys.XorKey111),
            EncryptionVersion.Ver120 => new L2Ver120Stream(stream),
            EncryptionVersion.Ver121 => new XorStream(stream, EncryptionKeys.GetXorKey121(fileName)),

            // LameCrypt Xor
            EncryptionVersion.Ver811 => new XorStream(new LameCryptStream(stream), EncryptionKeys.XorKey111),
            EncryptionVersion.Ver820 => new L2Ver120Stream(new LameCryptStream(stream)),
            EncryptionVersion.Ver821 => new XorStream(new LameCryptStream(stream),
                EncryptionKeys.GetXorKey121(fileName)),

            // Blowfish
            EncryptionVersion.Ver211 => new BlowfishStream(stream, EncryptionKeys.BlowfishKey211),
            EncryptionVersion.Ver212 => new BlowfishStream(stream, EncryptionKeys.BlowfishKey212),

            // LameCrypt Blowfish
            EncryptionVersion.Ver911 => new BlowfishStream(new LameCryptStream(stream), EncryptionKeys.BlowfishKey211),
            EncryptionVersion.Ver912 => new BlowfishStream(new LameCryptStream(stream), EncryptionKeys.BlowfishKey212),

            // RSA
            EncryptionVersion.Ver411 => new L2Ver41xInputStream(stream, EncryptionKeys.RsaDecryption411),
            EncryptionVersion.Ver412 => new L2Ver41xInputStream(stream, EncryptionKeys.RsaDecryption412),
            EncryptionVersion.Ver413 => new L2Ver41xInputStream(stream, EncryptionKeys.RsaDecryption413),
            EncryptionVersion.Ver414 => new L2Ver41xInputStream(stream, EncryptionKeys.RsaDecryption414),

            // LameCrypt RSA
            EncryptionVersion.Ver611 => new L2Ver41xInputStream(new LameCryptStream(stream),
                EncryptionKeys.RsaDecryption411),
            EncryptionVersion.Ver612 => new L2Ver41xInputStream(new LameCryptStream(stream),
                EncryptionKeys.RsaDecryption412),
            EncryptionVersion.Ver613 => new L2Ver41xInputStream(new LameCryptStream(stream),
                EncryptionKeys.RsaDecryption413),
            EncryptionVersion.Ver614 => new L2Ver41xInputStream(new LameCryptStream(stream),
                EncryptionKeys.RsaDecryption414),

            _ => throw new InvalidOperationException($"Encryption version {(int)version} is not supported.")
        };

    private static Stream CreateOutputStream(Stream stream, string fileName, EncryptionVersion version)
    {
        throw new NotImplementedException();
    }

    private static EncryptionVersion ReadHeader(Stream stream)
    {
        Span<byte> buffer = stackalloc byte[HeaderSize];
        int bytesRead = stream.Read(buffer);

        ReadOnlySpan<byte> header = buffer[..bytesRead];
        if (header.Length != HeaderSize)
            ThrowInvalidFile();

        // TODO: conversion to UTF-16LE in big endian architectures
        ReadOnlySpan<char> str = MemoryMarshal.Cast<byte, char>(header);
        if (!str.StartsWith(_headerPrefix))
            ThrowInvalidFile();

        str = str[_headerPrefix.Length..];
        if (!uint.TryParse(str, out uint value))
            ThrowInvalidFile();

        return (EncryptionVersion)value;
    }

    private static void WriteHeader(Stream stream, EncryptionVersion version)
    {
        Span<byte> header = stackalloc byte[HeaderSize];
        Span<char> str = MemoryMarshal.Cast<byte, char>(header);

        // TODO: conversion from UTF-16LE in big endian architectures
        _headerPrefix.AsSpan().CopyTo(str);
        if (!((uint)version).TryFormat(str[_headerPrefix.Length..], out int bytesWritten) || bytesWritten != 3)
            throw new InvalidOperationException("Invalid encryption method");

        stream.Write(header);
    }

    [DoesNotReturn]
    private static void ThrowInvalidFile() => throw new InvalidOperationException("File is not encrypted L2 file.");
}
