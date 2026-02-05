/*
 * 
 * NLog로 남기는 로그를 유니티 콘솔에도 띄워주는 코드
 * 
 */

using UnityEngine;
using NLog;
using NLog.Targets;

[Target("myLog")]
public sealed class UnityLogTarget : TargetWithLayout
{
#if UNITY_EDITOR
    protected override void Write(LogEventInfo logEvent)
    {
        string message = this.RenderLogEvent(this.Layout, logEvent);

        switch (logEvent.Level.Name.ToLower())
        {
            case "trace":
            case "debug":
                Debug.Log($"<color=black>[{logEvent.Level.Name.ToUpper()}]</color> {message}");
                break;
            case "info":
                Debug.Log($"<color=white>[INFO]</color> {message}");
                break;
            case "warn":
                Debug.LogWarning($"<color=yellow>[WARN]</color> {message}");
                break;
            case "error":
            case "fatal":
                Debug.LogError($"<color=red>[{logEvent.Level.Name.ToUpper()}]</color> {message}");
                break;
        }
    }
#endif
}