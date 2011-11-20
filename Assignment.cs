using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoodleCrawler
{
    public class Assignment
    {
        public string Topic { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public DateTime Due { get; set; }
        public DateTime Submitted { get; set; }
        public double Grade { get; set; }
        public string ClassName { get; set; }
    }
    public class Course
    {
        public int CourseID { get; set; }
        public string CourseName { get; set; }
        public Course() : this(0, "") { }
        public Course(int CourseID, string ClassName)
        {
            this.CourseID = CourseID;
            this.CourseName = ClassName;
        }
    }
}
