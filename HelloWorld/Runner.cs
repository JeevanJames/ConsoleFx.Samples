#region --- License & Copyright Notice ---
/*
ConsoleFx CommandLine Processing Library
Copyright 2015 Jeevan James

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/
#endregion

using ConsoleFx.Parser.Styles;
using ConsoleFx.Parser.Validators;
using ConsoleFx.Programs;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

using static ConsoleFx.Utilities.ConsoleEx;
using static System.Console;

namespace HelloWorld
{
    public enum BackupType
    {
        Full,
        Incremental,
    }

    internal static class Runner
    {
        private static bool Verbose;
        private static BackupType BackupType = BackupType.Full;
        private static List<string> Excludes = new List<string>();
        private static DirectoryInfo BackupDirectory;

        internal static int Run()
        {
            var app = new ConsoleProgram<WindowsParserStyle>(Handler);
            try
            {
                app.AddOption("verbose", "v")
                    .Flag(() => Verbose);
                app.AddOption("type", "t")
                    .ParametersRequired()
                    .ValidateWith(new EnumValidator<BackupType>() { Message = "Please specify either Full or Incremental for the backup type." })
                    .AssignTo(() => BackupType);
                app.AddOption("exclude", "e")
                    .Optional(int.MaxValue)
                    .ParametersRequired(int.MaxValue)
                    .ValidateWith(new RegexValidator(@"^[\w.*?]+$"))
                    .AddToList(() => Excludes);
                app.AddArgument()
                    .ValidateWith(new PathValidator(PathType.Directory))
                    .AssignTo(() => BackupDirectory, directory => new DirectoryInfo(directory));
                return app.Run();
            }
            catch (Exception ex)
            {
                return app.HandleError(ex);
            }
            finally
            {
                if (Debugger.IsAttached)
                    ReadLine();
            }
        }

        private static int Handler()
        {
            WriteLine($"{BackupType} backup requested for the directory {BackupDirectory}");
            if (Excludes.Count > 0)
            {
                WriteLine($"Following files to be excluded:");
                foreach (string exclude in Excludes)
                    WriteLine($"    {exclude}");
            }
            if (Verbose)
                WriteLine("Verbose output requested.");
            return 0;
        }
    }
}