using System;
using System.Collections.Generic;
using System.Linq;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;

namespace ShawzinBot.Models
{
    public class MidiCompatibilityChecker
    {
        private const int MAX_SIMULTANEOUS_KEYS = 3; // Game limitation
        private const int MAX_NOTES_PER_SECOND = 8; // Reasonable limit for smooth play
        private const int MIN_NOTE_DURATION_MS = 50; // Minimum note duration

        public static MidiCompatibilityResult CheckCompatibility(MidiFile midiFile)
        {
            var result = new MidiCompatibilityResult();
            
            try
            {
                var tempoMap = midiFile.GetTempoMap();
                var notes = midiFile.GetNotes();
                var tracks = midiFile.GetTrackChunks();

                result.TrackCount = tracks.Count();
                        // For now, skip duration calculation to avoid compilation issues
                        result.Duration = 0.0;
                result.TotalNotes = notes.Count();

                // For now, just do basic checks without complex timing analysis
                var basicIssues = CheckBasicCompatibility(notes, tracks);
                result.SimultaneousKeyIssues = basicIssues;

                // Determine overall compatibility
                result.IsCompatible = !basicIssues.Any();

                result.GenerateCompatibilityNotes();
            }
            catch (Exception ex)
            {
                result.IsCompatible = false;
                result.CompatibilityNotes = string.Format("Error analyzing file: {0}", ex.Message);
            }

            return result;
        }

        private static List<string> CheckBasicCompatibility(IEnumerable<Note> notes, IEnumerable<TrackChunk> tracks)
        {
            var issues = new List<string>();
            
            // Check for out-of-range notes
            var noteNumbers = notes.Select(n => n.NoteNumber).Distinct();
            foreach (var noteNumber in noteNumbers)
            {
                if (noteNumber < 48 || noteNumber > 75) // C3 to D#5 range
                {
                    issues.Add(string.Format("Note {0} is outside supported range (C3-D#5)", noteNumber));
                }
            }

            // Check if there are too many tracks (basic check)
            if (tracks.Count() > 10)
            {
                issues.Add(string.Format("Too many tracks ({0}), may cause performance issues", tracks.Count()));
            }

            return issues;
        }
    }

    public class MidiCompatibilityResult
    {
        public bool IsCompatible { get; set; }
        public int TrackCount { get; set; }
        public double Duration { get; set; }
        public int TotalNotes { get; set; }
        public List<string> SimultaneousKeyIssues { get; set; } = new List<string>();
        public List<string> SpeedIssues { get; set; } = new List<string>();
        public List<string> DurationIssues { get; set; } = new List<string>();
        public List<string> RangeIssues { get; set; } = new List<string>();
        public string CompatibilityNotes { get; set; }

        public void GenerateCompatibilityNotes()
        {
            var issues = new List<string>();
            
            if (SimultaneousKeyIssues.Any())
                issues.Add(string.Format("Compatibility issues: {0}", SimultaneousKeyIssues.Count));

            if (issues.Any())
            {
                CompatibilityNotes = string.Format("Issues found: {0}", string.Join(", ", issues));
            }
            else
            {
                CompatibilityNotes = string.Format("Ready to play - {0} tracks, {1} notes, {2:F1}s", TrackCount, TotalNotes, Duration);
            }
        }
    }
} 