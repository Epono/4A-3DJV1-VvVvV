using UnityEngine;
using System.Collections;

public class Utils {

    public static string NetworkPlayerToFormattedAddress(NetworkPlayer networkPlayer) {
        return networkPlayer.ipAddress + ":" + networkPlayer.port;
    }
}