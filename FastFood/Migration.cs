using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace SqlServerToPostgresMigration
{
    /// <summary>
    /// SQL Server to PostgreSQL Migration Tool
    /// </summary>
    public class SqlMigrationTool
    {
        private readonly Dictionary<string, string> _dataTypeMap;
        private readonly Dictionary<string, string> _functionMap;
        private List<string> _warnings;
        private List<string> _errors;

        public SqlMigrationTool()
        {
            _warnings = new List<string>();
            _errors = new List<string>();

            // Data Type Mappings
            _dataTypeMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                // Integer types
                { @"\bINT\b", "INTEGER" },
                { @"\bBIGINT\b", "BIGINT" },
                { @"\bSMALLINT\b", "SMALLINT" },
                { @"\bTINYINT\b", "SMALLINT" },
                
                // String types
                { @"\bNVARCHAR\s*\(\s*MAX\s*\)", "TEXT" },
                { @"\bVARCHAR\s*\(\s*MAX\s*\)", "TEXT" },
                { @"\bNVARCHAR\s*\((\d+)\)", "VARCHAR($1)" },
                { @"\bVARCHAR\s*\((\d+)\)", "VARCHAR($1)" },
                { @"\bNTEXT\b", "TEXT" },
                { @"\bTEXT\b", "TEXT" },
                
                // Date/Time types
                { @"\bDATETIME2?\b", "TIMESTAMP" },
                { @"\bSMALLDATETIME\b", "TIMESTAMP" },
                { @"\bDATE\b", "DATE" },
                { @"\bTIME\b", "TIME" },
                
                // Numeric types
                { @"\bDECIMAL\s*\((\d+)\s*,\s*(\d+)\)", "NUMERIC($1,$2)" },
                { @"\bNUMERIC\s*\((\d+)\s*,\s*(\d+)\)", "NUMERIC($1,$2)" },
                { @"\bMONEY\b", "NUMERIC(19,4)" },
                { @"\bSMALLMONEY\b", "NUMERIC(10,4)" },
                { @"\bFLOAT\b", "DOUBLE PRECISION" },
                { @"\bREAL\b", "REAL" },
                
                // Binary types
                { @"\bVARBINARY\s*\(\s*MAX\s*\)", "BYTEA" },
                { @"\bVARBINARY\s*\((\d+)\)", "BYTEA" },
                { @"\bIMAGE\b", "BYTEA" },
                
                // Other types
                { @"\bBIT\b", "BOOLEAN" },
                { @"\bUNIQUEIDENTIFIER\b", "UUID" }
            };

            // Function Mappings
            _functionMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { @"\bGETDATE\s*\(\s*\)", "NOW()" },
                { @"\bGETUTCDATE\s*\(\s*\)", "NOW() AT TIME ZONE 'UTC'" },
                { @"\bNEWID\s*\(\s*\)", "gen_random_uuid()" },
                { @"\bISNULL\s*\(", "COALESCE(" },
                { @"\bLEN\s*\(", "LENGTH(" },
                { @"\bCHARINDEX\s*\(", "STRPOS(" }
            };
        }

        public ConversionResult ConvertQuery(string sql)
        {
            _warnings.Clear();
            _errors.Clear();

            string original = sql;
            string converted = sql;

            // Apply all conversions
            converted = RemoveDboSchema(converted);
            converted = RemoveSquareBrackets(converted);
            converted = ConvertDataTypes(converted);
            converted = ConvertFunctions(converted);
            converted = ConvertBooleanValues(converted);
            converted = ConvertTopToLimit(converted);
            converted = ConvertIdentity(converted);

            return new ConversionResult
            {
                Success = true,
                Original = original,
                Converted = converted,
                Warnings = new List<string>(_warnings),
                Errors = new List<string>(_errors)
            };
        }

        private string ConvertDataTypes(string sql)
        {
            string result = sql;
            foreach (var kvp in _dataTypeMap)
            {
                result = Regex.Replace(result, kvp.Key, kvp.Value, RegexOptions.IgnoreCase);
            }
            return result;
        }

        private string ConvertFunctions(string sql)
        {
            string result = sql;
            foreach (var kvp in _functionMap)
            {
                result = Regex.Replace(result, kvp.Key, kvp.Value, RegexOptions.IgnoreCase);
            }
            return result;
        }

        private string RemoveSquareBrackets(string sql)
        {
            return Regex.Replace(sql, @"\[(\w+)\]", m =>
            {
                string identifier = m.Groups[1].Value;
                // Lowercase simple identifiers
                return identifier.ToLower();
            });
        }

        private string RemoveDboSchema(string sql)
        {
            return Regex.Replace(sql, @"\bdbo\.", "", RegexOptions.IgnoreCase);
        }

        private string ConvertIdentity(string sql)
        {
            return Regex.Replace(sql,
                @"IDENTITY\s*\(\s*\d+\s*,\s*\d+\s*\)",
                "GENERATED ALWAYS AS IDENTITY",
                RegexOptions.IgnoreCase);
        }

        private string ConvertTopToLimit(string sql)
        {
            var match = Regex.Match(sql, @"SELECT\s+TOP\s+(\d+)\s+(.*?)\s+FROM",
                RegexOptions.IgnoreCase | RegexOptions.Singleline);

            if (match.Success)
            {
                string limitValue = match.Groups[1].Value;
                string selectClause = match.Groups[2].Value;

                sql = Regex.Replace(sql,
                    @"SELECT\s+TOP\s+\d+\s+",
                    "SELECT ",
                    RegexOptions.IgnoreCase);

                sql = sql.TrimEnd(';').Trim() + $" LIMIT {limitValue};";
                _warnings.Add("TOP converted to LIMIT");
            }

            return sql;
        }

        private string ConvertBooleanValues(string sql)
        {
            sql = Regex.Replace(sql, @"=\s*'true'", "= true", RegexOptions.IgnoreCase);
            sql = Regex.Replace(sql, @"=\s*'false'", "= false", RegexOptions.IgnoreCase);
            sql = Regex.Replace(sql, @"=\s*'1'", "= true");
            sql = Regex.Replace(sql, @"=\s*'0'", "= false");
            return sql;
        }

        public List<QueryLocation> ScanCSharpFile(string filePath)
        {
            var queries = new List<QueryLocation>();
            string content = File.ReadAllText(filePath);

            // Pattern for SQL queries in strings
            var patterns = new[]
            {
                @"""((?:SELECT|INSERT|UPDATE|DELETE|CREATE)[^""]+)""",
                @"@""((?:SELECT|INSERT|UPDATE|DELETE|CREATE)[^""]+?)"""
            };

            foreach (var pattern in patterns)
            {
                var matches = Regex.Matches(content, pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                foreach (Match match in matches)
                {
                    int lineNumber = content.Substring(0, match.Index).Count(c => c == '\n') + 1;
                    string query = match.Groups[1].Value;

                    queries.Add(new QueryLocation
                    {
                        LineNumber = lineNumber,
                        Query = query,
                        FilePath = filePath
                    });
                }
            }

            return queries;
        }

        public ProjectScanResult ScanProject(string projectRoot)
        {
            var result = new ProjectScanResult
            {
                FilesScanned = 0,
                QueriesFound = 0,
                Files = new Dictionary<string, List<QueryConversion>>()
            };

            var csFiles = Directory.GetFiles(projectRoot, "*.cs", SearchOption.AllDirectories)
                .Where(f => !f.Contains("\\bin\\") && !f.Contains("\\obj\\"));

            foreach (var file in csFiles)
            {
                var queries = ScanCSharpFile(file);

                if (queries.Any())
                {
                    result.FilesScanned++;
                    result.QueriesFound += queries.Count;

                    string relativePath = file.Replace(projectRoot, "").TrimStart('\\', '/');
                    result.Files[relativePath] = new List<QueryConversion>();

                    foreach (var query in queries)
                    {
                        var conversion = ConvertQuery(query.Query);
                        result.Files[relativePath].Add(new QueryConversion
                        {
                            LineNumber = query.LineNumber,
                            Original = conversion.Original,
                            Converted = conversion.Converted,
                            Warnings = conversion.Warnings,
                            Errors = conversion.Errors
                        });
                    }
                }
            }

            return result;
        }

        public void GenerateReport(string projectRoot, string outputFile = "migration_report.json")
        {
            var result = ScanProject(projectRoot);

            string json = JsonConvert.SerializeObject(result, Formatting.Indented);
            File.WriteAllText(outputFile, json);

            Console.WriteLine($"✓ Report generated: {outputFile}");
            Console.WriteLine($"  Files scanned: {result.FilesScanned}");
            Console.WriteLine($"  Queries found: {result.QueriesFound}");
        }
    }

    public class ConversionResult
    {
        public bool Success { get; set; }
        public string Original { get; set; }
        public string Converted { get; set; }
        public List<string> Warnings { get; set; }
        public List<string> Errors { get; set; }
    }

    public class QueryLocation
    {
        public int LineNumber { get; set; }
        public string Query { get; set; }
        public string FilePath { get; set; }
    }

    public class QueryConversion
    {
        public int LineNumber { get; set; }
        public string Original { get; set; }
        public string Converted { get; set; }
        public List<string> Warnings { get; set; }
        public List<string> Errors { get; set; }
    }

    public class ProjectScanResult
    {
        public int FilesScanned { get; set; }
        public int QueriesFound { get; set; }
        public Dictionary<string, List<QueryConversion>> Files { get; set; }
    }

    // Example Console Application
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("SQL Server to PostgreSQL Migration Tool");
            Console.WriteLine("========================================\n");

            if (args.Length < 1)
            {
                Console.WriteLine("Usage:");
                Console.WriteLine("  SqlMigrationTool.exe <project_root_path>");
                Console.WriteLine("  SqlMigrationTool.exe --convert \"SELECT * FROM dbo.Menu\"");
                return;
            }

            var tool = new SqlMigrationTool();

            if (args[0] == "--convert" && args.Length > 1)
            {
                // Convert single query
                string query = args[1];
                var result = tool.ConvertQuery(query);

                Console.WriteLine("Original:");
                Console.WriteLine(result.Original);
                Console.WriteLine("\nConverted:");
                Console.WriteLine(result.Converted);

                if (result.Warnings.Any())
                {
                    Console.WriteLine("\nWarnings:");
                    foreach (var warning in result.Warnings)
                        Console.WriteLine($"  - {warning}");
                }
            }
            else
            {
                // Scan entire project
                string projectRoot = args[0];
                tool.GenerateReport(projectRoot);
            }

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}