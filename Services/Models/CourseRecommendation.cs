using DataAccess.Models;

namespace Services.Models;

public class CourseRecommendation
{
    public Course Course { get; set; } = null!;
    public string Reason { get; set; } = "";
    public int EnrollmentCount { get; set; }
}
