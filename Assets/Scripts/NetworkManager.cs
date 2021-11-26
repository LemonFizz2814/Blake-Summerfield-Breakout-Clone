using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetworkManager : NetworkBehaviour
{
    readonly SyncList<GameObject> players = new SyncList<GameObject>();

    [Command]
    public void PlayerJoinedCmd(GameObject _obj)
    {
        players.Add(_obj);
        Debug.Log("added " + players.Count);
    }
}
