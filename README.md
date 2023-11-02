# Rotel-Suite
This is a .NET library that can be used to control Rotel devices that support either IP or RS232 communication.

Repository consists of 2 projects:
 - communication library
 - example console app that makes use of the communication library

After first launch the library will attempt to generate a config file for Rotel A14 MKII that contains command mapping, it can be extended with mapping for any other Rotel device.
All communication config variables are stored in static class `Communication`, so make sure to update them before compilation. Of course external JSON config would be useful, but that's something to work on - feel free to contribute.
RS232 communication can be useful for european users that want to turn on their Rotel devices: due to EU energy regulations it's not possible to turn on through IP or by signal detection.

31-10-2023: the library was tested only against IP communication, only against Rotel A14 MKII, however I should be able to test the RS232 communication in coming days. Note for RS232: if you plan on using it, make sure to get yourself a high quality cable/adapter since not all of them are guaranteed to work (I learned it the hard way).

02-11-2023: after minor fix I was able to successfully use the RS232 connection. The new USB-RS232 cable I ordered uses genuine FTDI chipset (FT232R) and handles communication like a charm.
