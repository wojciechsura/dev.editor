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
        private const int MaxCharsPerAlphabet = 256;
        private const double MinimumSequenceFrequency = 0.0000000001;
        private const double MinimumSequenceFitness = -10.0; // Log10(MinimumSequenceFrequency)
        private const string Header = "LANGINFO";

        private const int MaxIterations = 10000;
        private const int MaxIterationsWithoutImprovement = 300;

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

        private void ValidateSeqStats(Dictionary<int, SequenceInfoModel[]> sequences)
        {
            foreach (var seq in sequences)
            {
                if (seq.Value.Any(seqInfo => seqInfo.Frequency <= 0.0 || seqInfo.Frequency >= 1.0))
                    throw new ArgumentException("Invalid sequence statistics (entry must have frequency between 0.0 and 1.0)!");
            }
        }

        private void ValidateAlphabet(Dictionary<char, int> alphabet)
        {
            if (alphabet.Count > MaxCharsPerAlphabet)
                throw new ArgumentException("Too many characters in the alphabet!");
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

        private LanguageStatisticsModel GenerateLanguageStatisticsModel(string[] lines, 
            Dictionary<char, int> alphabet, 
            Func<bool> checkCancellation = null, 
            Action<int> reportProgress = null)
        {
            if (alphabet.Count > MaxCharsPerAlphabet)
                throw new ArgumentException($"Maximum of {MaxCharsPerAlphabet} characters per alphabet is supported!");

            int bitsPerChar = 1;
            int maxAlphabetEntries = 2;

            while (maxAlphabetEntries < alphabet.Count)
            {
                bitsPerChar++;
                maxAlphabetEntries <<= 1;
            }

            // Building model from all possible combinations of digrams, trigrams, quadgrams
            var model = new LanguageStatisticsModel(bitsPerChar);
            for (int i = MinChars; i <= MaxChars; i++)
                model.Sequences[i] = new int[1 << (bitsPerChar * i)];
            foreach (var ch in alphabet.Keys)
                model.LetterStats[ch] = 0;

            for (int l = 0; l < lines.Length; l++)
            {
                if (checkCancellation?.Invoke() ?? false)
                    return null;

                reportProgress?.Invoke(l * 100 / lines.Length);

                string line = lines[l];

                var current = new int[MaxChars + 1];
                var mask = new int[MaxChars + 1];
                for (int i = 0; i <= MaxChars; i++)
                {
                    current[i] = 0;
                    mask[i] = (1 << (bitsPerChar * i)) - 1;
                }

                int totalChars = 0;

                foreach (var ch in line)
                {
                    var lowerCh = char.ToLowerInvariant(ch);
                    bool isLetter = alphabet.TryGetValue(lowerCh, out int letterCode);

                    if (isLetter)
                    {
                        model.LetterStats[lowerCh]++;

                        totalChars++;

                        for (int i = MinChars; i <= MaxChars; i++)
                            current[i] = ((current[i] << bitsPerChar) + letterCode) & mask[i];

                        for (int i = MinChars; i <= Math.Min(totalChars, MaxChars); i++)
                        {
                            // Increment count of i-grams for current i-gram codes
                            model.Sequences[i][current[i]]++;
                        }
                    }
                    else
                    {
                        // If we encountered a non-letter, word ended,
                        // so we reset current values and start over.

                        for (int i = MinChars; i <= MaxChars; i++)
                            current[i] = 0;
                        totalChars = 0;
                    }
                }
            }

            return model;
        }

        private double EvalFitness(string cipher, 
            Dictionary<char, char> decodingKey, 
            LanguageInfoModel languageInfo, 
            int charsToCompare)
        {
            // If cipher is too short to begin with, return worst possible value
            if (cipher.Length < charsToCompare)
                return Double.MinValue;

            double result = 0.0;

            int current = 0;
            int mask = (1 << (languageInfo.BitsPerChar * charsToCompare)) - 1;

            int charsProcessed = 0;
            int currentChar = 0;
            while (currentChar < cipher.Length)
            {
                var ch = Char.ToLowerInvariant(cipher[currentChar]);
                if (decodingKey.TryGetValue(ch, out char decoded))
                {
                    if (languageInfo.Alphabet.TryGetValue(decoded, out int code))
                    {
                        current = ((current << languageInfo.BitsPerChar) + code) & mask;
                        charsProcessed++;

                        if (charsProcessed >= charsToCompare)
                        {
                            result += languageInfo.SequenceFrequencies[charsToCompare][current].Fitness;
                        }
                    }
                }

                currentChar++;
            }

            if (charsProcessed < charsToCompare)
                return Double.MinValue;

            return result;
        }

        // Public methods -----------------------------------------------------

        public LanguageInfoModel BuildLanguageInfoModel(string[] lines, 
            string alphabet, 
            Func<bool> checkCancellation, 
            Action<int> reportProgress)
        {
            // Speeds up letter code lookup in the alphabet
            
            var alphabetDict = new Dictionary<char, int>();
            for (int i = 0; i < alphabet.Length; i++)
                alphabetDict[char.ToLowerInvariant(alphabet[i])] = i;

            var langStats = GenerateLanguageStatisticsModel(lines, alphabetDict, checkCancellation, reportProgress);

            // Sanity check

            if (langStats.LetterStats.Any(l => !char.IsLower(l.Key)))
                throw new ArgumentException("Letter keys must be all lowercase!");
            if (langStats.LetterStats.Any(l => l.Value <= 0))
                throw new ArgumentException("Invalid letter statistics (entry with occurences equal to 0 or less)");

            // Letters

            var letterSum = langStats.LetterStats
                .Sum(kvp => kvp.Value);

            Dictionary<char, double> letterFreqs;
            if (letterSum > 0)
                letterFreqs = langStats.LetterStats
                    .Select(kvp => new KeyValuePair<char, double>(kvp.Key, (float)kvp.Value / letterSum))
                    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            else
                letterFreqs = new Dictionary<char, double>();

            var sequenceFreqs = new Dictionary<int, SequenceInfoModel[]>();
            foreach (var seq in langStats.Sequences)
            {
                var sum = seq.Value.Sum();

                if (sum > 0)
                    sequenceFreqs[seq.Key] = seq.Value
                        .Select(v => new SequenceInfoModel(v, Math.Max(MinimumSequenceFrequency, (double)v / sum), FitnessFromFrequency(Math.Max(MinimumSequenceFrequency, (double)v / sum))))
                        .ToArray();
                else
                    sequenceFreqs[seq.Key] = seq.Value
                        .Select(v => new SequenceInfoModel(v, MinimumSequenceFrequency, MinimumSequenceFitness))
                        .ToArray();
            }

            return new LanguageInfoModel(langStats.BitsPerChar, alphabetDict, letterFreqs, sequenceFreqs);
        }

        public void SaveLanguageInfoModel(string filename, LanguageInfoModel model)
        {
            using (FileStream fs = new FileStream(filename, FileMode.Create, FileAccess.Write))
            using (BinaryWriter writer = new BinaryWriter(fs))
            {
                // Header

                writer.Write(Header);
                writer.Write(1); // Version 1

                // Alphabet

                writer.Write(model.BitsPerChar);
                writer.Write(model.Alphabet.Count);

                foreach (var kvp in model.Alphabet)
                {
                    writer.Write(kvp.Key);
                    writer.Write(kvp.Value);
                }

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
                    writer.Write(seqKvp.Value.Length);

                    foreach (var seqModel in seqKvp.Value)
                    {
                        writer.Write(seqModel.Count);
                        writer.Write(seqModel.Frequency);
                        // Fitness can be evaluated from frequency
                    }
                }
            }
        }

        public LanguageInfoModel LoadLanguageInfoModel(string filename)
        {
            var alphabet = new Dictionary<char, int>();
            var letters = new Dictionary<char, double>();
            var sequences = new Dictionary<int, SequenceInfoModel[]>();
            int bitsPerChar;

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

                // Alphabet

                bitsPerChar = reader.ReadInt32();

                int alphabetSize = reader.ReadInt32();
                for (int i = 0; i < alphabetSize; i++)
                {
                    char ch = reader.ReadChar();
                    int code = reader.ReadInt32();

                    alphabet[ch] = code;
                }

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

                    var seqInfos = new SequenceInfoModel[seqCount];

                    for (int j = 0; j < seqCount; j++)
                    {
                        int cnt = reader.ReadInt32();
                        double freq = reader.ReadDouble();

                        seqInfos[j] = new SequenceInfoModel(cnt, freq, FitnessFromFrequency(freq));
                    }

                    sequences[seqLength] = seqInfos;
                }
            }

            // Validate

            ValidateAlphabet(alphabet);
            ValidateLetterStats(letters);
            ValidateSeqStats(sequences);

            return new LanguageInfoModel(bitsPerChar, alphabet, letters, sequences);
        }

        public string Process(Dictionary<char, char> inputKey, string data, bool forward, bool useUnrecognizedCharsDirectly, Func<bool> checkCancellation = null)
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
                    if (Char.IsWhiteSpace(dataChar) || useUnrecognizedCharsDirectly)
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

            int charsToCompare = 4;

            // Reversing key to obtain decoding key
            var decodingKey = new Dictionary<char, char>();
            foreach (var kvp in initialKey)
            {
                // Sanity check
                if (!languageInfo.Alphabet.ContainsKey(kvp.Key))
                    throw new ArgumentException("Initial key contains a plaintext letter, which is not in provided language's alphabet!");

                decodingKey[kvp.Value] = kvp.Key;
            }

            var bestFitness = EvalFitness(cipher, decodingKey, languageInfo, charsToCompare);
            var bestKey = new Dictionary<char, char>(decodingKey);

            var iterationsWithoutProgress = 0;

            for (int i = 0; i < MaxIterations; i++)
            {
                reportProgress?.Invoke(i * 100 / MaxIterations);

                var key = new Dictionary<char, char>();
                var cipherChars = decodingKey.Keys.ToList();
                var plaintextChars = decodingKey.Values.ToList();

                for (int j = plaintextChars.Count - 1; j >= 0; j--)
                {
                    // Replace j-th entry with random one
                    int rndIndex = Random.Next(j + 1);

                    var tmp = plaintextChars[rndIndex];
                    plaintextChars[rndIndex] = plaintextChars[j];
                    plaintextChars[j] = tmp;

                    key[cipherChars[j]] = plaintextChars[j];
                }

                int index1 = 0;
                int index2 = 1;

                var fitness = EvalFitness(cipher, key, languageInfo, charsToCompare);

                while (true)
                {
                    if (checkCancellation?.Invoke() ?? false)
                        return null;

                    // Randomize two indices to exchange
                    char cipher1 = key[cipherChars[index1]];
                    char cipher2 = key[cipherChars[index2]];

                    // Exchange key entries
                    key[cipherChars[index1]] = cipher2;
                    key[cipherChars[index2]] = cipher1;

                    // Evaluate new fitness
                    var newFitness = EvalFitness(cipher, key, languageInfo, charsToCompare);

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
                        key[cipherChars[index1]] = cipher1;
                        key[cipherChars[index2]] = cipher2;

                        // Try next replacement
                        index2++;
                        if (index2 >= cipherChars.Count)
                        {
                            index1++;
                            index2 = index1 + 1;
                        }

                        if (index2 >= cipherChars.Count)
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

            // Revert the key to get the ciphering key again
            var result = new Dictionary<char, char>();
            foreach (var kvp in bestKey)
                result[kvp.Value] = kvp.Key;

            // Return the best key found
            return result;
        }

        public string ExtractAlphabet(string[] lines)
        {
            HashSet<char> letters = new HashSet<char>();
            foreach (var line in lines)
                foreach (var ch in line.Where(c => char.IsLetter(c)).Select(c => char.ToLowerInvariant(c)))
                    letters.Add(ch);

            return String.Join(String.Empty, letters.OrderBy(c => c));
        }
    }
}
