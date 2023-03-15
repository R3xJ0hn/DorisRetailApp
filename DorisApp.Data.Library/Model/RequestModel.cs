using System;
using System.Collections.Generic;
using System.Text;

namespace DorisApp.Data.Library.Model
{
    public class RequestModel<T>
    {
        public List<T> Models { get; set; } = new List<T>();

        public int IsInPage { get; set; }

        public int ItemPerPage { get; set; }

        public int TotalPages { get; set; }

        public int TotalItems { get; set; }
    }
}
