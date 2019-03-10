using Microsoft.EntityFrameworkCore;
using P01_StudentSystem.Data.Models;

namespace P01_StudentSystem.Data
{
    public class StudentDbContext : DbContext
    {
        public StudentDbContext()
        {
        }

        public StudentDbContext(DbContextOptions<StudentDbContext> options)
            : base(options)
        {

        }

        public DbSet<Course> Courses { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Homework> Homeworks { get; set; }
        public DbSet<StudentCourse> StudentCourses { get; set; }
        public DbSet<Resource> Resources { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Student>()
                        .HasKey(s => s.StudentId);

            modelBuilder.Entity<Student>()
                        .OwnsOne(s => s.Name);

            modelBuilder.Entity<StudentCourse>()
                        .HasKey(s => new { s.StudentId, s.CourseId });

            //modelBuilder.Entity<Homework>(entity =>
            //{
            //    entity.HasOne(e => e.Student)
            //          .WithMany(s => s.HomeworkSubmissions);

            //    entity.HasOne(e => e.Course)
            //          .WithMany(c => c.HomeworkSubmissions);
            //});

            //modelBuilder.Entity<Course>().HasData(new Course[]
            //{
            //    new Course()
            //    {
            //        CourseId = 1,
            //        Name = "Entity Framework",
            //        Description = "Testing",
            //        StartDate = new DateTime(2019, 1, 20),
            //        EndDate = new DateTime(2019, 3, 30)
            //    }
            //});

            //modelBuilder.Entity<Resource>().HasData(new Resource[]
            //{
            //    new Resource()
            //    {
            //        ResourceId = 1,
            //        CourseId = 1,
            //        Name = "Presentation",
            //        ResourceType = ResourceType.Presentation,
            //        Url = "www.studentsystem.my/presentation"
            //    }
            //});
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(Configuration.ConnectionString);
            }
        }
    }
}
