using System.IO;

namespace ShawzinBot.Models
{
    public class MidiFileModel
    {
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public string DisplayName { get; set; }
        public bool IsCompatible { get; set; }
        public string CompatibilityNotes { get; set; }
        public int TrackCount { get; set; }
        public double Duration { get; set; }

        public MidiFileModel(string filePath)
        {
            FilePath = filePath;
            FileName = Path.GetFileName(filePath);
            DisplayName = Path.GetFileNameWithoutExtension(filePath);
            IsCompatible = true;
            CompatibilityNotes = "Ready to play";
            TrackCount = 0;
            Duration = 0;
        }

        public void UpdateCompatibilityInfo(int trackCount, double duration, bool isCompatible, string notes)
        {
            TrackCount = trackCount;
            Duration = duration;
            IsCompatible = isCompatible;
            CompatibilityNotes = notes;
        }
    }
} 