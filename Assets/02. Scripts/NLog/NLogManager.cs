/*
 * 
 * 사용 예시
 * 
 * NLogManager.logger.Debug("msg");
 * 
 */

using UnityEngine;
using NLog;
using NLog.Config;
using System.IO;

public static class NLogManager
{
    public static readonly NLog.Logger logger = LogManager.GetCurrentClassLogger();

    static NLogManager()
    {
        LogManager.Configuration = new XmlLoggingConfiguration(Path.Combine(Application.streamingAssetsPath, "NLog.config"));
    }
}