using System.Text;
using MarkovChains;
using MyProgram;
using System;
using System.IO;
using System.Text.RegularExpressions;

// The main function mostly gives inputs to markov and, or compiles stuff
namespace MyProgram
{
    class Launcher
    {
        static void Main(string[] args)
        {
            Markov<string> sample;
            
            // Change the Path info ( Will update this later )
            var folder = new DirectoryInfo("C:/Users/omery/Desktop/Makrov/Markovw/MarkovChains/rest");

            string file = "Test.txt", dataFile = Path.Combine(folder.FullName, "Data.txt");

            string input = File.ReadAllText(Path.Combine(folder.FullName, "Test.txt"));
            
            // Pull the lines from the folder(test folder, change its contents to teach a new style) and train them
            void TrainSample(string folderPath, string fileName)
            {
                var files = Directory.GetFiles(folderPath, fileName, SearchOption.AllDirectories);
                if (files.Length == 0) throw new Exception("No sample files found.");
                sample = new Markov<string>(" ");
                foreach (var file in files)
                {
                    var content = new List<string>();
                    using (var sampleFile = new StreamReader(file))
                    {
                        while (!sampleFile.EndOfStream)
                        {
                            var line = sampleFile.ReadLine().Trim();
                            foreach (var entry in line.Split(' ', StringSplitOptions.RemoveEmptyEntries))
                            {
                                content.Add(entry);
                            }
                        }
                    }

                    if (content.Count > 6) sample.Train(content, 6);
                }
            }

            // Generate method is stationed inside the Markov Class and can be called from its objects
            // The style of writing can be controlled from the input file 'Test'
            // markov chain algorithm is completely dependent of the style of the author, not the algorithm
            // Stuff to do: Send certain keywords for elimination or name filling
            // Stuff to do 2: Create a specialized model and its output generator
            // (namely for name and chronological words gen)
            // The training algorithm does not require and can be used on both models, only new generators needed
            void GenerateOutput(int requiredWordCount)
            {
                var result = sample.Generate(requiredWordCount, true);
                var resultString = new StringBuilder();
                foreach (var entry in result)
                {
                    resultString.Append(entry + " ");
                }

                Console.Write(resultString.ToString());
            }

            // Stuff to do: Finish this next time you read this >:( REMINDER TO MYSELF ) 
            void LoadData(int requiredWordCount)
            {
                //this will load data
            }
            
            void DungeonMaster(string input)
            {
                // Initialize the output arrays
                List<string> Narr = new List<string>();
                List<string> Dev = new List<string>();
                List<string> Con = new List<string>();

                // Use a regular expression to split the input into sections
                string[] sections = Regex.Split(input, @"(?=\[Begin)");

                // Loop through each section and add it to the appropriate output array
                foreach (string section in sections)
                {
                    // Use a regular expression to extract the section type
                    Match typeMatch = Regex.Match(section, @"\[Begin(\w+)\]");
                    if (typeMatch.Success)
                    {
                        string type = typeMatch.Groups[1].Value;

                        // Add the section to the appropriate output array based on its type
                        switch (type)
                        {
                            case "Narr":
                                Narr.Add(Regex.Replace(section, @"\[.*?\]", "").Trim());
                                break;
                            case "Dev":
                                Dev.Add(Regex.Replace(section, @"\[.*?\]", "").Trim());
                                break;
                            case "Con":
                                Con.Add(Regex.Replace(section, @"\[.*?\]", "").Replace("======", "").Trim());
                                break;
                            default:
                                Console.WriteLine($"Unknown section type: {type}");
                                break;
                        }
                    }
                    else
                    {
                        //Console.WriteLine($"Section without a valid type: {section}");
                    }
                }

                // Print the output arrays
                //Console.WriteLine("\n Narr:");
                //Console.WriteLine(string.Join("\n", Narr));
                //Console.WriteLine("\n Dev:");
                //Console.WriteLine(string.Join("\n", Dev));
                //Console.WriteLine("\n Con:");
                //Console.WriteLine(string.Join("\n", Con));
                
                using (StreamWriter writer = new StreamWriter(dataFile)) {
                    // Write each string to the file
                    foreach (string str in Narr) {
                        writer.WriteLine(str);
                    }
                }
                TrainSample(folder.FullName, "Data.txt");
                GenerateOutput(40);
                
                using (StreamWriter writer = new StreamWriter(dataFile)) {
                    // Write each string to the file
                    foreach (string str in Dev) {
                        writer.WriteLine(str);
                    }
                }
                TrainSample(folder.FullName, "Data.txt");
                GenerateOutput(20);
                
                using (StreamWriter writer = new StreamWriter(dataFile)) {
                    // Write each string to the file
                    foreach (string str in Con) {
                        writer.WriteLine(str);
                    }
                }
                TrainSample(folder.FullName, "Data.txt");
                GenerateOutput(20);
                
            }
            
            
            // Read Train Sample and GenerateOutput by order to understand how the code works
            // Training creates an usable database of Sequence objects for the generation
            // The generating writes out the output by using markov mathematical algorithm
            // Generate output after all prints the result

            DungeonMaster(input);
            
            //TrainSample(folder.FullName, file);
            
            //GenerateOutput(200);
        }
    }
}