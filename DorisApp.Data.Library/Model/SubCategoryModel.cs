﻿using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DorisApp.Data.Library.Model
{
    public class SubCategoryModel
    {
        public int Id { get; set; }
     
        public int CategoryId { get; set; }
        public int CreatedByUserId { get; set; }
        public int UpdatedByUserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        private string subCategoryName = string.Empty;

        [Required]
        [DisplayName("Category Name")]
        public string SubCategoryName
        {
            get { return subCategoryName; }
            set { subCategoryName = value.Trim(); }
        }


    }
}
