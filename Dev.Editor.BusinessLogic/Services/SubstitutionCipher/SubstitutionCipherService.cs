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

        private LanguageStatisticsModel GenerateLanguageStatisticsModel(string[] lines, Func<bool> checkCancellation, Action<int> reportProgress)
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
            if (inputKey.Any(kvp => !char.IsLower(kvp.Key) || !char.IsLower(kvp.Value)))
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
                    result.Append(dataChar);
                }

                if (checkCancellation?.Invoke() ?? false)
                {
                    return null;
                }
            }

            return result.ToString();
        }
    }
}
