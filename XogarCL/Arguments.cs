using System;
using System.Collections.Generic;
using System.Text;

namespace XogarCL
{
    internal class Arguments
    {
        private readonly IList<string> _arguments;

        [Argument(Name = "Random", Description = "Open a randomly selected game.")]
        public bool UseRandom { get; private set; }

        [Argument(Name = "Help", Description = "Show this message.")]
        public bool ShowUsage { get; private set; }

        [Argument(Name = "<game id>", Description = "The integer value of the specific game id to start.")]
        public long GameId { get; private set; }

        [Argument(Name = "SteamPath=", 
            Description = "Set the Steam install directory for the Xogar library. Saves the value to the config file.",
            Example = "Example SteamPath= \"C:\\program files\\steam\"")]
        public string SteamPath { get; private set; }

        public Arguments(IList<string> args)
        {
            _arguments = args;
        }

        public void Parse()
        {
            if (_arguments.Count <= 0)
            {
                ShowUsage = true;
                throw new ArgumentException("No arguments provided. Arguments required. ");
            }

            string firstArg = _arguments[0].ToLowerInvariant();

            if ("Help".Equals(firstArg, StringComparison.OrdinalIgnoreCase))
            {
                ShowUsage = true;
            }
            else if ("Random".Equals(firstArg, StringComparison.OrdinalIgnoreCase))
            {
                UseRandom = true;
            }
            else if ("SteamPath=".Equals(firstArg, StringComparison.OrdinalIgnoreCase))
            {
                if (_arguments.Count < 2)
                {
                    ShowUsage = true;
                    throw new ArgumentException("Steam install path is required as the second parameter here.");
                }
                SteamPath = _arguments[1];
            }
            else
            {
                try
                {
                    GameId = long.Parse(firstArg);
                }
                catch
                {
                    ShowUsage = true;
                    throw new ArgumentException("Invalid first argument.");
                }
            }
        }

        public string GetUsageStatement()
        {
            var statement = new StringBuilder();
            var properties = GetType().GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.GetProperty);
            const int padSize = 16;
            foreach (var property in properties)
            {
                var attributes = property.GetCustomAttributes(typeof(ArgumentAttribute), false);
                if (attributes != null)
                {
                    foreach (ArgumentAttribute attribute in attributes)
                    {
                        statement.AppendFormat("{0}{1}{2}", attribute.Name.PadRight(padSize), attribute.Description, Environment.NewLine);
                        if (!string.IsNullOrEmpty(attribute.Example))
                        {
                            statement.AppendLine(new string(' ', padSize) + attribute.Example);
                        }
                    }
                }
            }

            return statement.ToString();
        }

    }
}
