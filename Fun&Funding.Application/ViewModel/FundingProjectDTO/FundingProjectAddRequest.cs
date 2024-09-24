﻿using Fun_Funding.Domain.Entity;
using Fun_Funding.Domain.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fun_Funding.Application.ViewModel.BankAccountDTO;

using Fun_Funding.Application.ViewModel.FundingFileDTO;
using Fun_Funding.Application.ViewModel.PackageDTO;
using Microsoft.AspNetCore.Http;

namespace Fun_Funding.Application.ViewModel.FundingProjectDTO
{
    public class FundingProjectAddRequest
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string Description { get; set; } = string.Empty;
        [Required]
        public string Introduction { get; set; } = string.Empty;

        [Required]
        public DateTime EndDate { get; set; }
        [Required(ErrorMessage = "Target is required.")]
        [Range(0.01, (double)decimal.MaxValue, ErrorMessage = "Target must be greater than 0.")]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Target { get; set; }
        [Required]
        [Range(0, (double)decimal.MaxValue)]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Balance { get; set; }

        [Required]
        public BankAccountRequest? BankAccount { get; set; }
        //public virtual ICollection<Category> Categories { get; set; }
        [Required]
        public virtual ICollection<PackageAddRequest> Packages { get; set; }
        //public IFormFile ThumbnailFile { get; set; }

        //public List<IFormFile> Stories {  get; set; }
        public List<FundingFileRequest> FundingFiles { get; set; }
        [Required]
        public string Email { get; set; }

    }
}
