# Wim-Tech-Assessment
Repository for Technical Assessment from Wim Technologies

# Frequency Distribution Problem
This program assigns a set of frequencies across a list of Cellular Sites while minimizing the interference caused by sites using the same frequency in close proximity to each other.
This is done by optimizing the distribution such that sites that have similar frequencies are spaced as far apart as possible.

## Inputs
### Frequencies
The frequencies to be assigned are defined in an Array in Program.cs.
```C#
int[] Frequencies = { 110, 111, 112, 113, 114, 115 };
```
### Cellular Sites
The Sites are imported from a file, the path of which is also defined in Program.cs, See "Example Input.csv".

| Cell ID | Easting | Northing | Longitude | Latitude |
|------|-----|---|---|---|
| A |	536660 |	183800 |	-0.03098 |	51.53657 |
| B	| 537032 |	184006 |	-0.02554 |	51.53833 |
| ... | ... | ... | ... | ... |

## Output
In the command line, the program returns the following table
| Cell ID | Frequency | Total Interference |
|------|-----|---|
| A |	113 |	0.00255.. |
| B	| 112 |	0.00206.. |
| ... | ... | ... |

It also writes the following table to an output file, name and path of which is defined in Program.cs.
| Cell ID | Frequency | Total Interference | Easting | Northing | Longitude | Latitude |
|------|-----|---|-----|---|---|---|
| A |	113 |	0.00255.. |	536660 |	183800 |	-0.03098 |	51.53657 |
| B	| 112 |	0.00206.. | 537032 |	184006 |	-0.02554 |	51.53833 |
| ... | ... | ... | ... | ... | ... | ... |
