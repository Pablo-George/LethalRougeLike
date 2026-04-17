using Unity.Netcode;
using Unity.Collections;
using System;

namespace LethalRogueLike.src.Networking;

public class NetworkHandler : NetworkBehaviour
{
    public static NetworkHandler? Instance { get; private set; }

    public static void Initialize()
    {
        Plugin.Logger.LogInfo("[NetworkHandler] Initialized.");
    }

    public override void OnNetworkSpawn()
    {
        try
        {
            if (Instance != null && Instance != this)
            {
                Plugin.Logger.LogWarning("[NetworkHandler] Duplicate instance detected! Destroying.");
                Destroy(gameObject);
                return;
            }
            Instance = this;

            Plugin.Logger.LogInfo($"[NetworkHandler] Spawned. IsHost: {IsHost}, IsServer: {IsServer}, IsClient: {IsClient}");

            if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsServer)
            {
                Plugin.Logger.LogDebug("[NetworkHandler] Registering sync message handlers...");
                NetworkManager.Singleton.CustomMessagingManager.RegisterNamedMessageHandler(
                    SyncMessages.SyncMessageType,
                    HandleSyncMessage);
                NetworkManager.Singleton.CustomMessagingManager.RegisterNamedMessageHandler(
                    SyncMessages.VoteMessageType,
                    HandleVoteMessage);
                Plugin.Logger.LogDebug("[NetworkHandler] Sync handlers registered.");
            }
            else
            {
                Plugin.Logger.LogDebug("[NetworkHandler] Not server - skipping handler registration.");
            }
        }
        catch (Exception ex)
        {
            Plugin.Logger.LogError($"[NetworkHandler] Error in OnNetworkSpawn: {ex.Message}");
        }
    }

    public override void OnNetworkDespawn()
    {
        try
        {
            if (NetworkManager.Singleton != null)
            {
                Plugin.Logger.LogDebug("[NetworkHandler] Unregistering message handlers...");
                NetworkManager.Singleton.CustomMessagingManager.UnregisterNamedMessageHandler(SyncMessages.SyncMessageType);
                NetworkManager.Singleton.CustomMessagingManager.UnregisterNamedMessageHandler(SyncMessages.VoteMessageType);
                Plugin.Logger.LogDebug("[NetworkHandler] Message handlers unregistered.");
            }
        }
        catch (Exception ex)
        {
            Plugin.Logger.LogError($"[NetworkHandler] Error in OnNetworkDespawn: {ex.Message}");
        }
    }

    private void HandleSyncMessage(ulong clientId, FastBufferReader reader)
    {
        try
        {
            if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsServer)
            {
                Plugin.Logger.LogWarning("[NetworkHandler] HandleSyncMessage called on non-server!");
                return;
            }

            reader.ReadValueSafe(out SyncMessages.ModifierSyncMessage syncMsg);
            Plugin.Logger.LogDebug($"[NetworkHandler] Received sync from client {clientId}: RunId={syncMsg.RunId}");
            BroadcastSyncState();
        }
        catch (Exception ex)
        {
            Plugin.Logger.LogError($"[NetworkHandler] Error handling sync message: {ex.Message}");
        }
    }

    private void HandleVoteMessage(ulong clientId, FastBufferReader reader)
    {
        try
        {
            if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsServer)
            {
                return;
            }

            reader.ReadValueSafe(out SyncMessages.ModifierVoteMessage voteMsg);
            var clientIdStr = voteMsg.ClientId.ToString();
            var modifierId = voteMsg.ModifierId.ToString();
            Plugin.Logger.LogDebug($"[NetworkHandler] Received vote from client {clientIdStr}: {modifierId}");

            RunState.Instance.RecordVote(clientIdStr, modifierId);

            if (RunState.Instance.AllVotesReceived())
            {
                var winner = RunState.Instance.GetVoteWinner();
                BroadcastVoteResult(winner);
                RunState.Instance.ClearVotes();
                RunState.Instance.SelectModifier(winner);
            }
        }
        catch (Exception ex)
        {
            Plugin.Logger.LogError($"[NetworkHandler] Error handling vote: {ex.Message}");
        }
    }

    public void SendVote(string modifierId)
    {
        try
        {
            if (NetworkManager.Singleton == null || NetworkManager.Singleton.IsServer)
            {
                return;
            }

            using var writer = new FastBufferWriter(256, Allocator.Temp);
            var msg = new SyncMessages.ModifierVoteMessage
            {
                ModifierId = modifierId,
                ClientId = NetworkManager.Singleton.LocalClientId.ToString()
            };

            writer.WriteValueSafe(msg);
            if (NetworkManager.Singleton != null)
            {
                NetworkManager.Singleton.CustomMessagingManager.SendNamedMessage(
                    SyncMessages.VoteMessageType,
                    0UL,
                    writer);
            }

            Plugin.Logger.LogInfo($"[NetworkHandler] Sent vote: {modifierId}");
        }
        catch (Exception ex)
        {
            Plugin.Logger.LogError($"[NetworkHandler] Error sending vote: {ex.Message}");
        }
    }

    private void BroadcastVoteResult(string winnerModifierId)
    {
        try
        {
            if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsServer)
            {
                return;
            }

            using var writer = new FastBufferWriter(256, Allocator.Temp);
            var msg = new SyncMessages.ModifierVoteResultMessage
            {
                WinnerModifierId = winnerModifierId
            };

            writer.WriteValueSafe(msg);
            NetworkManager.Singleton.CustomMessagingManager.SendNamedMessageToAll(
                SyncMessages.VoteResultMessageType,
                writer);

            Plugin.Logger.LogInfo($"[NetworkHandler] Broadcasted vote result: {winnerModifierId}");
        }
        catch (Exception ex)
        {
            Plugin.Logger.LogError($"[NetworkHandler] Error broadcasting vote result: {ex.Message}");
        }
    }

    public void BroadcastSyncState()
    {
        try
        {
            if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsServer)
            {
                Plugin.Logger.LogWarning("[NetworkHandler] BroadcastSyncState called on non-server!");
                return;
            }

            using var writer = new FastBufferWriter(256, Allocator.Temp);
            var msg = new SyncMessages.ModifierSyncMessage
            {
                RunId = RunState.Instance.RunId,
                LandingCount = RunState.Instance.LandingCount,
                IsSelectionPending = RunState.Instance.IsSelectionPending,
                ActiveModifierCount = RunState.Instance.ActiveModifiers.Count,
                PendingChoiceCount = RunState.Instance.PendingChoices.Count
            };

            writer.WriteValueSafe(msg);

            NetworkManager.Singleton.CustomMessagingManager.SendNamedMessageToAll(
                SyncMessages.SyncMessageType,
                writer);

            Plugin.Logger.LogDebug($"[NetworkHandler] Broadcast sync: RunId={msg.RunId}, Landing={msg.LandingCount}, ActiveModifiers={msg.ActiveModifierCount}");
        }
        catch (Exception ex)
        {
            Plugin.Logger.LogError($"[NetworkHandler] Error broadcasting sync: {ex.Message}");
        }
    }

    public void BroadcastModifierSelection(string modifierId)
    {
        try
        {
            if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsServer)
            {
                Plugin.Logger.LogWarning("[NetworkHandler] BroadcastModifierSelection called on non-server!");
                return;
            }

            if (string.IsNullOrEmpty(modifierId))
            {
                Plugin.Logger.LogWarning("[NetworkHandler] BroadcastModifierSelection called with null/empty modifierId!");
                return;
            }

            using var writer = new FastBufferWriter(256, Allocator.Temp);
            var msg = new SyncMessages.ModifierSelectMessage
            {
                ModifierId = modifierId
            };

            writer.WriteValueSafe(msg);

            NetworkManager.Singleton.CustomMessagingManager.SendNamedMessageToAll(
                SyncMessages.SelectMessageType,
                writer);

            Plugin.Logger.LogInfo($"[NetworkHandler] Broadcasted modifier selection: {modifierId}");
        }
        catch (Exception ex)
        {
            Plugin.Logger.LogError($"[NetworkHandler] Error broadcasting modifier selection: {ex.Message}");
        }
    }
}
