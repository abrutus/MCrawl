﻿/**
 * Mcrawl.MoodleCrawler
 * Logs in, Fetches all Courses and all their respective assignments
 * Andre Brutus and Class 446
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;
using System.Collections.Specialized;
using System.Net;

namespace MoodleCrawler
{
    public class MoodleCrawler
    {
        // Data structure for the Username and Password
        NameValueCollection credentials;
        // URL where the credentials get posted to.
        private string login_url = "https://eclass.e.southern.edu/login/index.php";

        // URL where the assignment page lives in each class id
        private string assignmentListUrlbyClass = "https://eclass.e.southern.edu/mod/assignment/index.php?id=";

        // URL that gets retrieved after login
        private string postLoginUrl = "https://eclass.e.southern.edu/my/";

        //Course Page Page (for begins-with selector)
        private string courseUrl="https://eclass.e.southern.edu/course/view.php?id=";


        private string html;
        private List<Assignment> assignment_list;
        private List<int> courses;
        public List<Course> ListofCourses;
        public List<Course> getCourses()
        {
            return ListofCourses;
        }


        private CookieAwareWebClient mBrowser;

        public CookieAwareWebClient Browser
        {
            get
            {
                if (mBrowser == null) // init default instance
                    mBrowser = new CookieAwareWebClient();
                return mBrowser;
            }
            set
            {
                mBrowser = value;
            }
        }


        //Constructor
        public MoodleCrawler() : this(null, null)
        {

        }

        public MoodleCrawler(string u, string p)
        {   // Initialize Assignment List
            assignment_list = new List<Assignment>();
            courses = new List<int>();
            ListofCourses = new List<Course>();
            // Save User Information
            credentials = new System.Collections.Specialized.NameValueCollection();
            credentials.Add("username", u);
            credentials.Add("password", p);

            // Start Browser and perform login
            Browser = new CookieAwareWebClient();
            Browser.Encoding = Encoding.UTF8;
            Login();

            //Load the course array
            LoadCourses();
        }


        // Perform login
        public void Login()
        {
            Browser.UploadValues(login_url, credentials);
        }

        // Make sure we have no duplicate courses (if its not -1) then dont add
        public int findinList(List<int> a, int seek)
        {
            for (int i = 0; i < a.Count; i++)
            {
                if (a[i] == seek)
                    return i;
            }
            return -1;
        }

        //Load the CourseID Array (Called upon construction)
        public void LoadCourses()
        {
            // Load Page
            HtmlAgilityPack.HtmlDocument htmldoc = new HtmlAgilityPack.HtmlDocument();

            htmldoc.LoadHtml(html = Browser.DownloadString(postLoginUrl));
            var htmlTopNode = htmldoc.DocumentNode;

            var courses = htmlTopNode.SelectNodes("//a[starts-with(@href, '"+courseUrl+"')]");
            //Add courses to the course<int> array
            foreach (HtmlNode course in courses)
            {
                int found = Convert.ToInt32(course.GetAttributeValue("href", "").Substring(49));
                // Only add if the course isn't in the array already
                if (findinList(this.courses, found) == -1)
                {
                    this.courses.Add(found);
                }
            }
        }
        /**
         * Fetchs all the assignments for a given course 
         */
        List<Assignment> fetchAssignmentsByCourse(int CourseID)
        {
            List<Assignment> alist = new List<Assignment>();
            var html = Browser.DownloadString(assignmentListUrlbyClass + Convert.ToString(CourseID));
            HtmlAgilityPack.HtmlDocument htmldoc = new HtmlAgilityPack.HtmlDocument();
            htmldoc.LoadHtml(html);
            
            // Parse zebra style rows (alternating class r0,r1,r0,r1)
            //var className = htmldoc.DocumentNode.Descendants("a").Where(tr => tr.GetAttributeValue("href", "").Contains("https://eclass.e.southern.edu/course/view.php?id="+CourseID)).ToArray();
            
            var links = htmldoc.DocumentNode.Descendants("tr").Where(tr => tr.GetAttributeValue("class", "").Contains("r0")).ToArray();
            var links2 = htmldoc.DocumentNode.Descendants("tr").Where(tr => tr.GetAttributeValue("class", "").Contains("r1")).ToArray();
            var className = htmldoc.DocumentNode.Descendants("div").Where(div => div.GetAttributeValue("class", "").Contains("coursename")).ToArray();
            ListofCourses.Add(new Course(CourseID, className[0].InnerText));
            int list_size = links.Length + links2.Length;
            List<HtmlNode> assigments = new List<HtmlNode>();
            foreach (HtmlNode l in links)
            {
                assigments.Add(l);
            }
            foreach (HtmlNode l in links2)
            {
                assigments.Add(l);
            }
            int count = 0;

            foreach (HtmlNode single in assigments)
            {
                Assignment hw = new Assignment();
                hw.ClassName = className[0].InnerText;
                foreach (HtmlNode tds in single.ChildNodes)
                {
                    if (tds.Name == "td")
                    {
                        /** 
                         * The DOM is in order and the table columns are as follows:
                         * Topic | Name | Type | Due | Submitted | Grade
                         * So I create the assignment and once I've filled in the grade
                         * (the last data item in order) I append it to my assignment list.
                         */
                        switch (count)
                        {
                            case 0:
                                hw.Topic = tds.InnerText;
                                break;
                            case 1:
                                hw.Name = tds.InnerText;
                                break;
                            case 2:
                                hw.Type = tds.InnerText;
                                break;
                            case 3:
                                hw.Due = DateTime.Parse(tds.InnerText);
                                break;
                            case 4:
                                if (tds.InnerText != "")
                                    hw.Submitted = DateTime.Parse(tds.InnerText);
                                break;
                            case 5:
                                hw.Grade = -1.00;
                                if (tds.InnerText != "" && tds.InnerText.Trim() != "-")
                                    hw.Grade = Convert.ToDouble(tds.InnerText);
                                alist.Add(hw);
                                break;
                            default:
                                count = 0;
                                break;
                        }
                        count++;
                    }
                }
            }

            return alist;
        }
        public string Html()
        {
            return html;
        }

        /**
         * Given the course array (that is loaded during construction)
         * fetch all assignments for all courses
         */
        public List<Assignment> FetchAllAssignments()
        {
            List<Assignment> assignments_from_all_courses = new List<Assignment>();
            // Foreach Courses
            foreach (int a in courses)
            {
                // Foreach assignment in given course
                var b = fetchAssignmentsByCourse(a);
                foreach (Assignment c in b)
                {
                    // Add to assignment list
                    assignments_from_all_courses.Add(c);
                }
            }
            return assignments_from_all_courses;
        }
    }
}
