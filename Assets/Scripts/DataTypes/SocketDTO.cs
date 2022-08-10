using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SocketDTO
{
    [Serializable]
    public struct JoinResponse
    {
        public string id;
        public string nickname;
        public string spriteName;
        public float[] pos;
        public int tokenId;
    }

    [Serializable]
    public struct JoinRequest
    {
        public string nickname;
        public string spriteName;
        public float[] pos;
    }

    [Serializable]
    public struct LeaveResponse
    {
        public string id;
    }

    [Serializable]
    public struct TransformRequest
    {
        public float[] pos;
    }

    [Serializable]
    public struct TransformResponse
    {
        public string id;
        public float[] pos;
    }

    [Serializable]
    public struct MembersResponse
    {
        public string id;
        public JoinResponse[] members;
    }

    [Serializable]
    public struct NameResponse
    {
        public string id;
        public string name;
    }

    [Serializable]
    public struct ReactionRequest
    {
        public int idx;
    }

    [Serializable]
    public struct ReactionResponse
    {
        public string id;
        public int idx;
    }

    [Serializable]
    public struct PerformResponse
    {
        public bool state;
        public string id;
    }

    [Serializable]
    public struct VinylRequest
    {
        public int tokenId;
    }

    [Serializable]
    public struct VinylResponse
    {
        public string id;
        public int tokenId;
    }

    [Serializable]
    public struct StudioJoinRequest
    {
        public string deviceType;
        public string channelName;
    }

    [Serializable]
    public struct StudioJoinResponse
    {
        public string deviceType;
    }

    [Serializable]
    public struct StudioStreamRequest
    {
        public string streamKey;
    }
}
