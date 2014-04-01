Randomer
========

Generate random data for hosted feature services on ArcGIS Online.

* DISCLAIMER - use at your own risk!

The workflow here is

 - Log in to ArcGIS Online using your developer or organizational account
 - A list of your hosted feature services is shown
 - Select one and it will query the first layer in the service
 - Enter the number of features to create (between 1 and 1000)
 - Only attribute fields that are required and are editable will be auto populated (with random data)
 
 Note that only point feature classes ar ecurrently supported.
 
 This is a very basic example whose primary use is to quickly generate dummy data for testing purposes. Feel free to contribute if you are feeling that way inclined.
