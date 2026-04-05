using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
#endif

namespace CodeGeneration
{
#if UNITY_EDITOR
    public static class ConstKeysGenerator
    {
        private const string DefaultOutputFolderPath = "Assets/Generated";
        private const string DefaultNamespace = "Loot.Data";
        private const string FallbackClassName = "GeneratedKeys";
        private const string FallbackMemberName = "UnnamedItem";

        public static void GenerateItemKeysClass(string className, List<ConstPair> itemsSetups)
        {
            GenerateKeysClass(className, itemsSetups, DefaultOutputFolderPath, DefaultNamespace);
        }

        public static void GenerateKeysClass(
            string className,
            IReadOnlyList<ConstPair> constPairs,
            string outputFolderPath,
            string namespaceName)
        {
            var safeClassName = MakeSafeIdentifier(className, FallbackClassName);
            var safeNamespace = string.IsNullOrWhiteSpace(namespaceName) ? DefaultNamespace : namespaceName.Trim();
            var safeOutputFolderPath = string.IsNullOrWhiteSpace(outputFolderPath) ? DefaultOutputFolderPath : outputFolderPath.Trim();

            if (!IsProjectSafeFolderPath(safeOutputFolderPath))
            {
                Debug.LogError($"Code generation aborted. Unsafe output folder path: {safeOutputFolderPath}");
                return;
            }

            if (!Directory.Exists(safeOutputFolderPath))
            {
                Directory.CreateDirectory(safeOutputFolderPath);
            }

            var filePath = Path.Combine(safeOutputFolderPath, $"{safeClassName}.cs");
            var lines = BuildClassLines(safeNamespace, safeClassName, constPairs);
            var content = string.Join("\n", lines);

            SafeWriteAllText(filePath, content);

            AssetDatabase.Refresh();
        }

        private static List<string> BuildClassLines(string namespaceName, string className, IReadOnlyList<ConstPair> constPairs)
        {
            var lines = new List<string>
            {
                "// AUTO-GENERATED CODE. DO NOT EDIT.",
                $"namespace {namespaceName}",
                "{",
                $"    public static class {className}",
                "    {"
            };

            var usedNames = new HashSet<string>();
            var usedIds = new HashSet<string>();

            foreach (var pair in constPairs ?? Enumerable.Empty<ConstPair>())
            {
                if (string.IsNullOrWhiteSpace(pair.Id))
                {
                    Debug.LogWarning($"Skipped generated key for '{pair.Name}' because id is empty.");
                    continue;
                }

                if (!usedIds.Add(pair.Id))
                {
                    Debug.LogWarning($"Skipped duplicated generated id '{pair.Id}' for key '{pair.Name}'.");
                    continue;
                }

                var baseName = MakeSafeIdentifier(pair.Name, FallbackMemberName);
                var uniqueName = MakeUniqueName(baseName, usedNames);
                var escapedId = EscapeStringLiteral(pair.Id);

                lines.Add($"        public const string {uniqueName} = \"{escapedId}\";");
            }

            lines.Add("    }");
            lines.Add("}");

            return lines;
        }

        private static string MakeUniqueName(string baseName, HashSet<string> usedNames)
        {
            if (usedNames.Add(baseName))
            {
                return baseName;
            }

            var index = 2;
            var candidate = $"{baseName}_{index}";
            while (!usedNames.Add(candidate))
            {
                index++;
                candidate = $"{baseName}_{index}";
            }

            return candidate;
        }

        private static string MakeSafeIdentifier(string value, string fallback)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return fallback;
            }

            var safe = new StringBuilder(value.Length);

            foreach (var c in value.Trim())
            {
                safe.Append(char.IsLetterOrDigit(c) ? c : '_');
            }

            if (safe.Length == 0)
            {
                return fallback;
            }

            if (char.IsDigit(safe[0]))
            {
                safe.Insert(0, '_');
            }

            return safe.ToString();
        }

        private static string EscapeStringLiteral(string value)
        {
            return value.Replace("\\", "\\\\").Replace("\"", "\\\"");
        }

        private static bool IsProjectSafeFolderPath(string folderPath)
        {
            if (string.IsNullOrWhiteSpace(folderPath))
            {
                return false;
            }

            var normalizedFolderPath = folderPath.Trim();
            if (normalizedFolderPath != "Assets" &&
                !normalizedFolderPath.StartsWith("Assets/") &&
                !normalizedFolderPath.StartsWith("Assets\\"))
            {
                return false;
            }

            var projectRootPath = Path.GetFullPath(Path.Combine(Application.dataPath, ".."))
                .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                + Path.DirectorySeparatorChar;
            var fullOutputPath = Path.GetFullPath(normalizedFolderPath)
                .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                + Path.DirectorySeparatorChar;

            return fullOutputPath.StartsWith(projectRootPath, System.StringComparison.OrdinalIgnoreCase);
        }

        private static void SafeWriteAllText(string filePath, string content)
        {
            var tempPath = $"{filePath}.tmp";

            File.WriteAllText(tempPath, content, new UTF8Encoding(false));

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            File.Move(tempPath, filePath);
        }
    }

    public struct ConstPair
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
#endif
}