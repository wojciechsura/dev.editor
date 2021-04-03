using Dev.Editor.BusinessLogic.Models.SubstitutionCipher;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.Services.SubstitutionCipher
{
    class SubstitutionCipherService : ISubstitutionCipherService
    {
        // Private constants --------------------------------------------------

        private const int MinChars = 2;
        private const int MaxChars = 4;
        private const double MinimumSequenceFrequency = 0.0000000001;
        private const double MinimumSequenceFitness = -10.0; // Log10(MinimumSequenceFrequency)
        private const string Header = "LANGINFO";

        private const int MaxIterations = 10000;
        private const int MaxIterationsWithoutImprovement = 1000;

        // Private types ------------------------------------------------------

        private class UniqueValueComparer : IEqualityComparer<KeyValuePair<char, char>>
        {
            public bool Equals(KeyValuePair<char, char> x, KeyValuePair<char, char> y)
            {
                return x.Value.Equals(y.Value);
            }

            public int GetHashCode(KeyValuePair<char, char> obj)
            {
                return obj.Value.GetHashCode();
            }
        }

        // Private methods ----------------------------------------------------

        private void ValidateSeqStats(Dictionary<int, Dictionary<string, SequenceInfoModel>> sequences)
        {
            foreach (var seq in sequences)
            {
                if (seq.Value.Any(kvp => kvp.Key.Any(ch => !Char.IsLetter(ch) || !Char.IsLower(ch))))
                    throw new ArgumentException("Invalid sequence statistics (entry with non-lowercase letter key!");
                if (seq.Value.Any(kvp => kvp.Value.Frequency <= 0.0 || kvp.Value.Frequency >= 1.0))
                    throw new ArgumentException("Invalid sequence statistics (entry must have frequency between 0.0 and 1.0)!");
                if (seq.Value.Any(kvp => kvp.Key.Length != seq.Key))
                    throw new ArgumentException("Invalid entry in sequence statistics (string length does not match required sequence length)!");
            }
        }

        private void ValidateLetterStats(Dictionary<char, double> letters)
        {
            if (letters.Any(kvp => !Char.IsLetter(kvp.Key) || !Char.IsLower(kvp.Key)))
                throw new ArgumentException("Invalid letter statistics key (should be a lowercase letter!)");
            if (letters.Any(kvp => kvp.Value < 0.0 || kvp.Value > 1.0))
                throw new ArgumentException("Invalid letter frequency (should be between 0.0 and 1.0)!");
        }

        private static double FitnessFromFrequency(double freq)
        {
            if (freq <= 0.0 || freq >= 1.0)
                throw new ArgumentException("Invalid frequency!");

            return Math.Log10(Math.Max(MinimumSequenceFrequency, freq));
        }

        private LanguageStatisticsModel GenerateLanguageStatisticsModel(string[] lines, Func<bool> checkCancellation = null, Action<int> reportProgress = null)
        {
            var model = new LanguageStatisticsModel();
            for (int i = MinChars; i <= MaxChars; i++)
                model.Sequences[i] = new Dictionary<string, int>();

            for (int l = 0; l < lines.Length; l++)
            {
                if (checkCancellation?.Invoke() ?? false)
                    return null;

                reportProgress?.Invoke(l * 100 / lines.Length);

                string line = lines[l];

                char[] chars = new char[MaxChars];
                int nextChar = 0;
                int totalChars = 0;

                foreach (var ch in line)
                {
                    if (char.IsLetter(ch))
                    {
                        var lowerCh = Char.ToLowerInvariant(ch);

                        if (model.LetterStats.ContainsKey(lowerCh))
                            model.LetterStats[lowerCh]++;
                        else
                            model.LetterStats[lowerCh] = 1;

                        chars[nextChar++] = lowerCh;
                        nextChar %= MaxChars;
                        totalChars++;

                        if (totalChars >= MinChars)
                        {
                            StringBuilder sb = new StringBuilder();
                            int index = (nextChar - Math.Min(totalChars, MaxChars));
                            if (index < 0)
                                index += MaxChars;

                            for (int i = 1; i < MinChars; i++)
                                sb.Append(chars[(index + i - 1) % MaxChars]);

                            for (int i = MinChars; i <= Math.Min(MaxChars, totalChars); i++)
                            {
                                sb.Append(chars[(index + i - 1) % MaxChars]);
                                var seq = sb.ToString();

                                if (model.Sequences[i].ContainsKey(seq))
                                    model.Sequences[i][seq]++;
                                else
                                    model.Sequences[i][seq] = 1;
                            }
                        }
                    }
                    else
                    {
                        nextChar = 0;
                        totalChars = 0;
                    }
                }
            }

            return model;
        }

        private double EvalFitness(string text, LanguageInfoModel languageInfo, int charsToCompare)
        {
            double result = 0.0;

            char[] buffer = new char[charsToCompare];

            int index = 0;
            int bufferIndex = 0;
            int letterCount = 0;

            while (index < text.Length && letterCount < charsToCompare - 1)
            {
                if (char.IsLetter(text[index]))
                {
                    buffer[bufferIndex++] = Char.ToLowerInvariant(text[index]);
                    letterCount++;
                }

                index++;
            }

            for (; index < text.Length; index++)
            {
                if (char.IsLetter(text[index]))
                {
                    buffer[bufferIndex++] = Char.ToLowerInvariant(text[index]);
                    bufferIndex %= charsToCompare;

                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < charsToCompare; i++)
                        sb.Append(char.ToLowerInvariant(buffer[(bufferIndex + i) % charsToCompare]));

                    var key = sb.ToString();
                    if (languageInfo.SequenceFrequencies[charsToCompare].ContainsKey(key))
                        result += languageInfo.SequenceFrequencies[charsToCompare][key].Fitness;
                    else
                        result += MinimumSequenceFitness;
                }
            }

            return result;
        }

        // Public methods -----------------------------------------------------

        public LanguageInfoModel BuildLanguageInfoModel(string[] lines, Func<bool> checkCancellation, Action<int> reportProgress)
        {
            var model = GenerateLanguageStatisticsModel(lines, checkCancellation, reportProgress);

            // Sanity check

            if (model.LetterStats.Any(l => !char.IsLower(l.Key)))
                throw new ArgumentException("Letter keys must be all lowercase!");
            if (model.LetterStats.Any(l => l.Value <= 0))
                throw new ArgumentException("Invalid letter statistics (entry with occurences equal to 0 or less)");

            if (model.Sequences.Any(seq => seq.Value.Any(kvp => kvp.Key.Any(ch => !Char.IsLower(ch)))))
                throw new ArgumentException("Invalid sequence statistics (entry with non-lowercase key)!");
            if (model.Sequences.Any(seq => seq.Value.Any(kvp => kvp.Value <= 0)))
                throw new ArgumentException("Invalid sequence statistics (entry with occurences equal to 0 or less)!");
            if (model.Sequences.Any(seq => seq.Value.Any(kvp => kvp.Key.Length != seq.Key)))
                throw new ArgumentException("Invalid entry in sequence statistics (string length does not match required sequence length)!");

            // Letters

            var letterSum = model.LetterStats
                .Sum(kvp => kvp.Value);

            Dictionary<char, double> letterFreqs;
            if (letterSum > 0)
                letterFreqs = model.LetterStats
                    .Select(kvp => new KeyValuePair<char, double>(kvp.Key, (float)kvp.Value / letterSum))
                    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            else
                letterFreqs = new Dictionary<char, double>();

            var sequenceFreqs = new Dictionary<int, Dictionary<string, SequenceInfoModel>>();
            foreach (var sequenceKvp in model.Sequences)
            {
                var sum = sequenceKvp.Value.Sum(kvp => kvp.Value);

                if (sum > 0)
                    sequenceFreqs[sequenceKvp.Key] = sequenceKvp.Value
                        .Select(kvp => new KeyValuePair<string, double>(kvp.Key, (double)kvp.Value / sum))
                        .ToDictionary(kvp => kvp.Key, kvp => new SequenceInfoModel(kvp.Value, FitnessFromFrequency(kvp.Value)));
                else
                    sequenceFreqs[sequenceKvp.Key] = new Dictionary<string, SequenceInfoModel>();
            }

            return new LanguageInfoModel(letterFreqs, sequenceFreqs);
        }

        public void SaveLanguageInfoModel(string filename, LanguageInfoModel model)
        {
            using (FileStream fs = new FileStream(filename, FileMode.Create, FileAccess.Write))
            using (BinaryWriter writer = new BinaryWriter(fs))
            {
                // Header

                writer.Write(Header);
                writer.Write(1); // Version 1

                // Letter statistics

                writer.Write(model.LetterFrequencies.Count);

                foreach (var kvp in model.LetterFrequencies)
                {
                    writer.Write(kvp.Key);
                    writer.Write(kvp.Value);
                }

                // Sequence statistics

                writer.Write(model.SequenceFrequencies.Count);

                foreach (var seqKvp in model.SequenceFrequencies)
                {
                    writer.Write(seqKvp.Key);

                    writer.Write(seqKvp.Value.Count);

                    foreach (var kvp in seqKvp.Value)
                    {
                        writer.Write(kvp.Key);
                        writer.Write(kvp.Value.Frequency);
                    }
                }
            }
        }

        public LanguageInfoModel LoadLanguageInfoModel(string filename)
        {
            var letters = new Dictionary<char, double>();
            var sequences = new Dictionary<int, Dictionary<string, SequenceInfoModel>>();

            using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
            using (BinaryReader reader = new BinaryReader(fs))
            {
                // Header

                var header = reader.ReadString();
                if (header != Header)
                    throw new IOException("Invalid language file header!");

                var version = reader.ReadInt32();
                if (version != 1)
                    throw new IOException("Unsupported language file version!");

                // Letter statistics

                int count = reader.ReadInt32();
                for (int i = 0; i < count; i++)
                {
                    var letter = reader.ReadChar();
                    var freq = reader.ReadDouble();

                    letters[letter] = freq;
                }

                // Sequence statistics

                int sequenceLengthCount = reader.ReadInt32();

                for (int i = 0; i < sequenceLengthCount; i++)
                {
                    int seqLength = reader.ReadInt32();

                    int seqCount = reader.ReadInt32();

                    var seqInfo = new Dictionary<string, SequenceInfoModel>();

                    for (int j = 0; j < seqCount; j++)
                    {
                        string seq = reader.ReadString();
                        double freq = reader.ReadDouble();

                        seqInfo[seq] = new SequenceInfoModel(freq, FitnessFromFrequency(freq));
                    }

                    sequences[seqLength] = seqInfo;
                }
            }

            // Validate

            ValidateLetterStats(letters);
            ValidateSeqStats(sequences);

            return new LanguageInfoModel(letters, sequences);
        }

        public string Process(Dictionary<char, char> inputKey, string data, bool forward, Func<bool> checkCancellation = null)
        {
            if (data == null)
                return null;

            if (inputKey == null)
                throw new InvalidOperationException("Key cannot be null!");
            if (inputKey.Any(kvp => (char.IsLetter(kvp.Key) && !char.IsLower(kvp.Key)) || (char.IsLetter(kvp.Value) && !char.IsLower(kvp.Value))))
                throw new InvalidOperationException("Only lowercase letters are allowed in the key!");

            Dictionary<char, char> key;
            if (forward)
                key = inputKey;
            else
            {
                key = inputKey
                    .Distinct(new UniqueValueComparer())
                    .ToDictionary(kvp => kvp.Value, kvp => kvp.Key);
            }

            StringBuilder result = new StringBuilder();

            foreach (var dataChar in data)
            {
                bool isUpper = char.IsUpper(dataChar);

                var dataCharKey = char.ToLowerInvariant(dataChar);
                if (key.ContainsKey(dataCharKey))
                {
                    if (isUpper)
                        result.Append(char.ToUpper(key[dataCharKey]));
                    else
                        result.Append(key[dataCharKey]);
                }
                else
                {
                    if (Char.IsWhiteSpace(dataChar))
                        result.Append(dataChar);
                    else
                        result.Append("·");
                }

                if (checkCancellation?.Invoke() ?? false)
                {
                    return null;
                }
            }

            return result.ToString();
        }

        public Dictionary<char, char> TrySolve(string cipher, 
            Dictionary<char, char> initialKey, 
            LanguageInfoModel languageInfo, 
            Func<bool> checkCancellation = null, 
            Action<int> reportProgress = null)
        {
            var Random = new Random((int)DateTime.Now.Ticks);

            int charsToCompare = 2;

            var possiblePlaintext = Process(initialKey, cipher, false);
            var bestFitness = EvalFitness(possiblePlaintext, languageInfo, charsToCompare);
            var bestKey = new Dictionary<char, char>(initialKey);

            var iterationsWithoutProgress = 0;

            for (int i = 0; i < MaxIterations; i++)
            {
                reportProgress?.Invoke(i * 100 / MaxIterations);

                var key = new Dictionary<char, char>(initialKey);
                var keys = key.Keys.ToList();
                var values = key.Values.ToList();

                for (int j = values.Count - 1; j >= 0; j--)
                {
                    // Replace j-th entry with random one
                    int rndIndex = Random.Next(j + 1);

                    var tmp = values[rndIndex];
                    values[rndIndex] = values[j];
                    values[j] = tmp;

                    key[keys[j]] = values[j];
                }

                int index1 = 0;
                int index2 = 1;

                possiblePlaintext = Process(key, cipher, false);
                var fitness = EvalFitness(possiblePlaintext, languageInfo, charsToCompare);

                while (true)
                {
                    if (checkCancellation?.Invoke() ?? false)
                        return null;

                    // Randomize two indices to exchange
                    char cipher1 = key[keys[index1]];
                    char cipher2 = key[keys[index2]];

                    // Exchange key entries
                    key[keys[index1]] = cipher2;
                    key[keys[index2]] = cipher1;

                    // Try to uncipher the message
                    possiblePlaintext = Process(key, cipher, false);

                    // Evaluate new fitness
                    var newFitness = EvalFitness(possiblePlaintext, languageInfo, charsToCompare);

                    if (newFitness > fitness)
                    {
                        // Change stays
                        fitness = newFitness;

                        // Repeat replacing key from the beginning
                        index1 = 0;
                        index2 = 1;
                    }
                    else
                    {
                        // Revert change
                        key[keys[index1]] = cipher1;
                        key[keys[index2]] = cipher2;

                        // Try next replacement
                        index2++;
                        if (index2 >= keys.Count)
                        {
                            index1++;
                            index2 = index1 + 1;
                        }

                        if (index2 >= keys.Count)
                            break;
                    }
                }

                // Fitness now represents best fitness from this iteration

                if (fitness > bestFitness)
                {
                    System.Diagnostics.Debug.WriteLine($"Fitness improved from {bestFitness} to {fitness} for key {String.Join("", key.Values)}");

                    bestKey = new Dictionary<char, char>(key);
                    bestFitness = fitness;

                    iterationsWithoutProgress = 0;
                }
                else
                {
                    iterationsWithoutProgress++;
                }

                if (iterationsWithoutProgress > MaxIterationsWithoutImprovement)
                    break;
            }
 
            // Return the best key found
            return bestKey;
        }
    }
}
