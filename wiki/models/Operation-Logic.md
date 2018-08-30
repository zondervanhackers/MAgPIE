
# Import WMSInventory

* Processes all new or modified files.
* Harvests each Inventory File separately.
* Dynamic Stream Parser is created at run time to accommodate for multiple file structures.
* The entire operation is marked as a failure in its log file if one file throws an unhandled exception.

# Import WMSTransactions

* Processes all new or modified files.
* Harvests all file data at once.
* User Barcodes that are longer than 16 and that match a pattern of "SomeTextHere01022016" will be shortened to SomeText01022016".

# Import Demographics

* Processes all new or modified files.
* Harvests all file data at once.

# Import CounterTransactions

* Processes a months worth of data from a vendor.
* Will harvest multiple reports for one month if multiple are specified.
* If a record has no Identifiers we generate one by combining parts from its Name (First Letter + Length + Hashcode + Last Letter)

# EbscoHostCounter

* Processes a months worth of data from EbscoHost.
* Use Sushi Service to obtain database information.
* Uses WebScraping to obtain every other type (JR1, BR1...)
* We must use WebScraping for everything except for databases because the reports would have no differentiation between databases.

# Sync

* Adds or Updates any new files from the source to the destination.
* It uses a file pattern which is a regular expression to grab only the file names that match. [Regex Method](https://msdn.microsoft.com/en-us/library/twcw2f1c(v=vs.110).aspx)