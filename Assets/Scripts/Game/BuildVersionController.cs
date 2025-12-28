#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

public class BuildVersionController : IPreprocessBuildWithReport
{
    public int callbackOrder => 0;

    public void OnPreprocessBuild(BuildReport report)
    {
        string currentVersion = PlayerSettings.bundleVersion;
        string[] parts = currentVersion.Split('.');

        int buildNumber = 0;

        if (parts.Length > 0 && int.TryParse(parts[^1], out int parsed))
        {
            buildNumber = parsed + 1;
            parts[^1] = buildNumber.ToString();
        }
        else
        {
            // np. "1.0" → "1.0.1"
            buildNumber = 1;
            System.Array.Resize(ref parts, parts.Length + 1);
            parts[^1] = buildNumber.ToString();
        }

        string newVersion = string.Join(".", parts);
        PlayerSettings.bundleVersion = newVersion;

        UnityEngine.Debug.Log("New build version: " + newVersion);
    }
}
#endif
