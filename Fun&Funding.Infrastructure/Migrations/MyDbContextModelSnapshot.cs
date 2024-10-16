using System;
using Fun_Funding.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Fun_Funding.Infrastructure.Migrations
{
    [DbContext(typeof(MyDbContext))]
    partial class MyDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("CategoryFundingProject", b =>
                {
                    b.Property<Guid>("CategoriesId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ProjectsId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("CategoriesId", "ProjectsId");

                    b.HasIndex("ProjectsId");

                    b.ToTable("CategoryFundingProject");
                });

            modelBuilder.Entity("Fun_Funding.Domain.Entity.BankAccount", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("BankCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("BankNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTimeOffset?>("DeletedAt")
                        .HasColumnType("datetimeoffset");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.ToTable("BankAccount");
                });

            modelBuilder.Entity("Fun_Funding.Domain.Entity.Category", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTimeOffset?>("DeletedAt")
                        .HasColumnType("datetimeoffset");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Category");
                });

            modelBuilder.Entity("Fun_Funding.Domain.Entity.CommissionFee", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("CommissionType")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTimeOffset?>("DeletedAt")
                        .HasColumnType("datetimeoffset");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<decimal>("Rate")
                        .HasColumnType("decimal(18, 2)");

                    b.Property<DateTime>("UpdateDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Version")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("CommissionFee");
                });

            modelBuilder.Entity("Fun_Funding.Domain.Entity.DigitalKey", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTimeOffset?>("DeletedAt")
                        .HasColumnType("datetimeoffset");

                    b.Property<DateTime>("ExpiredDate")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("KeyString")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("MarketplaceProjectId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("MarketplaceProjectId");

                    b.ToTable("DigitalKey");
                });

            modelBuilder.Entity("Fun_Funding.Domain.Entity.FundingFile", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTimeOffset?>("DeletedAt")
                        .HasColumnType("datetimeoffset");

                    b.Property<int>("Filetype")
                        .HasColumnType("int");

                    b.Property<Guid?>("FundingProjectId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("URL")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("FundingProjectId");

                    b.ToTable("FundingFile");
                });

            modelBuilder.Entity("Fun_Funding.Domain.Entity.FundingProject", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<decimal>("Balance")
                        .HasColumnType("decimal(18, 2)");

                    b.Property<Guid?>("BankAccountId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTimeOffset?>("DeletedAt")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Introduction")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<decimal>("Target")
                        .HasColumnType("decimal(18, 2)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("BankAccountId");

                    b.HasIndex("UserId");

                    b.ToTable("FundingProject");
                });

            modelBuilder.Entity("Fun_Funding.Domain.Entity.MarketplaceFile", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTimeOffset?>("DeletedAt")
                        .HasColumnType("datetimeoffset");

                    b.Property<int>("Filetype")
                        .HasColumnType("int");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<Guid?>("MarketingProjectId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("MarketplaceProjectId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("URL")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("MarketplaceProjectId");

                    b.ToTable("MarketplaceFile");
                });

            modelBuilder.Entity("Fun_Funding.Domain.Entity.MarketplaceProject", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTimeOffset?>("DeletedAt")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("FundingProjectId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Introduction")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(18, 2)");

                    b.HasKey("Id");

                    b.HasIndex("FundingProjectId")
                        .IsUnique();

                    b.ToTable("MarketplaceProject");
                });

            modelBuilder.Entity("Fun_Funding.Domain.Entity.Milestone", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTimeOffset?>("DeletedAt")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("DisbursementPercentage")
                        .HasColumnType("decimal(18, 2)");

                    b.Property<int>("Duration")
                        .HasColumnType("int");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("MilestoneName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("MilestoneOrder")
                        .HasColumnType("int");

                    b.Property<DateTime>("UpdateDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("Version")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Milestones");
                });

            modelBuilder.Entity("Fun_Funding.Domain.Entity.Order", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTimeOffset?>("DeletedAt")
                        .HasColumnType("datetimeoffset");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<decimal>("TotalAmount")
                        .HasColumnType("decimal(18, 2)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Order");
                });

            modelBuilder.Entity("Fun_Funding.Domain.Entity.OrderDetail", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTimeOffset?>("DeletedAt")
                        .HasColumnType("datetimeoffset");

                    b.Property<Guid>("DigitalKeyID")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<Guid>("OrderId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("DigitalKeyID");

                    b.HasIndex("OrderId");

                    b.ToTable("OrderDetail");
                });

            modelBuilder.Entity("Fun_Funding.Domain.Entity.Package", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTimeOffset?>("DeletedAt")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<decimal>("LimitQuantity")
                        .HasColumnType("decimal(18, 2)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("PackageStatus")
                        .HasColumnType("int");

                    b.Property<int>("PackageTypes")
                        .HasColumnType("int");

                    b.Property<Guid>("ProjectId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<decimal>("RequiredAmount")
                        .HasColumnType("decimal(18, 2)");

                    b.Property<string>("Url")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("ProjectId");

                    b.ToTable("Package");
                });

            modelBuilder.Entity("Fun_Funding.Domain.Entity.PackageBacker", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTimeOffset?>("DeletedAt")
                        .HasColumnType("datetimeoffset");

                    b.Property<decimal>("DonateAmount")
                        .HasColumnType("decimal(18, 2)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<bool>("IsHidden")
                        .HasColumnType("bit");

                    b.Property<Guid>("PackageId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("PackageId");

                    b.HasIndex("UserId");

                    b.ToTable("PackageBacker");
                });

            modelBuilder.Entity("Fun_Funding.Domain.Entity.ProjectCoupon", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<decimal>("CommissionRate")
                        .HasColumnType("decimal(18, 2)");

                    b.Property<string>("CouponKey")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CouponName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTimeOffset?>("DeletedAt")
                        .HasColumnType("datetimeoffset");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<Guid>("MarketplaceProjectId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("MarketplaceProjectId");

                    b.ToTable("ProjectCoupon");
                });

            modelBuilder.Entity("Fun_Funding.Domain.Entity.ProjectMilestone", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTimeOffset?>("DeletedAt")
                        .HasColumnType("datetimeoffset");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("FundingProjectId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<Guid>("MilestoneId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("FundingProjectId");

                    b.HasIndex("MilestoneId");

                    b.ToTable("ProjectMilestones");
                });

            modelBuilder.Entity("Fun_Funding.Domain.Entity.ProjectMilestoneBacker", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("BackerId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Comment")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTimeOffset?>("DeletedAt")
                        .HasColumnType("datetimeoffset");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<Guid>("ProjectMilestoneId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<decimal>("Star")
                        .HasColumnType("decimal(18, 2)");

                    b.HasKey("Id");

                    b.HasIndex("BackerId");

                    b.HasIndex("ProjectMilestoneId");

                    b.ToTable("ProjectMilestoneBacker");
                });

            modelBuilder.Entity("Fun_Funding.Domain.Entity.ProjectMilestoneRequirement", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTimeOffset?>("DeletedAt")
                        .HasColumnType("datetimeoffset");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<Guid>("ProjectMilestoneId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("RequirementId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("RequirementStatus")
                        .HasColumnType("int");

                    b.Property<DateTime>("UpdateDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("ProjectMilestoneId");

                    b.HasIndex("RequirementId");

                    b.ToTable("ProjectMilestoneRequirements");
                });

            modelBuilder.Entity("Fun_Funding.Domain.Entity.ProjectRequirementFile", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTimeOffset?>("DeletedAt")
                        .HasColumnType("datetimeoffset");

                    b.Property<int>("File")
                        .HasColumnType("int");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("ProjectMilestoneRequirementId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("URL")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("ProjectMilestoneRequirementId");

                    b.ToTable("ProjectRequirementFiles");
                });

            modelBuilder.Entity("Fun_Funding.Domain.Entity.Requirement", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTimeOffset?>("DeletedAt")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<Guid>("MilestoneId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Version")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("MilestoneId");

                    b.ToTable("Requirements");
                });

            modelBuilder.Entity("Fun_Funding.Domain.Entity.RewardItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTimeOffset?>("DeletedAt")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ImageUrl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("PackageId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("PackageId");

                    b.ToTable("RewardItem");
                });

            modelBuilder.Entity("Fun_Funding.Domain.Entity.RewardTracking", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("BackerId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTimeOffset?>("DeletedAt")
                        .HasColumnType("datetimeoffset");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<Guid>("PackageId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("ShipDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("TrackingStatus")
                        .HasColumnType("int");

                    b.Property<Guid?>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("PackageId");

                    b.HasIndex("UserId");

                    b.ToTable("RewardTracking");
                });

            modelBuilder.Entity("Fun_Funding.Domain.Entity.Stage", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTimeOffset?>("DeletedAt")
                        .HasColumnType("datetimeoffset");

                    b.Property<Guid?>("FundingProjectId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<DateTime>("UpdateDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("FundingProjectId");

                    b.ToTable("Stage");
                });

            modelBuilder.Entity("Fun_Funding.Domain.Entity.SystemWallet", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<decimal>("CommissionRate")
                        .HasColumnType("decimal(18, 2)");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTimeOffset?>("DeletedAt")
                        .HasColumnType("datetimeoffset");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<decimal>("TotalAmount")
                        .HasColumnType("decimal(18, 2)");

                    b.HasKey("Id");

                    b.ToTable("SystemWallet");
                });

            modelBuilder.Entity("Fun_Funding.Domain.Entity.Transaction", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("CommissionFeeId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTimeOffset?>("DeletedAt")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<Guid?>("OrderDetailId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("OrderId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("PackageId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("SystemWalletId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<decimal>("TotalAmount")
                        .HasColumnType("decimal(18, 2)");

                    b.Property<int>("TransactionType")
                        .HasColumnType("int");

                    b.Property<Guid?>("WalletId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("CommissionFeeId");

                    b.HasIndex("SystemWalletId");

                    b.HasIndex("WalletId");

                    b.ToTable("Transaction");
                });

            modelBuilder.Entity("Fun_Funding.Domain.Entity.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("Address")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Bio")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DayOfBirth")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("FullName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("Gender")
                        .HasColumnType("int");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<int>("UserStatus")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers", (string)null);
                });

            modelBuilder.Entity("Fun_Funding.Domain.Entity.UserFile", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTimeOffset?>("DeletedAt")
                        .HasColumnType("datetimeoffset");

                    b.Property<int>("Filetype")
                        .HasColumnType("int");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("URL")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("UserId")
                        .IsUnique()
                        .HasFilter("[UserId] IS NOT NULL");

                    b.ToTable("UserFile");
                });

            modelBuilder.Entity("Fun_Funding.Domain.Entity.Wallet", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("BackerId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<decimal>("Balance")
                        .HasColumnType("decimal(18, 2)");

                    b.Property<Guid>("BankAccountId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTimeOffset?>("DeletedAt")
                        .HasColumnType("datetimeoffset");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.HasIndex("BackerId")
                        .IsUnique();

                    b.HasIndex("BankAccountId")
                        .IsUnique();

                    b.ToTable("Wallet");
                });

            modelBuilder.Entity("Fun_Funding.Domain.Entity.WithdrawRequest", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<decimal>("Amount")
                        .HasColumnType("decimal(18, 2)");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTimeOffset?>("DeletedAt")
                        .HasColumnType("datetimeoffset");

                    b.Property<DateTime>("ExpiredDate")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<bool>("IsFinished")
                        .HasColumnType("bit");

                    b.Property<Guid?>("ProjectId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("RequestType")
                        .HasColumnType("int");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<Guid?>("WalletId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("ProjectId");

                    b.HasIndex("WalletId");

                    b.ToTable("WithdrawRequest");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole<System.Guid>", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<System.Guid>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("RoleId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<System.Guid>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<System.Guid>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<System.Guid>", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("RoleId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<System.Guid>", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens", (string)null);
                });

            modelBuilder.Entity("CategoryFundingProject", b =>
                {
                    b.HasOne("Fun_Funding.Domain.Entity.Category", null)
                        .WithMany()
                        .HasForeignKey("CategoriesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Fun_Funding.Domain.Entity.FundingProject", null)
                        .WithMany()
                        .HasForeignKey("ProjectsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Fun_Funding.Domain.Entity.DigitalKey", b =>
                {
                    b.HasOne("Fun_Funding.Domain.Entity.MarketplaceProject", "MarketplaceProject")
                        .WithMany("DigitalKeys")
                        .HasForeignKey("MarketplaceProjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("MarketplaceProject");
                });

            modelBuilder.Entity("Fun_Funding.Domain.Entity.FundingFile", b =>
                {
                    b.HasOne("Fun_Funding.Domain.Entity.FundingProject", null)
                        .WithMany("SourceFiles")
                        .HasForeignKey("FundingProjectId");
                });

            modelBuilder.Entity("Fun_Funding.Domain.Entity.FundingProject", b =>
                {
                    b.HasOne("Fun_Funding.Domain.Entity.BankAccount", "BankAccount")
                        .WithMany()
                        .HasForeignKey("BankAccountId");

                    b.HasOne("Fun_Funding.Domain.Entity.User", "User")
                        .WithMany("FundingProjects")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("BankAccount");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Fun_Funding.Domain.Entity.MarketplaceFile", b =>
                {
                    b.HasOne("Fun_Funding.Domain.Entity.MarketplaceProject", null)
                        .WithMany("MarketingFiles")
                        .HasForeignKey("MarketplaceProjectId");
                });

            modelBuilder.Entity("Fun_Funding.Domain.Entity.MarketplaceProject", b =>
                {
                    b.HasOne("Fun_Funding.Domain.Entity.FundingProject", "FundingProject")
                        .WithOne("MarketingProject")
                        .HasForeignKey("Fun_Funding.Domain.Entity.MarketplaceProject", "FundingProjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("FundingProject");
                });

            modelBuilder.Entity("Fun_Funding.Domain.Entity.Order", b =>
                {
                    b.HasOne("Fun_Funding.Domain.Entity.User", "User")
                        .WithMany("Orders")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("Fun_Funding.Domain.Entity.OrderDetail", b =>
                {
                    b.HasOne("Fun_Funding.Domain.Entity.DigitalKey", "DigitalKey")
                        .WithMany()
                        .HasForeignKey("DigitalKeyID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Fun_Funding.Domain.Entity.Order", "Order")
                        .WithMany("OrderDetails")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("DigitalKey");

                    b.Navigation("Order");
                });

            modelBuilder.Entity("Fun_Funding.Domain.Entity.Package", b =>
                {
                    b.HasOne("Fun_Funding.Domain.Entity.FundingProject", "Project")
                        .WithMany("Packages")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Project");
                });

            modelBuilder.Entity("Fun_Funding.Domain.Entity.PackageBacker", b =>
                {
                    b.HasOne("Fun_Funding.Domain.Entity.Package", "Package")
                        .WithMany("PackageUsers")
                        .HasForeignKey("PackageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Fun_Funding.Domain.Entity.User", "User")
                        .WithMany("PackageUsers")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Package");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Fun_Funding.Domain.Entity.ProjectCoupon", b =>
                {
                    b.HasOne("Fun_Funding.Domain.Entity.MarketplaceProject", "MarketplaceProject")
                        .WithMany("ProjectCoupons")
                        .HasForeignKey("MarketplaceProjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("MarketplaceProject");
                });

            modelBuilder.Entity("Fun_Funding.Domain.Entity.ProjectMilestone", b =>
                {
                    b.HasOne("Fun_Funding.Domain.Entity.FundingProject", "FundingProject")
                        .WithMany("ProjectMilestones")
                        .HasForeignKey("FundingProjectId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Fun_Funding.Domain.Entity.Milestone", "Milestone")
                        .WithMany("ProjectMilestones")
                        .HasForeignKey("MilestoneId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("FundingProject");

                    b.Navigation("Milestone");
                });

            modelBuilder.Entity("Fun_Funding.Domain.Entity.ProjectMilestoneBacker", b =>
                {
                    b.HasOne("Fun_Funding.Domain.Entity.User", "Backer")
                        .WithMany("ProjectMilestoneBackers")
                        .HasForeignKey("BackerId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Fun_Funding.Domain.Entity.ProjectMilestone", "ProjectMilestone")
                        .WithMany("ProjectMilestoneBackers")
                        .HasForeignKey("ProjectMilestoneId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Backer");

                    b.Navigation("ProjectMilestone");
                });

            modelBuilder.Entity("Fun_Funding.Domain.Entity.ProjectMilestoneRequirement", b =>
                {
                    b.HasOne("Fun_Funding.Domain.Entity.ProjectMilestone", "ProjectMilestone")
                        .WithMany("ProjectMilestoneRequirements")
                        .HasForeignKey("ProjectMilestoneId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Fun_Funding.Domain.Entity.Requirement", "Requirement")
                        .WithMany("ProjectRequirements")
                        .HasForeignKey("RequirementId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("ProjectMilestone");

                    b.Navigation("Requirement");
                });

            modelBuilder.Entity("Fun_Funding.Domain.Entity.ProjectRequirementFile", b =>
                {
                    b.HasOne("Fun_Funding.Domain.Entity.ProjectMilestoneRequirement", "ProjectRequirement")
                        .WithMany("RequirementFiles")
                        .HasForeignKey("ProjectMilestoneRequirementId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ProjectRequirement");
                });

            modelBuilder.Entity("Fun_Funding.Domain.Entity.Requirement", b =>
                {
                    b.HasOne("Fun_Funding.Domain.Entity.Milestone", "Milestone")
                        .WithMany("Requirements")
                        .HasForeignKey("MilestoneId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Milestone");
                });

            modelBuilder.Entity("Fun_Funding.Domain.Entity.RewardItem", b =>
                {
                    b.HasOne("Fun_Funding.Domain.Entity.Package", null)
                        .WithMany("RewardItems")
                        .HasForeignKey("PackageId");
                });

            modelBuilder.Entity("Fun_Funding.Domain.Entity.RewardTracking", b =>
                {
                    b.HasOne("Fun_Funding.Domain.Entity.Package", null)
                        .WithMany("RewardTrackings")
                        .HasForeignKey("PackageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Fun_Funding.Domain.Entity.User", null)
                        .WithMany("RewardTrackings")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Fun_Funding.Domain.Entity.Stage", b =>
                {
                    b.HasOne("Fun_Funding.Domain.Entity.FundingProject", "FundingProject")
                        .WithMany("Stages")
                        .HasForeignKey("FundingProjectId");

                    b.Navigation("FundingProject");
                });

            modelBuilder.Entity("Fun_Funding.Domain.Entity.Transaction", b =>
                {
                    b.HasOne("Fun_Funding.Domain.Entity.CommissionFee", "CommissionFee")
                        .WithMany("Transactions")
                        .HasForeignKey("CommissionFeeId");

                    b.HasOne("Fun_Funding.Domain.Entity.SystemWallet", "SystemWallet")
                        .WithMany("Transactions")
                        .HasForeignKey("SystemWalletId");

                    b.HasOne("Fun_Funding.Domain.Entity.Wallet", "Wallet")
                        .WithMany("Transactions")
                        .HasForeignKey("WalletId");

                    b.Navigation("CommissionFee");

                    b.Navigation("SystemWallet");

                    b.Navigation("Wallet");
                });

            modelBuilder.Entity("Fun_Funding.Domain.Entity.UserFile", b =>
                {
                    b.HasOne("Fun_Funding.Domain.Entity.User", null)
                        .WithOne("File")
                        .HasForeignKey("Fun_Funding.Domain.Entity.UserFile", "UserId");
                });

            modelBuilder.Entity("Fun_Funding.Domain.Entity.Wallet", b =>
                {
                    b.HasOne("Fun_Funding.Domain.Entity.User", "Backer")
                        .WithOne("Wallet")
                        .HasForeignKey("Fun_Funding.Domain.Entity.Wallet", "BackerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Fun_Funding.Domain.Entity.BankAccount", null)
                        .WithOne("Wallet")
                        .HasForeignKey("Fun_Funding.Domain.Entity.Wallet", "BankAccountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Backer");
                });

            modelBuilder.Entity("Fun_Funding.Domain.Entity.WithdrawRequest", b =>
                {
                    b.HasOne("Fun_Funding.Domain.Entity.FundingProject", "Project")
                        .WithMany("WithdrawRequests")
                        .HasForeignKey("ProjectId");

                    b.HasOne("Fun_Funding.Domain.Entity.Wallet", "Wallet")
                        .WithMany("WithdrawRequests")
                        .HasForeignKey("WalletId");

                    b.Navigation("Project");

                    b.Navigation("Wallet");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<System.Guid>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole<System.Guid>", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<System.Guid>", b =>
                {
                    b.HasOne("Fun_Funding.Domain.Entity.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<System.Guid>", b =>
                {
                    b.HasOne("Fun_Funding.Domain.Entity.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<System.Guid>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole<System.Guid>", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Fun_Funding.Domain.Entity.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<System.Guid>", b =>
                {
                    b.HasOne("Fun_Funding.Domain.Entity.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Fun_Funding.Domain.Entity.BankAccount", b =>
                {
                    b.Navigation("Wallet");
                });

            modelBuilder.Entity("Fun_Funding.Domain.Entity.CommissionFee", b =>
                {
                    b.Navigation("Transactions");
                });

            modelBuilder.Entity("Fun_Funding.Domain.Entity.FundingProject", b =>
                {
                    b.Navigation("MarketingProject");

                    b.Navigation("Packages");

                    b.Navigation("ProjectMilestones");

                    b.Navigation("SourceFiles");

                    b.Navigation("Stages");

                    b.Navigation("WithdrawRequests");
                });

            modelBuilder.Entity("Fun_Funding.Domain.Entity.MarketplaceProject", b =>
                {
                    b.Navigation("DigitalKeys");

                    b.Navigation("MarketingFiles");

                    b.Navigation("ProjectCoupons");
                });

            modelBuilder.Entity("Fun_Funding.Domain.Entity.Milestone", b =>
                {
                    b.Navigation("ProjectMilestones");

                    b.Navigation("Requirements");
                });

            modelBuilder.Entity("Fun_Funding.Domain.Entity.Order", b =>
                {
                    b.Navigation("OrderDetails");
                });

            modelBuilder.Entity("Fun_Funding.Domain.Entity.Package", b =>
                {
                    b.Navigation("PackageUsers");

                    b.Navigation("RewardItems");

                    b.Navigation("RewardTrackings");
                });

            modelBuilder.Entity("Fun_Funding.Domain.Entity.ProjectMilestone", b =>
                {
                    b.Navigation("ProjectMilestoneBackers");

                    b.Navigation("ProjectMilestoneRequirements");
                });

            modelBuilder.Entity("Fun_Funding.Domain.Entity.ProjectMilestoneRequirement", b =>
                {
                    b.Navigation("RequirementFiles");
                });

            modelBuilder.Entity("Fun_Funding.Domain.Entity.Requirement", b =>
                {
                    b.Navigation("ProjectRequirements");
                });

            modelBuilder.Entity("Fun_Funding.Domain.Entity.SystemWallet", b =>
                {
                    b.Navigation("Transactions");
                });

            modelBuilder.Entity("Fun_Funding.Domain.Entity.User", b =>
                {
                    b.Navigation("File")
                        .IsRequired();

                    b.Navigation("FundingProjects");

                    b.Navigation("Orders");

                    b.Navigation("PackageUsers");

                    b.Navigation("ProjectMilestoneBackers");

                    b.Navigation("RewardTrackings");

                    b.Navigation("Wallet");
                });

            modelBuilder.Entity("Fun_Funding.Domain.Entity.Wallet", b =>
                {
                    b.Navigation("Transactions");

                    b.Navigation("WithdrawRequests");
                });
#pragma warning restore 612, 618
        }
    }
}
