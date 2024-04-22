﻿// <auto-generated />
using System;
using EduSchool.Models.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace EduSchool.Migrations.Edu
{
    [DbContext(typeof(EduContext))]
    partial class EduContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            MySqlModelBuilderExtensions.AutoIncrementColumns(modelBuilder);

            modelBuilder.Entity("EduSchool.Models.DataModel.Absence", b =>
                {
                    b.Property<int>("AbsenceID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("AbsenceID"));

                    b.Property<int>("AbsenceTypeID")
                        .HasColumnType("int");

                    b.Property<int?>("AbsenceTypeID1")
                        .HasColumnType("int");

                    b.Property<int>("CourseID")
                        .HasColumnType("int");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("StudentID")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.HasKey("AbsenceID");

                    b.HasIndex("AbsenceTypeID");

                    b.HasIndex("AbsenceTypeID1");

                    b.HasIndex("CourseID");

                    b.HasIndex("StudentID");

                    b.ToTable("Absences");
                });

            modelBuilder.Entity("EduSchool.Models.DataModel.AbsenceType", b =>
                {
                    b.Property<int>("AbsenceTypeID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("AbsenceTypeID"));

                    b.Property<string>("TypeName")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("AbsenceTypeID");

                    b.ToTable("AbsenceTypes");
                });

            modelBuilder.Entity("EduSchool.Models.DataModel.Course", b =>
                {
                    b.Property<int>("CourseID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("CourseID"));

                    b.Property<string>("Category")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("InstructorID")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.Property<string>("InstructorName")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("datetime(6)");

                    b.HasKey("CourseID");

                    b.HasIndex("InstructorID");

                    b.ToTable("Courses");
                });

            modelBuilder.Entity("EduSchool.Models.DataModel.Enrollment", b =>
                {
                    b.Property<int>("CourseID")
                        .HasColumnType("int");

                    b.Property<string>("StudentID")
                        .HasColumnType("varchar(255)");

                    b.HasKey("CourseID", "StudentID");

                    b.HasIndex("StudentID");

                    b.ToTable("Enrollments");
                });

            modelBuilder.Entity("EduSchool.Models.DataModel.Grade", b =>
                {
                    b.Property<int>("GradeID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("GradeID"));

                    b.Property<string>("Comment")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("CourseID")
                        .HasColumnType("int");

                    b.Property<DateTime>("GradeDate")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("GradeValue")
                        .HasColumnType("int");

                    b.Property<string>("StudentID")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.Property<int>("Weight")
                        .HasColumnType("int");

                    b.HasKey("GradeID");

                    b.HasIndex("CourseID");

                    b.HasIndex("StudentID");

                    b.ToTable("Grades");
                });

            modelBuilder.Entity("EduSchool.Models.DataModel.User", b =>
                {
                    b.Property<string>("UserID")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("ContactPhoneNumber")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("UserType")
                        .HasColumnType("int");

                    b.HasKey("UserID");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("EduSchool.Models.DataModel.Absence", b =>
                {
                    b.HasOne("EduSchool.Models.DataModel.AbsenceType", "AbsenceType")
                        .WithMany()
                        .HasForeignKey("AbsenceTypeID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("EduSchool.Models.DataModel.AbsenceType", null)
                        .WithMany("Absences")
                        .HasForeignKey("AbsenceTypeID1");

                    b.HasOne("EduSchool.Models.DataModel.Course", "Course")
                        .WithMany("Absences")
                        .HasForeignKey("CourseID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("EduSchool.Models.DataModel.User", "Student")
                        .WithMany("Absences")
                        .HasForeignKey("StudentID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AbsenceType");

                    b.Navigation("Course");

                    b.Navigation("Student");
                });

            modelBuilder.Entity("EduSchool.Models.DataModel.Course", b =>
                {
                    b.HasOne("EduSchool.Models.DataModel.User", "Instructor")
                        .WithMany("CoursesTaught")
                        .HasForeignKey("InstructorID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Instructor");
                });

            modelBuilder.Entity("EduSchool.Models.DataModel.Enrollment", b =>
                {
                    b.HasOne("EduSchool.Models.DataModel.Course", "Course")
                        .WithMany("Enrollments")
                        .HasForeignKey("CourseID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("EduSchool.Models.DataModel.User", "Student")
                        .WithMany("Enrollments")
                        .HasForeignKey("StudentID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Course");

                    b.Navigation("Student");
                });

            modelBuilder.Entity("EduSchool.Models.DataModel.Grade", b =>
                {
                    b.HasOne("EduSchool.Models.DataModel.Course", "Course")
                        .WithMany("Grades")
                        .HasForeignKey("CourseID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("EduSchool.Models.DataModel.User", "Student")
                        .WithMany("Grades")
                        .HasForeignKey("StudentID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Course");

                    b.Navigation("Student");
                });

            modelBuilder.Entity("EduSchool.Models.DataModel.AbsenceType", b =>
                {
                    b.Navigation("Absences");
                });

            modelBuilder.Entity("EduSchool.Models.DataModel.Course", b =>
                {
                    b.Navigation("Absences");

                    b.Navigation("Enrollments");

                    b.Navigation("Grades");
                });

            modelBuilder.Entity("EduSchool.Models.DataModel.User", b =>
                {
                    b.Navigation("Absences");

                    b.Navigation("CoursesTaught");

                    b.Navigation("Enrollments");

                    b.Navigation("Grades");
                });
#pragma warning restore 612, 618
        }
    }
}
