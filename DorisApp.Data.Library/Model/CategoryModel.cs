﻿using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DorisApp.Data.Library.Model
{
    public class CategoryModel
    {
        public int Id { get; set; }
        public int CreatedByUserId { get; set; }
        public int UpdatedByUserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        private string categoryName = string.Empty;

        [Required]
        [DisplayName("Category Name")]
        public string CategoryName
        {
            get { return categoryName; }
            set { categoryName = value.Trim(); }
        }
    }

}
