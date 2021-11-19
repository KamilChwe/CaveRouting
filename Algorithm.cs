using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Reflection;

// Made by Kamil Chwedura
// Matric Number 40457841

namespace CaveRouting
{
    public class Algorithm
    {
        static void Main(string[] args)
        {
            #region Get Filename from CLI
            // Detect and open the .Cav file

            // IF IN SOURCE CODE THE DIRECTORY IS CaveRouting\bin\Debug\net5.0
            // IF IN RELEASE ITS THE SAME FOLDER AS THE EXECUTABLE

            // Gets the file name, I get this from the CLI input
            string cavFilePath = args[0] + ".cav";
            #endregion

            #region Open file and turn it to List
            //Open the .cav File
            string cavFileContents = File.ReadAllText(cavFilePath);
            //Turn file into int list
            List<int> rawCavFile = cavFileContents.Split(',').Select(int.Parse).ToList();
            #endregion

            #region Generating Cav Map and Path Relation Map
            //Get the list of caverns
            List<Cavern> cavList = new List<Cavern>();
            //List all the relations (1 = is connected, 0 = is NOT connected)
            List<int> pathRelations = new List<int>();
            #endregion

            #region Open and Closed Caverns Lists
            //Setting up the Open Cavers List (Unvisited Caverns) and Closed Caverns List (Visited Caverns)
            List<Cavern> openCaverns = new List<Cavern>();
            List<Cavern> closedCaverns = new List<Cavern>();
            #endregion

            #region Dictionaries used for calculating most efficient path
            //Key-Value for each cavern that can be quickly reached
            Dictionary<Cavern, Cavern> cameFrom = new Dictionary<Cavern, Cavern>();
            //Cost of the start cavern to any given cavern
            Dictionary<Cavern, double> gScore = new Dictionary<Cavern, double>();
            //Cost of the start cavern to the last cavern passing through each given cavern
            Dictionary<Cavern, double> fScore = new Dictionary<Cavern, double>();
            #endregion

            #region Checks for the Algorithm
            bool isSuccessful = false;
            Cavern currentCavern = null;
            #endregion

            // Get number of caverns
            int cavAmount = rawCavFile[0];

            // Add all found caverns into list of caverns (SKIPPING ONE SINCE THE FIRST ENTRY IS THE AMOUNT OF CAVERNS)
            // Multiplying by two since each coordinate is made up from 2 pairs, for example if Cave amount = 30 then there is 30 X values and 30 Y values thus making 60
            for (int i = 1, j = 1; i < cavAmount * 2; i = i + 2, j++)
            {
                Cavern theCavern = new Cavern(rawCavFile[i], rawCavFile[i + 1], j);
                cavList.Add(theCavern);
            }

            // Add first cavern to the cavern list with its g score
            gScore.Add(cavList[0], 0);
            // Add first cavern and its distance between firs cavern and last cavern
            // Take away one from last cavern since first entry is the number of caverns
            fScore.Add(cavList[0], EuclideanDistanceCalc(cavList[0], cavList[cavList.Count() - 1]));

            //Make the goal cavern be the last cavern in the list
            //Taking away 1 since the first entry is the number of caverns
            Cavern goalCavern = cavList[cavList.Count - 1];
            //Add the frist cavern in the closedCavern list
            openCaverns.Add(cavList[0]);

            // Loop through each connection in the Matrix (not the movie one). Add it to Path Relations
            for (int i = cavAmount * 2 + 1; i < rawCavFile.Count(); i++)
            {
                int connected = rawCavFile[i];
                pathRelations.Add(connected);
            }

            //Go through each cavern and check which cavern is connected and which one is close, add them to the list
            for (int i = 0; i < cavAmount; i++)
            {
                for (int j = 0; j < cavList.Count(); j++)
                {
                    Cavern testCavern = cavList[i];

                    if(pathRelations[j + (cavList.Count * i)] == 1)
                    {
                        cavList[j].closestCavern().Add(testCavern);
                    }
                }
            }

            // While the amount of Open Caverns is greater than 0
            while(openCaverns.Count() > 0)
            {
                // Set the lowest Value to the maximum value of the double type
                double lowestVal = double.MaxValue;

                //Go through each cavern in the open caverns list
                foreach(Cavern cav in openCaverns)
                {
                    //Go through each entry in the fScore list
                    foreach(KeyValuePair<Cavern, double> entry in fScore)
                    {
                        Cavern key = entry.Key;
                        double value = entry.Value;

                        // If the value in the Open Caverns is in the fScore list then compare it and set it to lowest val
                        if(cav == key)
                        {
                            if(lowestVal > value)
                            {
                                lowestVal = value;
                                currentCavern = key;
                            }
                        }
                    }
                }

                // If the current Cavern is the cavern we're trying to reach, exit this loop
                if (currentCavern.Equals(goalCavern))
                {
                    isSuccessful = true;
                    break;
                }

                // Remove currentCavern from the list of OpenCaverns
                openCaverns.Remove(currentCavern);
                // Add the currentCavern to the list of ClosedCaverns
                closedCaverns.Add(currentCavern);

                // Go through each closeCavern 
                foreach(Cavern closeCavern in currentCavern.closestCavern())
                {
                    //If the closeCavern has been already closed, skip.
                    if(closedCaverns.Contains(closeCavern))
                    {
                        continue;
                    }

                    // Distance to the closeCavern
                    double temporaryGScore = gScore[currentCavern] + EuclideanDistanceCalc(currentCavern, closeCavern);

                   //if the opencaverns list does NOT contain closecavern
                   if(!openCaverns.Contains(closeCavern))
                    {
                        openCaverns.Add(closeCavern);
                    }
                   //Otherwise if the temporary g score is equal or greater than the gscore of the closecavern, skip.
                   else if (temporaryGScore >= gScore[closeCavern])
                    {
                        continue;
                    }

                    // add closecavern and currentcavern to the cameFrom key and value list
                    cameFrom.Add(closeCavern, currentCavern);
                    //add close cavern and the temporary gscore to the list
                    gScore.Add(closeCavern, temporaryGScore);
                    // add closecavern and gscore of the closecavern to the distance between closecavern and goal cavern
                    fScore.Add(closeCavern, gScore[closeCavern] + EuclideanDistanceCalc(closeCavern, goalCavern));
                }
            }

            // If the path to the goal cavern has been FINALLY found
            if(isSuccessful)
            {
                //Write the report to the CLI.
                Console.WriteLine("Found the Path!");
                Console.WriteLine("Caverns Travelled: ");

                List<Cavern> finalPath = Backtrack(cameFrom, currentCavern);

                string output = "";

                //Calc the caverns travelled
                for (int i = 0; i < finalPath.Count(); i++)
                {
                    Cavern current = finalPath[i];
                    output = output + current.Id + " ";
                }

                Console.WriteLine(output.Trim());
                //CREATE THE CSN FILE
                File.WriteAllText(args[0] + ".csn", output.Trim());
            }
            // If no path has been found, write that no path has been found
            else
            {
                Console.WriteLine("No Path Detected.");
                File.WriteAllText(args[0] + ".csn", "0");
            }
        }

        // Calculates the Euclidean Distance between the First Cavern and the Last cavern
        static double EuclideanDistanceCalc(Cavern fromCavern, Cavern toCavern)
        {
            double x = (toCavern.PosX - fromCavern.PosX);
            double y = (toCavern.PosY - fromCavern.PosY);
            double distance = Math.Sqrt(x * x * y * y);
            return distance;
        }

        //Calculates the total Path, backtracks through each node after finding the shortest path and calculates the distance between them and adds them to the Total Path/Distance variable
        static List<Cavern> Backtrack(Dictionary<Cavern, Cavern> cameFrom, Cavern currentCavern)
        {
            List<Cavern> totalPath = new List<Cavern>();
            totalPath.Add(currentCavern);

            while(cameFrom.ContainsKey(currentCavern))
            {
                currentCavern = cameFrom[currentCavern];
                totalPath.Add(currentCavern);
            }

            totalPath.Reverse();
            return (totalPath);
        }
    }
}
