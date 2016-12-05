namespace aoc2016_1 {

    // System
    using System;
    using System.IO;
    using System.Linq;
    using System.Collections.Generic;

    // Solution class
    class Day1Main {

        // The cursor's absolute heading
        private enum AbsoluteHeading {
            North = 0,
            East = 1,
            South = 2,
            West = 3
        };

        // The relative heading provided by the instruction
        private enum RelativeHeading {
            Right = 1,
            Left = -1
        };

        // Representation of an instruction from the input
        private struct Instruction {
            public RelativeHeading Direction;
            public int Steps;
            // Create a new Instruction from a raw instruction string
            public Instruction(string rawInstruction) {
                // Set the relative heading change
                var rawHeading = rawInstruction.Trim().First();
                switch(rawHeading) {
                    case 'R':
                    case 'r':
                        this.Direction = RelativeHeading.Right;
                        break;
                    case 'L':
                    case 'l':
                        this.Direction = RelativeHeading.Left;
                        break;
                    default:
                        throw new ArgumentException("Unknown direction string within input instruction");
                }
                // Set the number of steps to take
                this.Steps = int.Parse(new string(rawInstruction.Trim().Skip(1).ToArray()));
            }
        };

        /// <summary>
        /// Program entrypoint
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args) {
            // Read the input and generate the instruction set
            var fileReader = new StreamReader("input.txt");
            var rawInstructions = fileReader.ReadToEnd().Split(',').ToList();
            var instructions = rawInstructions.Select(raw => new Instruction(raw));
            // Initialize the cursor's travel state
            var currentHeading = AbsoluteHeading.North;
            var currentCoordinate = new Tuple<int, int>(0, 0);
            var visitedLocations = new HashSet<Tuple<int, int>> { currentCoordinate };
            var found = false;
            var firstDuplicate = new Tuple<int, int>(0, 0);
            // Move the cursor through each instruction
            foreach(Instruction instruction in instructions) {
                // Determine new heading
                currentHeading = GenerateNextHeading(currentHeading, instruction.Direction);
                // Generate the next coordinate from the provided step count
                List<Tuple<int, int>> traveledLocations;
                currentCoordinate = GenerateNextCoordinate(
                    currentCoordinate, 
                    currentHeading, 
                    instruction.Steps, 
                    out traveledLocations);
                // If we have not yet found the first repeated location, and we have repeated a location
                // Set the 'first duplicate' indication
                if(!found && visitedLocations.Any(loc => traveledLocations.Contains(loc))) {
                    found = true;
                    firstDuplicate = traveledLocations.Find(loc => visitedLocations.Contains(loc));
                }
                // Add all traversed locations to the hash set to track visited locations
                foreach(Tuple<int,int> location in traveledLocations) {
                    visitedLocations.Add(location);
                }
            }
            // Calculate the distance
            var hqDistance = Math.Abs(currentCoordinate.Item1) + Math.Abs(currentCoordinate.Item2);
            var firstDuplicateDistance = Math.Abs(firstDuplicate.Item1) + Math.Abs(firstDuplicate.Item2);
            // Print the result
            Console.WriteLine(string.Format(
                "Final location coordinates are {0}, {1}", currentCoordinate.Item1, currentCoordinate.Item2));
            Console.WriteLine(string.Format(
                "The distance to the HQ is {0}", hqDistance));
            Console.WriteLine(string.Format(
                "The first location visited twice is {0}, {1}", firstDuplicate.Item1, firstDuplicate.Item2));
            Console.WriteLine(string.Format(
                "The distance to the first duplicated location is {0}", firstDuplicateDistance));
            Console.ReadKey();
        }

        /// <summary>
        /// Based on the relative direction change, determine the new heading
        /// </summary>
        /// <param name="current"></param>
        /// <param name="change"></param>
        /// <returns></returns>
        private static AbsoluteHeading GenerateNextHeading(AbsoluteHeading current, RelativeHeading change) {
            if(current == AbsoluteHeading.North && change == RelativeHeading.Left) {
                return AbsoluteHeading.West;
            } else if( current == AbsoluteHeading.West && change == RelativeHeading.Right) {
                return AbsoluteHeading.North;
            } else {
                return (AbsoluteHeading)((int)current + (int)change);
            }
        }

        /// <summary>
        /// Based on the steps taken and the heading, return the new position from the current position
        /// Also return a list of all locations traversed in the movement
        /// </summary>
        /// <param name="current"></param>
        /// <param name="heading"></param>
        /// <param name="steps"></param>
        /// <returns></returns>
        private static Tuple<int, int> GenerateNextCoordinate(
            Tuple<int, int> current, 
            AbsoluteHeading heading, 
            int steps, 
            out List<Tuple<int,int>> traveledLocations) {
            traveledLocations = new List<Tuple<int, int>>();
            switch(heading) {
                case AbsoluteHeading.North:
                    foreach(int step in GenerateRange(current.Item2, current.Item2 + steps)) {
                        traveledLocations.Add(new Tuple<int, int>(current.Item1, step));
                    }
                    break;
                case AbsoluteHeading.South:
                    foreach(int step in GenerateRange(current.Item2, current.Item2 - steps)) {
                        traveledLocations.Add(new Tuple<int, int>(current.Item1, step));
                    }
                    break;
                case AbsoluteHeading.East:
                    foreach(int step in GenerateRange(current.Item1, current.Item1 + steps)) {
                        traveledLocations.Add(new Tuple<int, int>(step, current.Item2));
                    }
                    break;
                case AbsoluteHeading.West:
                    foreach(int step in GenerateRange(current.Item1, current.Item1 - steps)) {
                        traveledLocations.Add(new Tuple<int, int>(step, current.Item2));
                    }
                    break;
                default:
                    throw new ApplicationException("Invalid enum value");
            }
            return traveledLocations.Last();
        }

        /// <summary>
        /// Generate a range of coordinates between points
        /// </summary>
        /// <param name="end"></param>
        /// <param name="start"></param>
        /// <returns></returns>
        private static IEnumerable<int> GenerateRange(int start, int end) {
            if(end == start) {
                return new List<int>();
            } else if(end > start) {
                return Enumerable.Range(start + 1, end - start);
            } else {
                return Enumerable.Range(end, start - end).Reverse();
            }
        }

    }

}
