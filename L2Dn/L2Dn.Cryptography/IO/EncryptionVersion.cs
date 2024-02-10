namespace L2Dn.IO;

public enum EncryptionVersion
{
    Unknown = 0,

    // Xor
    Ver111 = 111,
    Ver120 = 120,
    Ver121 = 121,

    // LameCrypt Xor
    Ver811 = 811,
    Ver820 = 820,
    Ver821 = 821,

    // Blowfish
    Ver211 = 211,
    Ver212 = 212,

    // LameCrypt Blowfish
    Ver911 = 911,
    Ver912 = 912,

    // RSA
    Ver411 = 411,
    Ver412 = 412,
    Ver413 = 413,
    Ver414 = 414,

    // LameCrypt RSA
    Ver611 = 611,
    Ver612 = 612,
    Ver613 = 613,
    Ver614 = 614,
}
