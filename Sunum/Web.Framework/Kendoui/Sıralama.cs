﻿namespace Web.Framework.Kendoui
{
    public class Sort
    {
        public string Field { get; set; }
        public string Dir { get; set; }
        public string ToExpression()
        {
            return Field + " " + Dir;
        }
    }
}
