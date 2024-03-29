﻿namespace L2Dn.AuthServer.NetworkGameServer.OutgoingPacket;

internal enum RegistrationResult
{
    Success = 0, // server registered
    InvalidAccessKey = 1, // invalid access key
    NotListedInDb = 2, // server is not listed in db table and new servers are not accepted
    AnotherServerRegistered = 3, // another server registered with the same id
}