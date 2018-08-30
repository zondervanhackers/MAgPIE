# Patron Tables

|Image|Explanation|
|-----|-----------|
|![Statistics.Patron.png](Statistics.Patron.png)||
|![Patrons](Patrons.png)|Has a Gender and Barcode for each individual Patron which is marked by an individual patronId. Has one to many relationship to PatronRecords.|
|![PatronRecords](PatronRecords.png)|Stores multiple time specific PatronRecordId's for each Patron. It also includes an effectiveStartDate and effectiveEndDate of that time period.|
|![StudentMajorRecords](StudentMajorRecords.png)|Stores data about a students major at a specific time.|
|![StudentRecords](StudentRecords.png)|Stores Data about a student at a specific time.|
|![StudentResidenceRecords](StudentResidenceRecords.png)|Stores records of a student at a specific time at a specific residence.|
|![StudentResidences](StudentResidences.png)|Stores Residence Names and their ResidenceCategory(Commuter, OnCampus, OffCampus, AwayFromCampus)|