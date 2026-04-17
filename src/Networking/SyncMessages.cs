using Unity.Netcode;
using Unity.Collections;
using System;

namespace LethalRogueLike.src.Networking;

public static class SyncMessages
{
    public const string SyncMessageType = "LethalCompanyMod_Sync";
    public const string SelectMessageType = "LethalCompanyMod_Select";
    public const string VoteMessageType = "LethalCompanyMod_vote";
    public const string VoteResultMessageType = "LethalCompanyMod_voteResult";

    public struct ModifierSyncMessage : INetworkSerializable
    {
        public FixedString64Bytes RunId;
        public int LandingCount;
        public bool IsSelectionPending;
        public int ActiveModifierCount;
        public int PendingChoiceCount;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref RunId);
            serializer.SerializeValue(ref LandingCount);
            serializer.SerializeValue(ref IsSelectionPending);
            serializer.SerializeValue(ref ActiveModifierCount);
            serializer.SerializeValue(ref PendingChoiceCount);
        }
    }

    public struct ModifierSelectMessage : INetworkSerializable
    {
        public FixedString64Bytes ModifierId;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref ModifierId);
        }
    }

    public struct ModifierVoteMessage : INetworkSerializable
    {
        public FixedString64Bytes ModifierId;
        public FixedString64Bytes ClientId;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref ModifierId);
            serializer.SerializeValue(ref ClientId);
        }
    }

    public struct ModifierVoteResultMessage : INetworkSerializable
    {
        public FixedString64Bytes WinnerModifierId;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref WinnerModifierId);
        }
    }
}
