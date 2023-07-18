namespace MarkovChains
{
    class Markov<T> where T : IComparable
    {
        private readonly List<Sequence<T>> trainedSequences = new List<Sequence<T>>();
        private readonly Random rand = new Random();
        private readonly T stopKey;
        
        // Sequence class will receive the entries as keys, and remember words as entrychains
        private class Sequence<T> where T : IComparable
        {
            public T Entry { get; set; }
            public List<T> EntryChain { get; set; }
        }

        // stopKey is important, don't mess with its creation while simultaneously generating data for later use
        public Markov(T stopKey)
        {
            this.stopKey = stopKey;
        }
        
        // Train method receives lines as list of words and creates sequence class objects from them
        // The sequence objects have a limit to the words it can remember, the limit is stated in order variable
        // Stuff to do: Change the way the output is created and station it outside of the method
        // by iterating trough the class objects, make it so the data accumulates instead of resetting every seance
        public void Train(List<T> trainingEntries, int order)
        {
            if (trainingEntries.Count < order)
            {
                throw new InvalidOperationException("Insufficient training entries to form a sequence.");
            }

            for (int idx = 0; idx < trainingEntries.Count; idx++)
            {
                T key = trainingEntries[idx];
                if (key.CompareTo(stopKey) != 0)
                {
                    var entryChain = new List<T>();
                    for (int i = 0; i < order; i++)
                    {
                        T e = trainingEntries[(idx + i + 1) % trainingEntries.Count];
                        entryChain.Add(e);
                        if (e.CompareTo(stopKey) == 0)
                        {
                            break;
                        }
                    }
                    if (entryChain.Count > 0)
                    {
                        var entry = new Sequence<T>
                        {
                            Entry = key,
                            EntryChain = entryChain
                        };
                        trainedSequences.Add(entry);
                    }
                }
            }
            
            // var fileOut = new StreamWriter(output);
            // foreach (var word in trainedSequences)
            {
                // Console.WriteLine(word.Entry + " " + string.Join(", ", word.EntryChain));
                // fileOut.WriteLine(word.Entry + " " + string.Join(", ", word.EntryChain));
                // This side part should fill the data file, but is not ready yet
            }
        }
        
        // This part uses markov mathematical algorithm to combine the previously trained words
        // keys and sequences are combined here until the desired length of words are reached here
        // Things to do: add a separate input system that will be independent from the trained sequence limit
        public List<T> Generate(int Length, bool includeStopKey)
        {
            List<T> result = new List<T>();

            // Select a random word that starts with "-"
            List<T> startingKeys = trainedSequences
                .Select(seq => seq.Entry)
                .Where(key => key.ToString().StartsWith("-"))
                .ToList();

            if (startingKeys.Count == 0)
            {
                throw new InvalidOperationException("No keys available to start the loop.");
            }

            T key = startingKeys[rand.Next(0, startingKeys.Count)];
            result.Add(key);

            while (result.Count < Length || !result.Last().ToString().EndsWith("."))
            {
                int sequences = trainedSequences.Count(source => source.Entry.CompareTo(key) == 0);
                if (sequences > 0)
                {
                    int wantedSequence = rand.Next(0, sequences);
                    sequences = 0;
                    foreach (var sequence in trainedSequences.Where(source => source.Entry.CompareTo(key) == 0))
                    {
                        if (sequences == wantedSequence)
                        {
                            foreach (var newEntry in sequence.EntryChain)
                            {
                                if ((newEntry.CompareTo(stopKey) != 0) || includeStopKey)
                                {
                                    result.Add(newEntry);
                                    key = newEntry;
                                    if (result.Count > Length && newEntry.ToString().EndsWith("."))
                                    {
                                        break;
                                    }
                                }
                            }
                            if (result.Count > Length || result.Last().ToString().EndsWith("."))
                            {
                                break;
                            }
                        }
                        sequences++;
                    }
                }
                else
                {
                    key = trainedSequences[rand.Next(0, trainedSequences.Count)].Entry;
                    result.Add(key);
                }
            }

            return result;
        }

    }
}
