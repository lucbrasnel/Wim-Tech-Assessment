using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CellSites
{
    internal class CellSite
    {
        public string CellID { get; }
        public int Frequency { get; set; }

        private double latitude;
        private double longitude;

        private int northing;
        private int easting;

        public CellSite(string cellID, double latitude, double longitude, int northing, int easting)
        {
            this.CellID = cellID;
            this.Frequency = -1;

            this.latitude = latitude;
            this.longitude = longitude;
            
            this.northing = northing;
            this.easting = easting;
        }

        // ------------------- GET / SET --------------------------

        public double[] GetLatLong()
        {
            double[] coordinates = new double[2];

            coordinates[0] = this.latitude;
            coordinates[1] = this.longitude;

            return coordinates;
        }

        public int[] GetNorthEasting()
        {
            int[] coordinates = new int[2];

            coordinates[0] = this.northing;
            coordinates[1] = this.easting;

            return coordinates;
        }

        // ------------------- UTILITY --------------------------

        public double GetGeoDistance(CellSite b)
        {
            // Might add ability to calculate geodisic distance with long/lat
            return -1;
        }

        public double GetProjectedDistance(CellSite b)
        {
            // Easting/Northing uses points on a projected 2D plane, so distance is an easy slope calculation
            // This however means that the distance will not be 100% accurate to the true distance, because of the projection
            // Myers, P.  2026.  Grid Reference Distance Calculator.  https://calculator.academy/grid-reference-distance-calculator/

            int[] a_cords = this.GetNorthEasting();
            int[] b_cords = b.GetNorthEasting();

            double distance = Math.Sqrt(Math.Pow( (a_cords[0] - b_cords[0]), 2) + Math.Pow((a_cords[1] - b_cords[1]), 2));

            return distance;
        }
    }

    // ===================================================== FREQUENCY SITE LIST =========================================================================
    // Wrapper class to maintain lists of which sites were assigned a frequency

    internal class FreqList
    {
        private Dictionary<int, List<CellSite>> SiteFrequencies;

        public FreqList(int[] frequencies)
        {
            this.SiteFrequencies = new Dictionary<int, List<CellSite>>();

            // Initialize Frequency lists
            foreach (int f in frequencies)
            {
                SiteFrequencies.Add(f, new List<CellSite>());
            }
        }

        // ------------------- METHODS --------------------------

        public void AddSite(CellSite site, int frequency)
        {
            // Assign a frequency to a site
            
            // Add to list of nodes with that freq
            this.SiteFrequencies[frequency].Add(site);

            // Update CellSite Frequency
            site.Frequency = frequency;
        }

        public void ChangeFrequency(CellSite site, int newFreq)
        {
            // Assign a new Frequency to a site

            // Remove from old freq
            int oldFreq = site.Frequency;
            this.SiteFrequencies[oldFreq].Remove(site);

            // Add to new freq
            site.Frequency = newFreq;
            this.SiteFrequencies[newFreq].Add(site);
        }

        public List<CellSite> GetFreqSites(int frequency)
        {
            // Get list of sites that were assigned a frequency

            if (this.SiteFrequencies.ContainsKey(frequency))
            {
                List<CellSite> freqSites = this.SiteFrequencies[frequency];
                return freqSites;
            }
            else
            {
                return null;
            }
        }

        // ---------------------------- TO STRING ----------------------------------------

        public override string ToString()
        {
            string output = "Frequency - Sites";

            foreach (var freq in SiteFrequencies)
            {
                output += $"\n{freq.Key} - ";

                foreach (CellSite site in freq.Value)
                {
                    output += site.CellID + ", ";
                }
            }

            return output;
        }
    }
}
