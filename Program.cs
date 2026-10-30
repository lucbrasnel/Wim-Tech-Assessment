using System.Linq;
using System.IO;
using System.Collections.Generic;
using CellSites;
using MaxHeap;
using CsvHelper;
using System.Transactions;

public class Solution
{
    public static void Main()
    {
        int[] Frequencies = { 110, 111, 112, 113, 114, 115 };
        string projDir = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName + "\\"; // Gets project directory from \bin\Debug
        string inputFile = projDir + "Example Input.csv";
        string outputFile = projDir + "Example Output.csv";
        bool verbose = true; // set to false to not print progress

        // -------------- Create List of cell sites & Assign random frequencies -------------------

        // List for the data from the file
        List<CellSite> Sites = new List<CellSite>();
        
        // To keep track of what sites are assigned which frequencies
        FreqList FreqSitesList = new FreqList(Frequencies);

        // Read File (using CsvHelper https://joshclose.github.io/CsvHelper/)
        using var reader = CsvReader.FromFile(inputFile);

        var CellID = reader.GetFieldIndex("Cell ID");
        var Easting = reader.GetFieldIndex("Easting");
        var Northing = reader.GetFieldIndex("Northing");
        var Longitude = reader.GetFieldIndex("Longitude");
        var Latitude = reader.GetFieldIndex("Latitude");

        while (reader.Read())
        {
            if (verbose){ Console.WriteLine("------- Reading File -------"); }
            // skip header ??

            // get site info
            string id = reader.GetString(CellID);
            int east = reader.Parse<int>(Easting);
            int north = reader.Parse<int>(Northing);

            double lon = reader.Parse<double>(Longitude);
            double lat = reader.Parse<double>(Latitude);

            // Add each site to list
            CellSite newSite = new CellSite(id, lat, lon, north, east);
            Sites.Add(newSite);

            // Assign random freq
            Random rng = new Random();
            int randFreq = rng.Next(Frequencies.Length); // get index of random frequency

            FreqSitesList.AddSite(newSite, Frequencies[randFreq]);

            if (verbose){ Console.WriteLine($"{id} set to {Frequencies[randFreq]}"); }
        }

        // ---------------------- Calculate interferences & Create max heap -----------------------

        if (verbose) { Console.WriteLine("\n------- Create Heap -------"); }

        // Max Heap
        InterferenceMaxHeap interferenceHeap = new InterferenceMaxHeap(Sites.Count);

        // Calculate interferences & add to heap
        foreach(CellSite s in Sites)
        {
            int freq = s.Frequency; // Get frequency assigned to site
            List<CellSite> sitesWithFreq = FreqSitesList.GetFreqSites(freq); // get list of all sites with same frequency

            double interference = CalcTotalInterference(s, sitesWithFreq); // calculate total frequency interference of that site

            interferenceHeap.AddSite(s, interference);

            if (verbose) { Console.WriteLine($"{s.CellID} has interference of {interference}"); }
        }

        // Sort heap
        interferenceHeap.BuildHeap();

        if (verbose) { Console.WriteLine("\n" + interferenceHeap.ToString()); }


        // ------------------------------ Optimize Frequencies -----------------------------------

        if (verbose) { Console.WriteLine("\n------- Optimize Frequencies -------"); }

        //
        int count = 1; // How many sites checked / index of site to check (starts at 1)
        int max = Sites.Count;

        while (count <= max) // When checked all sites, stop
        {
            if (verbose) { Console.WriteLine($"Index: {count}"); }

            // Get node with largest interference
            KeyValuePair<CellSite, double> current = interferenceHeap.GetAtIndex(count);

            int currentFreq = current.Key.Frequency;

            double smallestInterference = current.Value;
            int smallestFreq = currentFreq;

            // Calculate possible interferences for site
            foreach (int f in Frequencies)
            {
                if (f != currentFreq)
                {
                    List<CellSite> sitesWithFreq = FreqSitesList.GetFreqSites(f);
                    double newInterference = CalcTotalInterference(current.Key, sitesWithFreq);

                    if (newInterference < smallestInterference)
                    {
                        smallestInterference = newInterference;
                        smallestFreq = f;
                    }
                }
            }

            if (verbose) {
                Console.WriteLine($"{current.Key.CellID}: Current Freq {currentFreq}, Smallest Freq {smallestFreq}");
                Console.WriteLine($"{current.Key.CellID}: Current Intf {current.Value}, Smallest Intf {smallestInterference}");
            }

            // Assign new frequency if optimizes interference for site
            if (smallestFreq != currentFreq)
            {
                FreqSitesList.ChangeFrequency(current.Key, smallestFreq); // update frequency for site

                // Update interference for that frequency
                interferenceHeap.UpdateInterference(current.Key, smallestInterference); // update interference and resort heap

                List<CellSite> sitesWithFreq = FreqSitesList.GetFreqSites(smallestFreq); // get list of all sites with that freq

                foreach (CellSite s in sitesWithFreq) // update interference of other sites in that frequency
                {
                    if (s.CellID != current.Key.CellID)
                    {
                        double newInterference = CalcTotalInterference(s, sitesWithFreq);
                        interferenceHeap.UpdateInterference(s, newInterference);
                    }
                }

                // Reset counter to top of heap
                count = 1;

                if (verbose) { Console.WriteLine($"CHANGED FREQUENCY!!!"); }
            }
            else
            {
                // Already have optimal freq (for now), move to next
                count++;
            }
        }

        // -------------------------------- Optimal result? ---------------------------------------

        // Went through whole list of sites and did not change the frequency of anyone

        if (verbose) { Console.WriteLine("\n" + FreqSitesList.ToString() + "\n"); }

        // Calculate statistics
        double avgInterference = 0;
        double[] avgInterferencePerFreq = new double[Frequencies.Length];
        int[] totalSitesPerFreq = new int[Frequencies.Length];
        int avgSitePerFreq = 0;


        // Create output table
        // CellID | Frequency | Interference
        string output = "Cell ID - Frequency - Total Interference";

        // Write to File
        using var writer = CsvWriter.ToFile(outputFile);
        writer.WriteHeader("Cell ID", "Frequency", "Total Interference", "Easting", "Northing", "Longitude", "Latitude");

        foreach (CellSite s in Sites)
        {
            double intf = interferenceHeap.GetSiteInterference(s);

            // Build Command line output
            output += $"\n{s.CellID} - {s.Frequency} - {intf}";

            // write to file
            writer.WriteField(s.CellID);
            writer.WriteField(s.Frequency);
            writer.WriteField(intf);
            writer.WriteField(s.GetNorthEasting()[1]); // Easting
            writer.WriteField(s.GetNorthEasting()[0]); // Northing
            writer.WriteField(s.GetLatLong()[1]); // Longitude
            writer.WriteField(s.GetLatLong()[0]); // Latitude

            writer.EndRecord();
        }

        if (verbose) { Console.WriteLine($"Wrote to file - {outputFile}\n"); }

            // Write command line output
            Console.WriteLine(output);

        // ---------------------------------- AI Extension ----------------------------------------

    }

    private static double CalcTotalInterference(CellSite site, List<CellSite> similarSites)
    {
        // Interference for a site is calculated based on it's distance from sites with the same frequency
        // Interference from site a on site b = 1/(distance from a to b)
        // This method returns the total inteference from all sites (excluding this site) with the same frequency

        double totalInterference = 0;

        foreach (CellSite s in similarSites)
        {
            if(s.CellID != site.CellID)
            {
                double d = site.GetProjectedDistance(s);

                totalInterference += 1 / d;
            }
        }

        return totalInterference;
    }
}