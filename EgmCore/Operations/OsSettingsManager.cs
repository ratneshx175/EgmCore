using EgmCore.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace EgmCore.Operations
{
    public class OsSettingsManager
    {
        private readonly string _file = "egm_data/system_config.json";

        // Attempts to set timezone. Returns resolved timezone id on success, null on failure.
        public string? SetTimezone(string tz, IEgmLogger audit, string actor = "operator")
        {
            if (string.IsNullOrWhiteSpace(tz))
            {
                audit.Log($"OS CHANGE FAILED | operator={actor} | invalid timezone: empty");
                return null;
            }

            var resolved = ResolveToUnique(tz, out var errorMessage);
            if (resolved == null)
            {
                audit.Log($"OS CHANGE FAILED | operator={actor} | {errorMessage}");
                return null;
            }

            var old = "UTC";
            try
            {
                if (File.Exists(_file))
                {
                    var doc = JsonSerializer.Deserialize<Dictionary<string, string>>(File.ReadAllText(_file));
                    if (doc != null && doc.TryGetValue("timezone", out var tv)) old = tv;
                }

                var newCfg = new Dictionary<string, string> { ["timezone"] = resolved };
                Directory.CreateDirectory(Path.GetDirectoryName(_file) ?? "egm_data");
                File.WriteAllText(_file, JsonSerializer.Serialize(newCfg, new JsonSerializerOptions { WriteIndented = true }));

                audit.Log($"OS CHANGE | operator={actor} | timezone {old} → {resolved}");
                audit.Log($"SIMULATED: timedatectl set-timezone {resolved}");
                return resolved;
            }
            catch (Exception ex)
            {
                audit.Log($"OS CHANGE FAILED | operator={actor} | error: {ex.Message}");
                return null;
            }
        }

        // Resolve input to a unique system timezone id; returns null and an error message if ambiguous/not found
        private string? ResolveToUnique(string input, out string errorMessage)
        {
            errorMessage = string.Empty;
            List<TimeZoneInfo> tzs;
            try
            {
                tzs = TimeZoneInfo.GetSystemTimeZones().ToList();
            }
            catch
            {
                errorMessage = "system timezones not available on this platform";
                return null;
            }

            var availableList = tzs.Select(t => t.Id).ToList();
            var available = availableList.ToHashSet(StringComparer.OrdinalIgnoreCase);

            var aliases = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
            {
                // prefer Windows ids first, then IANA where applicable
                ["india"] = new[] { "India Standard Time", "Asia/Kolkata" },
                ["conakry"] = new[] { "W. Central Africa Standard Time", "Africa/Conakry" },
                ["africa"] = new[] { "W. Central Africa Standard Time", "Africa/Conakry" },
                ["chicago"] = new[] { "Central Standard Time", "America/Chicago" },
                ["usa"] = new[] { "Central Standard Time", "America/Chicago" },
            };

            var req = input.Trim();

            // exact id match
            if (available.Contains(req)) return req;

            // alias (try candidate ids/names in order)
            if (aliases.TryGetValue(req, out var mappedCandidates))
            {
                foreach (var mapped in mappedCandidates)
                {
                    // direct id match
                    var byId = tzs.FirstOrDefault(t => t.Id.Equals(mapped, StringComparison.OrdinalIgnoreCase));
                    if (byId != null) return byId.Id;

                    // try display/standard name contains
                    var byName = tzs.Where(t => (!string.IsNullOrEmpty(t.DisplayName) && t.DisplayName.IndexOf(mapped, StringComparison.OrdinalIgnoreCase) >= 0)
                                              || (!string.IsNullOrEmpty(t.StandardName) && t.StandardName.IndexOf(mapped, StringComparison.OrdinalIgnoreCase) >= 0)).ToList();
                    if (byName.Count == 1) return byName[0].Id;

                    // try last-segment fallback
                    var seg = mapped.Split('/').Last();
                    var ends = tzs.Where(t => t.Id.EndsWith(seg, StringComparison.OrdinalIgnoreCase)).ToList();
                    if (ends.Count == 1) return ends[0].Id;
                }
            }

            // last two/last segment when input contains '/'
            if (req.Contains('/'))
            {
                var parts = req.Split('/', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length >= 2)
                {
                    var lastTwo = parts[^2] + "/" + parts[^1];
                    if (available.Contains(lastTwo)) return lastTwo;

                    var last = parts[^1];
                    var ends = availableList.Where(a => a.EndsWith(last, StringComparison.OrdinalIgnoreCase)).ToList();
                    if (ends.Count == 1) return ends[0];
                }
            }

            // if single-word input, try matching by DisplayName or StandardName last segment (e.g., 'Central', 'Africa', 'Chicago')
            if (!req.Contains(' '))
            {
                var byDisplay = tzs.Where(t => (!string.IsNullOrEmpty(t.DisplayName) && t.DisplayName.IndexOf(req, StringComparison.OrdinalIgnoreCase) >= 0)
                                               || (!string.IsNullOrEmpty(t.StandardName) && t.StandardName.IndexOf(req, StringComparison.OrdinalIgnoreCase) >= 0)
                                               || t.Id.Split('/').Last().Equals(req, StringComparison.OrdinalIgnoreCase)).ToList();
                if (byDisplay.Count == 1) return byDisplay[0].Id;
            }

            // fuzzy across Id, DisplayName and StandardName
            var matches = tzs.Where(t => t.Id.IndexOf(req, StringComparison.OrdinalIgnoreCase) >= 0
                                || (!string.IsNullOrEmpty(t.DisplayName) && t.DisplayName.IndexOf(req, StringComparison.OrdinalIgnoreCase) >= 0)
                                || (!string.IsNullOrEmpty(t.StandardName) && t.StandardName.IndexOf(req, StringComparison.OrdinalIgnoreCase) >= 0)).ToList();
            if (matches.Count == 1) return matches[0].Id;
            if (matches.Count == 0)
            {
                errorMessage = $"timezone '{input}' not recognized on this system";
                return null;
            }

            errorMessage = $"ambiguous timezone '{input}' (matches: {string.Join(',', matches.Select(m => m.Id).Take(8))})";
            return null;
        }
    }
}
