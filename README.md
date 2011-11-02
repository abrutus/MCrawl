MCrawl MoodleCrawler
====================
Service Reference
-----------------
You need to have MCrawl and HtmlAgilityPack added as a reference.
Example
-------
The following (console application) snippet will display all assignments in the terminal

    //using MCrawl;
    MoodleCrawler crawler = new MoodleCrawler("username", "password");
    var assignments=crawler.FetchAllAssignments();
    foreach (MoodleCrawler.Assignment a in assignments)
    {
    	Console.WriteLine(	"Topic: " + a.Topic +
    						"\r\nName: " + a.Name +
    						"\r\nType: " + a.Type +
    						"\r\nDue: " + a.Due +
    						"\r\nSubmitted: " +a.Submitted +
    						"\r\nGrade:" + a.Grade + "\r\n---\r\n");
    }