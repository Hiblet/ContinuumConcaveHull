# ContinuumConcaveHull

Alteryx C# plug-in to calculate the Concave Hull around a set of points.

Provided free, courtesy of Continuum (www.continuum.je)



## Introduction

This project is a Windows Visual Studio 2017 project for .Net Framework 4.6.1.  It is a Class Library DLL for use with Alteryx, compiled to x64.

The app implements a Alteryx plug-in tool that works out the convex hull, and then reduces that inwards to make a concave hull, around a set of points.

The user can specify a degree of concavity, as there are often multiple possible concave hulls for an arbitrary set of points.  For example, if 180 degrees is used, this represents no concavity, and if zero degrees is used, this is maximal concavity.  If a value like 90 degrees is used, it means that the resultant hull will have no internal angle less than a right angle of 90 degrees.  The user may have to 'play' with this value to achieve the expected or desired hull shape.

The tool allows the user to specify multiple sets of points to be handled separately.


## Installation

The DLL must be copied to "C:\Program Files\Alteryx\bin\Plugins\ContinuumConcaveHull" on your local installation.  

NOTE! Windows will block DLLs from the Internet, so once the DLL is on your PC, right click it, select "Show More Options", select "Properties", and on the General tab at the bottom you may see "This file came from another computer and might be blocked to help protect this computer.  Here there is a checkbox without a checkmark (tick) in it called Unblock.  Check the checkbox to unblock the DLL, otherwise Windows will refuse to let it work.

Next, you need an INI file in "C:\Program Files\Alteryx\Settings\AdditionalPlugIns".  The INI file is included in this repo, so you can copy it to your local folder.  The INI file has the path info that Alteryx needs to find the DLL, and the ToolGroup that the tool should appear in, in this case "Developer".

If you are not worried about putting the tool in a particular group, you can just copy the DLL to "C:\Program Files\Alteryx\bin\Plugins".  Once the DLL is there, it will become available in the Alteryx Designer Search tool, as "ConcaveHull".

The tool will have a grey-cog-wheel icon with a five pointed star in the centre.


## Usage

An optional Group field is available.  If no field is selected, all points are considered in a single group, named DEFAULT.  If a group field is selected, points are analysed within groups, and this allows the simultaneous processing of multiple point sets.

The tool needs to know which columns hold the X and Y coordinates.  Coordinate values expected to be double values, allowing for both latitude and longitude, and theoretical cartesian coordinates.

Finally, the tool requires a concavity angle limit.  180 degrees implies no concavity at all, and a convex hull is the result.  0 degrees implies maximum concavity, and a seriously spikey shape could result.  90 degrees implies that the internal angles of the hull will be no sharper than 90 degrees.  For an arbitrary set of points, there are potentially multiple resulting concave hulls, so the user can adjust this angle and find a value that yields a practical shape for their purpose.



## Example




## Author

Steve Hibbert (steve at Continuum dot je), June 2020 (www.continuum.je)



## License

MIT License: Use as you see fit, give credit where applicable.


## Updates

### V 1.0.0
Alpha Release.



## Credits and Acknowledgements

This tool was based on Liagson's treatment, but extended considerably.  

The tool uses a Graham Scan to establish the starting convex hull.  (https://en.wikipedia.org/wiki/Graham_scan)
