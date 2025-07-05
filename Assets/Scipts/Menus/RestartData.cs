using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RestartData
{
    public static List<UnitType> cachedPlayerTypes = null;

    public static bool HasSavedSelection => cachedPlayerTypes != null && cachedPlayerTypes.Count > 0;

    public static void Clear()
    {
        cachedPlayerTypes = null;
    }

}
