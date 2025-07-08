namespace LeaderboardApp.Services
{
    public class NicknameService
    {
        public static string GetNickName()
        {
            // List of nicknames
            var nicknames = new List<string>
            {
                "Web wizard", "Code demon", "King FF", "Code Joker", "Code Breaker", "Code dreamer",
                "Internet Guru", "Cookies", "Dev Lord", "Phoenix", "Page maker", "Cool Dev",
                "Tech Wolf", "Code King", "Code Lord", "Code Wiz", "Phileoreal", "Dev Whale",
                "Byte", "Dev Master", "King Programmer", "Supermutec", "Megatech", "Tech Beast",
                "Super Tech", "Data Pirate", "Hex clan", "Debug", "Zip demon", "Java Delia",
                "Java Monster", "Binary Beast", "Web Ghost", "Server Monk", "Lan Blast", "Bot Interpreter",
                "Patch Demon", "Terminal stack", "Chi monster", "Runtime Terror", "Portal Pirate",
                "Bug burster", "Scraper Nerd", "Babel Frontfire", "Faceless Dev", "Regex native",
                "App monster", "Cute Programmer", "CodeMaestro", "ByteWizard", "DevVirtuoso",
                "PixelPioneer", "SyntaxSorcerer", "BitBard", "QuantumQuasar", "DebugDynamo",
                "AlgorithmAlchemist", "TechTrooper", "ScriptSculptor", "BinaryBard", "CoderPhoenix",
                "CryptoCrusader", "CodeCommander", "DataDynamo", "LogicLion", "HackHarbinger",
                "FunctionFalcon", "CtrlSultan", "EchoEnigma", "KernelKing", "VectorVoyager",
                "NodeNinja", "LoopLion", "CrashCommando", "CipherScribe", "BitBison", "ScriptSorcerer",
                "DebugDynamo", "JavaJuggernaut", "RustRaider", "CryptoCrafter", "QuantumQuasar",
                "PixelPioneer", "CodeChampion", "TechTrailblazer", "BinaryBard", "CompilerConqueror",
                "SyntaxSculptor", "CodeCraftsman", "DataDynamo", "AlgorithmArchitect", "ScriptSorcerer",
                "BitBison", "LoopLion", "CodeCzar", "LogicLion", "ByteBuccaneer", "DebugDrifter",
                "NodeNebula", "KernelKhan", "EchoExpedition", "QuantumQuester", "HackHero",
                "FunctionFrontier", "CtrlCommando", "CodeCenturion", "SyntaxScribe", "BinaryBard",
                "DataDynamo", "AlgorithmAviator", "ScriptSculptor", "BitBison", "LoopLion",
                "CodeCrusader", "LogicLion", "ByteBarrister", "DebugDynamo", "NodeNavigator",
                "KernelKaiser", "EchoEngineer", "QuantumQuintessence", "HackHercules", "FunctionFrontrunner",
                "CtrlChampion", "CodeCaptain", "SyntaxSculptor", "BinaryBard", "DataDynamo",
                "AlgorithmArtisan", "ScriptSorcerer", "BitBison", "LoopLion", "CodeConductor",
                "LogicLion", "ByteBattleship", "DebugDynamo", "NodeNomad", "KernelKaiser",
                "EchoEngineer", "QuantumQuintessence", "HackHercules", "FunctionFuturist",
                "CtrlConqueror", "CodeCrusader", "SyntaxSculptor", "BinaryBard", "DataDynamo",
                "AlgorithmAdept", "ScriptSorcerer", "BitBison", "LoopLion", "CodeCraftsman",
                "LogicLion", "ByteBehemoth", "DebugDynamo", "NodeNurturer", "KernelKingpin",
                "EchoEmissary", "QuantumQuasar", "HackHeroic", "FunctionFrontiersman", "CtrlCrusader",
                "CodeCognoscente", "SyntaxSculptor", "BinaryBard", "DataDynamo", "AlgorithmAmbassador",
                "ScriptSorcerer", "BitBison", "LoopLion", "CodeCommandant", "LogicLion",
                "ByteBounty", "DebugDynamo", "NodeNoble", "KernelKeeper", "EchoEnvoy", "QuantumQuestor",
                "HackHighness", "FunctionFuturist", "CtrlChieftain", "CodeCzar", "SyntaxSculptor",
                "BinaryBard", "DataDynamo", "AlgorithmArchitect", "ScriptSorcerer", "BitBison",
                "LoopLion", "CodeCrusader", "LogicLion", "ByteBarrister", "DebugDynamo", "NodeNavigator",
                "KernelKaiser", "EchoEngineer", "QuantumQuintessence", "HackHercules", "FunctionFrontrunner",
                "CtrlChampion", "CodeCaptain", "SyntaxSculptor", "BinaryBard", "DataDynamo",
                "AlgorithmArtisan", "ScriptSorcerer", "BitBison", "LoopLion", "CodeConductor",
                "LogicLion", "ByteBattleship", "DebugDynamo", "NodeNomad", "KernelKaiser",
                "EchoEngineer", "QuantumQuintessence", "HackHercules", "FunctionFuturist"
            };

            // Select a random nickname from the list
            var random = new Random();
            return nicknames[random.Next(nicknames.Count)];
        }
    }
}
