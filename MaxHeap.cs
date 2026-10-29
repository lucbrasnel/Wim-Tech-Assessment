using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CellSites;

namespace MaxHeap
{
    internal class InterferenceMaxHeap
    {
        // Max Heap - Organizes Sites by interference, largest on top
        // Based on Prateek Garg's Tutorial on Hacker Earth https://www.hackerearth.com/practice/data-structures/trees/heapspriority-queues/tutorial/

        private KeyValuePair<CellSite, double>[] heap;
        private int count;
        private int heapSize;

        public InterferenceMaxHeap(int size)
        {
            heap = new KeyValuePair<CellSite, double>[size+1];
            count = 1; // start populating heap at 1, leave 0 empty
            heapSize = size + 1;
        }

        // ----------------------------- GET / SET ------------------------------------------

        // Add site to heap array
        public void AddSite(CellSite site, double interference)
        {
            if (count < heap.Length)
            {
                heap[count] = new KeyValuePair<CellSite, double>(site, interference);
                count++;
            }
        }

        // Get index of a site in the heap
        public int GetSiteIndex(CellSite site)
        {
            for (int i = 1; i < heap.Length; i++)
            {
                if (heap[i].Key.CellID == site.CellID)
                {
                    return i;
                }
            }

            return -1;
        }

        // Get the interference of a site
        public double GetSiteInterference(CellSite site)
        {
            int idx = GetSiteIndex(site);
            return heap[idx].Value;
        }

        // Get site and interference at index
        public KeyValuePair<CellSite, double> GetAtIndex(int idx)
        {
            return heap[idx];
        }

        // ---------------------------- HEAP methods ----------------------------------------

        // Heapify
        public void MaxHeapify(int idx)
        {
            int leftChild = 2 * idx;
            int rightChild = 2 * idx + 1;
            int Largest = 0;

            // if left child's interference is >, swap
            if (leftChild < heapSize && heap[leftChild].Value > heap[idx].Value)
            {
                Largest = leftChild;
            }
            else
            {
                Largest = idx;
            }

            // if right child's interference is >, swap
            if (rightChild < heapSize && heap[rightChild].Value > heap[idx].Value)
            {
                Largest = rightChild;
            }

            if (Largest != idx) // swap with largest child
            {
                KeyValuePair<CellSite, double> temp = heap[idx];
                heap[idx] = heap[Largest];
                heap[Largest] = temp;

                // Also check swapped child recursively
                MaxHeapify(Largest);
            }
        }

        // Change Interference value
        public void UpdateInterference(CellSite site, double newInterference)
        {
            // Update site's interference
            int idx = GetSiteIndex(site);

            if (idx != -1) // if site in heap (All sites should have been added before changing values)
            {
                KeyValuePair<CellSite, double> newVal = new KeyValuePair<CellSite, double>(site, newInterference);
                heap[idx] = newVal;

                // Move Updated value to top
                KeyValuePair<CellSite, double> temp = heap[1];
                heap[1] = heap[idx];
                heap[idx] = temp;

                // Resort new value in heap
                MaxHeapify(1);
            }
        }

        // Initial heap sort. Optimized
        // Because the last half of the arr contains leaf nodes, can start at their parents
        public void BuildHeap()
        {
            for (int i = heapSize/2; i >= 1; i--)
            {
                MaxHeapify(i);
            }
        }

        // ---------------------------- TO STRING ----------------------------------------

        public override string ToString()
        {
            string output = "Idx CellID Frequency Interference";

            for (int i = 1; i < heapSize; i++)
            {
                output += $"\n {i} {heap[i].Key.CellID} {heap[i].Key.Frequency} {heap[i].Value} -";
            }

            return output;
        }
    }
}
