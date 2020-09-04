using System;
using System.Collections.Generic;
using System.Text;
using CommandLine;
using CommandLine.Text;

namespace BTSaveEditor
{
    class Options
    {
        [Value(0, Required = true, MetaName = "save-file-path", HelpText = "Path to a save file")]
        public string saveName { get; set; }

        [Option('w', "write", HelpText = "Create a save file from gamesession and sub files")]
        public bool write { get; set; }

        [Option("sub", HelpText = "submarine file (compressed)")]
        public string subFile { get; set; }

        [Option("xml", HelpText = "submarine file (in xml format)")]
        public string subXmlFile { get; set; }

        [Option('s', "session", HelpText = "game session file (in xml format)")]
        public string sessionFile { get; set; }

        [Usage(ApplicationAlias = "BTSaveEditor")]
        public static IEnumerable<Example> Examples
        {
            get
            {
                return new List<Example>() {
                    new Example("Extract save file", new Options { saveName = "my_save_file.save" }),
                    new Example("Create save file from compressed sub file", new Options { 
                        saveName = "my_save_file.save", 
                        write = true, 
                        sessionFile = "gamesession.xml",
                        subFile = "my_submarine.sub"
                    }),
                    new Example("Create save file from XML sub file", new Options {
                        saveName = "my_save_file.save",
                        write = true,
                        sessionFile = "gamesession.xml",
                        subXmlFile = "my_submarine.sub.xml"
                    })

                };
            }
        }
    }
}
