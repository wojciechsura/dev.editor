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

        // Public methods -----------------------------------------------------

        public LanguageStatisticsModel InitializeLanguageStatisticsModel()
        {
            var result = new LanguageStatisticsModel();
            for (int i = MinChars; i <= MaxChars; i++)
                result.Sequences[i] = new Dictionary<string, int>();

            return result;
        }

        public void AddLineToLanguageStatisticsModel(LanguageStatisticsModel model, string line)
        {
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
        
        public LanguageInfoModel BuildLanguageInfoModel(LanguageStatisticsModel model)
        {
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

                    sequences[seqCount] = seqInfo;
                }
            }

            // Validate

            ValidateLetterStats(letters);
            ValidateSeqStats(sequences);

            return new LanguageInfoModel(letters, sequences);
        }
    }
}
