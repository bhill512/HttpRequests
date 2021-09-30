using Serilog;
using System;
using System.Collections.Generic;
using System.Text;

namespace HttpRequests
{
    public class DisplayInfo
    {
        ILogger _logger;

        public DisplayInfo(ILogger logger)
        {
            _logger = logger;
        }

        public void DisplayImdbInfo(ShowLookupTvMazeApi tvMaze)
        {
            if (string.IsNullOrWhiteSpace(tvMaze.Name))
            {
                Console.WriteLine("\nSorry. Nothing could be found with that ID\n");
            }
            else
            {
                Console.WriteLine($"\nYou found {tvMaze.Name}\n" +
                $"Here's some other info:\n" +
                $"Rating: {tvMaze.Rating.Average}\n" +
                $"Premiered: {tvMaze.Premiered}\n" +
                $"Ended: {tvMaze.Ended}\n" +
                $"Status: {tvMaze.Status}\n" +
                $"Runtime: {tvMaze.AverageRuntime} minutes \n" +
                $"Summary: {tvMaze.Summary}\n" +
                $"Network: {tvMaze.Network.Name}\n" +
                $"Show Url: {tvMaze.Url}\n");
            }
        }
    }
}
