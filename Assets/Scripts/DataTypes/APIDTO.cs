using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace APIDTO
{
    [Serializable]
    public struct MuseSignInRequest
    {
        public string userNumber;
    }

    [Serializable]
    public struct SignInRequest
    {
        public string userNumber;
        public string verificationCode;
    }

    [Serializable]
    public struct AutoSignInRequest
    {
        public string userNumber;
        public string refreshToken;
    }

    [Serializable]
    public struct SignInResponse
    {
        public int id;
        public string userAlias;
        public string refreshToken;
        public string accessToken;
    }

    [Serializable]
    public struct SignInResponseWrapper
    {
        public SignInResponse message;
    }

    [Serializable]
    public struct GetMusicianResponse
    {
        public int id;
        public string alias;
        public int points;
        public string liveUrl;
    }

    [Serializable]
    public struct RegisterRequest
    {
        public int museId;
        public string museAlias;
    }

    [Serializable]
    public struct RegisterResponse
    {
        public int id;
        public int points;
        public string streamKey;
    }

    [Serializable]
    public struct RandomResponse
    {
        public int museId;
    }
}