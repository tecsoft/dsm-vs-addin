
Dependency Structure Matrix Add In - Version 2.0 Beta
=======================================================

1. Notes
---------

The DSM is now available as a Visual Studio Add-In as well as for Reflector.

Now it is possible to create a dependency matrix for the projects and references of a Visual Studio solution.

1. Installation
----------------

1.1 Requirements

* For the VS Add-in, VS2008,VS2010 or VS2012
* For Reflector version 6.5, 7 and 8 are currently supported

1.2 Install Steps

1.2.1 Install files

* Execute the msi to install the files to a destination directory on your machine

1.2.1 Visual Studio add-in

* Open Visual Studio, go to Tools->Options
* Select Environment->Add-In/Macros Security
* Add the destination directory to the list of Add-in file paths
* Restart Visual Studio, the Add-In is henceforth available in the Tools menu

1.2.2

* Open Reflector
* Select Tools->Add-ins
* Remove any previous version of the DSM add-in (select add-in and click Remove)
* Click Add and select from the installation directory the dll that corresponds to your version of Reflector
  Tcdev.DsmReflector7.dll for version 7 for example

2. Getting Started
------------------- 

2.1 General

* The add-in works much the same way for Visual Studio and Reflector
  The main diffrence is that the VS version will scan the solution to automatically find project
  assembles and references, whereas the Reflector version takes assembles currently loaded in Reflector.

2.1.1 Visual Studio

* Open a solution, make sure it is compiled
* Open the DSM Add-in (Tools->Dependency Structure Matrix)
* Choose No when the add-in asks if you want to run the analysis now
* In the list of assembles, select those you wish to analyses (by default all project assembles
  are selected, references assemblies are listed but not selected by default)
* Click Run Analysis, after a short moment the DSM is displayed
* Expand the nodes in the left handle panel of the matrix to explore the module hierarchy
* Right-click on a dependency in the matrix to show relations involved in that dependency
* Arrange the order the nodes in the left hand panel to reflect the architecture of the application
* To save the current DSM, click on the Save Button, 
  save the file in the same directory as the current .sln file
* Next time you open the VS add-in, the DSM tool will detect automatically the previously saved file
  and reload the DSM.  To update the DSM with the latest compiled assembles, 
  click on Run Analysis in the Start Page tab

2.1.2 Reflector

* Using Reflector open the .NET assembly you wish to analyse.
* In the Tools menu, select "Dependency Structure Matrix"
* On the Start Page that opens, select the assembly to analyse and click Run Analysis
* The analysis runs and the matrix is displayed automatically
* Explore the DSM as above

3. Issues
---------

3.1 Notes

* The DSM Add-in v2.0 is currently in Beta.  There may be a number of minor issues to resolve
* If you find any please email me them to tom.e.carter@gmail

3.1.2 Known Issues

* Loading a previously saved .dsm, the selected items in the assembly list doesn't reflect accurately
  the assembles previously selected for analysis
* When there is no single root element we cannot partition the top level modules

4. Further Information
-----------------------

Please see the website http://www.tom-carter.net for further details on using the DSM Add-in




/-- End --/
