using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TapOn.Utils
{
    public static class TapOnUtils
    {
        public static IEnumerator WaitSomeTime(Action before = null, Action after = null, float time = 0.1f)
        {
            if (before != null) before();
            yield return new Unity.UIWidgets.async.UIWidgetsWaitForSeconds(time);
            if (after != null) after();
        }
    }
}
